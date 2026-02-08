using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Base class for coroutines and timers with runtime state and control methods.
	/// </summary>
	internal abstract class CoroutineBase
	{
		private readonly String _name;
		private CoroutineState _state = CoroutineState.Stopped;

		internal String Name => _name;
		internal CoroutineState State => _state;

		internal virtual IScriptSequenceBlock OnFrameUpdateSequence => null;
		internal virtual IScriptSequenceBlock OnHeartbeatSequence => null;
		internal virtual IScriptSequenceBlock OnElapsedSequence => null;
		internal virtual IScriptSequenceBlock OnStartedSequence => null;
		internal virtual IScriptSequenceBlock OnStoppedSequence => null;
		internal virtual IScriptSequenceBlock OnPausedSequence => null;
		internal virtual IScriptSequenceBlock OnResumedSequence => null;

		/// <summary>
		/// Sets the time scale. Clamped to >= 0 (no negative time).
		/// </summary>
		internal virtual Double TimeScale { get => 1.0; set => throw new NotImplementedException(nameof(TimeScale)); }
		internal virtual Int32 TimeSliceInterval => 0;
		internal virtual Int32 TimeSliceOffset => 0;
		internal virtual Boolean IsCounter => false;
		internal Boolean IsTimer => !IsCounter;
		internal virtual Boolean IsTimeSliced => false;
		protected CoroutineContinuationMode ContinuationMode { get; set; } = CoroutineContinuationMode.Finite;

		/// <summary>
		/// Factory method to create specialized coroutine instances.
		/// </summary>
		public static CoroutineBase Create(in CoroutineConfig config) => config.IsCounter ? new CounterCoroutine(config) :
			config.IsTimer ? new TimerCoroutine(config) : new PerpetualCoroutine(config);

		private CoroutineBase() {} // hide default ctor

		protected CoroutineBase(in CoroutineConfig config)
		{
			if (String.IsNullOrEmpty(config.Name))
				throw new ArgumentException("Coroutine name cannot be null or empty", nameof(config.Name));

			_name = config.Name;
		}

		/// <summary>
		/// Starts or restarts the coroutine. Always calls Stop() first to reset coroutine state (eg time/count).
		/// </summary>
		/// <returns>True if coroutine was in stopped state. False if coroutine was running or paused (restarting).</returns>
		internal Boolean Start()
		{
			Stop();
			_state = CoroutineState.Running;
			return true;
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
			ResetState();
			return true;
		}

		protected abstract void ResetState();

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
			return true;
		}

		/// <summary>
		/// Stop coroutine when object is destroyed.
		/// </summary>
		internal void OnObjectDestroyed() => Stop();

		/// <summary>
		/// Updates coroutine heartbeat state. Returns true if coroutine elapsed.
		/// </summary>
		internal Boolean ProcessHeartbeat(Double fixedDeltaTime)
		{
			if (_state != CoroutineState.Running)
				return false;

			var elapsed = OnHeartbeat(fixedDeltaTime);
			return ResolveState(elapsed);
		}

		/// <summary>
		/// Updates coroutine frame update state. Returns true if coroutine elapsed.
		/// </summary>
		internal Boolean ProcessFrameUpdate(Double deltaTime)
		{
			if (_state != CoroutineState.Running)
				return false;

			var elapsed = OnFrameUpdate(deltaTime);
			return ResolveState(elapsed);
		}

		private Boolean ResolveState(Boolean elapsed)
		{
			if (!elapsed)
				return false;

			if (ContinuationMode == CoroutineContinuationMode.Repeating)
				Start();
			else
				Stop();

			return true;
		}

		protected abstract Boolean OnFrameUpdate(Double deltaTime);
		protected abstract Boolean OnHeartbeat(Double fixedDeltaTime);
		public abstract override String ToString(); // force implementation of ToString()
	}
}
