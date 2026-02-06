using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Specifies how coroutine duration is measured.
	/// </summary>
	internal enum CoroutineDurationType
	{
		/// <summary>
		/// Duration measured in seconds (affected by deltaTime and TimeScale).
		/// </summary>
		TimeBased,
		/// <summary>
		/// Duration measured in heartbeat counts (fixed steps).
		/// </summary>
		HeartbeatCount
	}

	/// <summary>
	/// Represents an individual coroutine/timer instance with runtime state and control methods.
	/// Sequences are pre-created at registration time to avoid runtime allocation.
	/// </summary>
	internal sealed class CoroutineInstance
	{
 		private readonly String _name;
 		private CoroutineState _state = CoroutineState.Stopped;
 		private CoroutineDurationType _durationType = CoroutineDurationType.TimeBased;
 		private Double _elapsedTime;
 		private Double _duration;
 		private Int32 _elapsedCount;
 		private Int32 _targetCount;
 		private Double _timeScale = 1.0;
 		private Boolean _wasPausedByDisable;
 		private Int32 _timeSliceInterval;
 		private Int32 _timeSliceOffset;

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
		internal CoroutineDurationType DurationType => _durationType;
		internal Double ElapsedTime => _elapsedTime;
		internal Double Duration => _duration;
		internal Int32 ElapsedCount => _elapsedCount;
		internal Int32 TargetCount => _targetCount;
		internal Double TimeScale => _timeScale;
		internal Boolean HasDuration => _durationType == CoroutineDurationType.TimeBased ? _duration > 0 : _targetCount > 0;
		internal Boolean IsCountBased => _durationType == CoroutineDurationType.HeartbeatCount;
		internal Boolean WasPausedByDisable => _wasPausedByDisable;
		internal Int32 TimeSliceInterval => _timeSliceInterval;
		internal Int32 TimeSliceOffset => _timeSliceOffset;
		internal Boolean IsTimeSliced => _timeSliceInterval != 0;

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

		internal void SetDuration(Double duration)
		{
			_durationType = CoroutineDurationType.TimeBased;
			_duration = Math.Max(0, duration);
			_targetCount = 0;
		}

		internal void SetHeartbeatCount(Int32 count)
		{
			_durationType = CoroutineDurationType.HeartbeatCount;
			_targetCount = Math.Max(0, count);
			_duration = 0;
		}

		internal void SetTimeSliceInterval(Int32 interval) => _timeSliceInterval = interval;
		internal void SetTimeSliceOffset(Int32 offset) => _timeSliceOffset = Math.Max(0, offset);

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
		/// Starts or restarts the coroutine. Resets elapsed time/count.
		/// Returns true if started fresh (not restarting), indicating Started event should fire.
		/// </summary>
		internal Boolean Start()
		{
			var wasRunning = _state == CoroutineState.Running;
			var wasPaused = _state == CoroutineState.Paused;
			_elapsedTime = 0;
			_elapsedCount = 0;
			_state = CoroutineState.Running;
			_wasPausedByDisable = false;

			// Fire Started event only when starting fresh (not restarting or resuming)
			return !wasRunning && !wasPaused;
		}

		/// <summary>
		/// Stops the coroutine and resets state.
		/// Returns true if the coroutine was running or paused (indicating Stopped event should fire).
		/// </summary>
		internal Boolean Stop()
		{
			if (_state == CoroutineState.Stopped)
				return false;

			_state = CoroutineState.Stopped;
			_elapsedTime = 0;
			_elapsedCount = 0;
			_wasPausedByDisable = false;
			return true;
		}

		/// <summary>
		/// Pauses the coroutine, preserving current elapsed time.
		/// Returns true if the coroutine was running (indicating Paused event should fire).
		/// </summary>
		internal Boolean Pause()
		{
			if (_state != CoroutineState.Running)
				return false;

			_state = CoroutineState.Paused;
			return true;
		}

		/// <summary>
		/// Resumes a paused coroutine.
		/// Returns true if the coroutine was paused (indicating Resumed event should fire).
		/// </summary>
		internal Boolean Resume()
		{
			if (_state != CoroutineState.Paused)
				return false;

			_state = CoroutineState.Running;
			_wasPausedByDisable = false;
			return true;
		}

		/// <summary>
		/// Sets the time scale. Clamped to >= 0 (no negative time).
		/// </summary>
		internal void SetTimeScale(Double scale) => _timeScale = Math.Max(0, scale);

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
		/// Advances a time-based coroutine by deltaTime. Returns true if elapsed (duration reached).
		/// Only use for TimeBased coroutines.
		/// </summary>
		internal Boolean AdvanceTime(Double deltaTime)
		{
			if (_state != CoroutineState.Running || _durationType != CoroutineDurationType.TimeBased)
				return false;

			_elapsedTime += deltaTime * _timeScale;

			if (_duration > 0 && _elapsedTime >= _duration)
			{
				_state = CoroutineState.Stopped;
				return true; // elapsed
			}

			return false;
		}

		/// <summary>
		/// Advances a count-based coroutine by one heartbeat. Returns true if elapsed (count reached).
		/// Only use for HeartbeatCount coroutines.
		/// </summary>
		internal Boolean AdvanceHeartbeat()
		{
			if (_state != CoroutineState.Running || _durationType != CoroutineDurationType.HeartbeatCount)
				return false;

			_elapsedCount++;

			if (_targetCount > 0 && _elapsedCount >= _targetCount)
			{
				_state = CoroutineState.Stopped;
				return true; // elapsed
			}

			return false;
		}

		public override String ToString() =>
			_durationType == CoroutineDurationType.TimeBased
				? $"Coroutine({_name}, {_state}, {_elapsedTime:F2}/{_duration:F2}s)"
				: $"Coroutine({_name}, {_state}, {_elapsedCount}/{_targetCount} heartbeats)";
	}
}
