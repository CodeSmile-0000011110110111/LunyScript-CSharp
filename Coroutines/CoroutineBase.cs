using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Represents the execution state of a coroutine or timer.
	/// </summary>
	internal enum CoroutineState
	{
		/// <summary>
		/// Coroutine is not running and has no accumulated time.
		/// </summary>
		Stopped,

		/// <summary>
		/// Coroutine is actively running and accumulating time.
		/// </summary>
		Running,

		/// <summary>
		/// Coroutine is frozen at current time, will resume when unpaused.
		/// </summary>
		Paused,
	}

	/// <summary>
	/// Base class for coroutines and timers with runtime state and control methods.
	/// </summary>
	internal abstract class CoroutineBase
	{
		private readonly String _name;
		protected CoroutineState _state = CoroutineState.Stopped;
		private Boolean _wasPausedByDisable;

		protected readonly IScriptSequenceBlock _onElapsedSequence;

		internal String Name => _name;
		internal CoroutineState State => _state;
		internal Boolean WasPausedByDisable => _wasPausedByDisable;

		internal virtual IScriptSequenceBlock OnUpdateSequence => null;
		internal virtual IScriptSequenceBlock OnHeartbeatSequence => null;
		internal virtual IScriptSequenceBlock OnElapsedSequence => _onElapsedSequence;
		internal virtual IScriptSequenceBlock OnStartedSequence => null;
		internal virtual IScriptSequenceBlock OnStoppedSequence => null;
		internal virtual IScriptSequenceBlock OnPausedSequence => null;
		internal virtual IScriptSequenceBlock OnResumedSequence => null;

		internal virtual Boolean IsCountBased => false;
		internal virtual Boolean IsTimeSliced => false;
		internal virtual Int32 TimeSliceInterval => 0;
		internal virtual Int32 TimeSliceOffset => 0;
		internal virtual Double TimeScale => 1.0;

		/// <summary>
		/// Factory method to create specialized coroutine instances.
		/// </summary>
		public static CoroutineBase Create(in CoroutineOptions options)
		{
			if (options.IsTimer)
			{
				if (options.TargetCount > 0)
					return new CounterCoroutine(options);

				return new TimerCoroutine(options);
			}

			if (options.TargetCount > 0 || options.TimeSliceInterval != 0)
				return new CountBasedCoroutine(options);

			if (options.Duration > 0)
				return new TimeBasedCoroutine(options);

			return new Coroutine(options);
		}

		protected CoroutineBase(in CoroutineOptions options)
		{
			if (String.IsNullOrEmpty(options.Name))
				throw new ArgumentException("Coroutine name cannot be null or empty", nameof(options.Name));

			_name = options.Name;
			_onElapsedSequence = CreateSequenceIfNotEmpty(options.OnElapsed);
		}

		protected static IScriptSequenceBlock CreateSequenceIfNotEmpty(IScriptActionBlock[] blocks) =>
			blocks != null && blocks.Length > 0 ? new SequenceBlock(blocks) : null;

		/// <summary>
		/// Starts or restarts the coroutine. Resets elapsed time/count.
		/// Returns true if started fresh (not restarting), indicating Started event should fire.
		/// </summary>
		internal virtual Boolean Start()
		{
			Stop();
			_state = CoroutineState.Running;
			return true;
		}

		/// <summary>
		/// Stops the coroutine and resets state.
		/// Returns true if the coroutine was running or paused (indicating Stopped event should fire).
		/// </summary>
		internal virtual Boolean Stop()
		{
			if (_state == CoroutineState.Stopped)
				return false;

			_state = CoroutineState.Stopped;
			_wasPausedByDisable = false;
			ResetState();
			return true;
		}

		protected virtual void ResetState() {}

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
		internal virtual void SetTimeScale(Double scale) {}

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
		/// </summary>
		internal virtual Boolean AdvanceTime(Double deltaTime) => false;

		/// <summary>
		/// Advances a count-based coroutine by one heartbeat. Returns true if elapsed (count reached).
		/// </summary>
		internal virtual Boolean AdvanceHeartbeat() => false;

		public abstract override String ToString();
	}
}
