using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Represents an individual coroutine/timer instance with runtime state and control methods.
	/// Sequences are pre-created at registration time to avoid runtime allocation.
	/// </summary>
	internal sealed class CoroutineInstance
	{
		private readonly String _name;
		private CoroutineState _state = CoroutineState.Stopped;
		private Single _elapsedTime;
		private Single _duration;
		private Single _timeScale = 1f;
		private Boolean _wasPausedByDisable;

		// Pre-created sequences (created once at registration, no runtime allocation)
		private IScriptSequenceBlock _onUpdateSequence;
		private IScriptSequenceBlock _onHeartbeatSequence;
		private IScriptSequenceBlock _onElapsedSequence;
		private IScriptSequenceBlock _onStartedSequence;
		private IScriptSequenceBlock _onStoppedSequence;
		private IScriptSequenceBlock _onPausedSequence;
		private IScriptSequenceBlock _onResumedSequence;

		internal String Name => _name;
		internal CoroutineState State => _state;
		internal Single ElapsedTime => _elapsedTime;
		internal Single Duration => _duration;
		internal Single TimeScale => _timeScale;
		internal Boolean HasDuration => _duration > 0;
		internal Boolean WasPausedByDisable => _wasPausedByDisable;

		internal IScriptSequenceBlock OnUpdateSequence => _onUpdateSequence;
		internal IScriptSequenceBlock OnHeartbeatSequence => _onHeartbeatSequence;
		internal IScriptSequenceBlock OnElapsedSequence => _onElapsedSequence;
		internal IScriptSequenceBlock OnStartedSequence => _onStartedSequence;
		internal IScriptSequenceBlock OnStoppedSequence => _onStoppedSequence;
		internal IScriptSequenceBlock OnPausedSequence => _onPausedSequence;
		internal IScriptSequenceBlock OnResumedSequence => _onResumedSequence;

		internal CoroutineInstance(String name)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentException("Coroutine name cannot be null or empty", nameof(name));

			_name = name;
		}

		internal void SetDuration(Single duration) => _duration = Math.Max(0, duration);

		internal void SetOnUpdateBlocks(IScriptActionBlock[] blocks) =>
			_onUpdateSequence = CreateSequenceIfNotEmpty(blocks);

		internal void SetOnHeartbeatBlocks(IScriptActionBlock[] blocks) =>
			_onHeartbeatSequence = CreateSequenceIfNotEmpty(blocks);

		internal void SetOnElapsedBlocks(IScriptActionBlock[] blocks) =>
			_onElapsedSequence = CreateSequenceIfNotEmpty(blocks);

		internal void SetOnStartedBlocks(IScriptActionBlock[] blocks) =>
			_onStartedSequence = CreateSequenceIfNotEmpty(blocks);

		internal void SetOnStoppedBlocks(IScriptActionBlock[] blocks) =>
			_onStoppedSequence = CreateSequenceIfNotEmpty(blocks);

		internal void SetOnPausedBlocks(IScriptActionBlock[] blocks) =>
			_onPausedSequence = CreateSequenceIfNotEmpty(blocks);

		internal void SetOnResumedBlocks(IScriptActionBlock[] blocks) =>
			_onResumedSequence = CreateSequenceIfNotEmpty(blocks);

		private static IScriptSequenceBlock CreateSequenceIfNotEmpty(IScriptActionBlock[] blocks) =>
			blocks != null && blocks.Length > 0 ? new SequenceBlock(blocks) : null;

		/// <summary>
		/// Starts or restarts the coroutine. Resets elapsed time.
		/// </summary>
		internal void Start()
		{
			var wasRunning = _state == CoroutineState.Running;
			_elapsedTime = 0;
			_state = CoroutineState.Running;
			_wasPausedByDisable = false;

			// If restarting, no need to fire Started again (implicit stop)
			if (!wasRunning && _onStartedSequence != null)
			{
				// Started sequence will be executed by runner
			}
		}

		/// <summary>
		/// Stops the coroutine and resets state.
		/// </summary>
		internal void Stop()
		{
			if (_state == CoroutineState.Stopped)
				return;

			_state = CoroutineState.Stopped;
			_elapsedTime = 0;
			_wasPausedByDisable = false;
			// Stopped sequence will be executed by runner
		}

		/// <summary>
		/// Pauses the coroutine, preserving current elapsed time.
		/// </summary>
		internal void Pause()
		{
			if (_state != CoroutineState.Running)
				return;

			_state = CoroutineState.Paused;
			// Paused sequence will be executed by runner
		}

		/// <summary>
		/// Resumes a paused coroutine.
		/// </summary>
		internal void Resume()
		{
			if (_state != CoroutineState.Paused)
				return;

			_state = CoroutineState.Running;
			_wasPausedByDisable = false;
			// Resumed sequence will be executed by runner
		}

		/// <summary>
		/// Sets the time scale. Clamped to >= 0 (no negative time).
		/// </summary>
		internal void SetTimeScale(Single scale) => _timeScale = Math.Max(0, scale);

		/// <summary>
		/// Auto-pause when object is disabled.
		/// </summary>
		internal void PauseByDisable()
		{
			if (_state != CoroutineState.Running)
				return;

			_state = CoroutineState.Paused;
			_wasPausedByDisable = true;
		}

		/// <summary>
		/// Auto-resume when object is re-enabled (only if was paused by disable).
		/// </summary>
		internal void ResumeByEnable()
		{
			if (_state != CoroutineState.Paused || !_wasPausedByDisable)
				return;

			_state = CoroutineState.Running;
			_wasPausedByDisable = false;
		}

		/// <summary>
		/// Advances the coroutine by deltaTime. Returns true if elapsed (duration reached).
		/// </summary>
		internal Boolean Advance(Single deltaTime)
		{
			if (_state != CoroutineState.Running)
				return false;

			_elapsedTime += deltaTime * _timeScale;

			if (HasDuration && _elapsedTime >= _duration)
			{
				_state = CoroutineState.Stopped;
				return true; // elapsed
			}

			return false;
		}

		public override String ToString() => $"Coroutine({_name}, {_state}, {_elapsedTime:F2}/{_duration:F2}s)";
	}
}
