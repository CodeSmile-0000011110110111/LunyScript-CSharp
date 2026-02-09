using LunyScript.Blocks;
using LunyScript.Execution;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Base class for coroutines and timers with runtime state and control methods.
	/// </summary>
	internal abstract class CoroutineBase
	{
		private readonly String _name;
		private CoroutineState _state = CoroutineState.New;

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
		internal virtual Boolean IsTimeSliced => false;
		internal virtual Boolean IsCounter => false;
		internal Boolean IsTimer => !IsCounter;
		internal Boolean CanProcess => _state != CoroutineState.Stopped && _state != CoroutineState.Paused;
		protected CoroutineContinuationMode ContinuationMode { get; set; } = CoroutineContinuationMode.Finite;

		/// <summary>
		/// Factory method to create specialized coroutine instances.
		/// </summary>
		public static CoroutineBase Create(in CoroutineConfig config) => config.IsTimeSliced ? new TimeSliceCoroutine(config) :
			config.IsCounter ? new CounterCoroutine(config) :
			config.IsTimer ? new TimerCoroutine(config) :
			new Coroutine(config);

		private CoroutineBase() {} // hide default ctor

		protected CoroutineBase(in CoroutineConfig config)
		{
			if (String.IsNullOrEmpty(config.Name))
				throw new ArgumentException("Coroutine name cannot be null or empty", nameof(config.Name));

			_name = config.Name;
		}

		/// <summary>
		/// First-time initialization of coroutine. Will Start() the coroutine
		/// </summary>
		internal void Init(ILunyScriptContext context)
		{
			if (_state != CoroutineState.New)
				return;

			OnStop();
			_state = CoroutineState.Stopped; // avoid Stop blocks executing
			Start(context);
		}

		/// <summary>
		/// Starts or restarts the coroutine.
		/// </summary>
		internal void Start(ILunyScriptContext context = null)
		{
			Stop(context);

			_state = CoroutineState.Running;
			OnStart();
			OnStartedSequence?.Execute(context);
		}

		protected abstract void OnStart();

		/// <summary>
		/// Stops the coroutine and resets state.
		/// Returns true if the coroutine was running or paused (indicating Stopped event should fire).
		/// </summary>
		internal Boolean Stop(ILunyScriptContext context = null)
		{
			if (_state == CoroutineState.Stopped)
				return false;

			_state = CoroutineState.Stopped;
			OnStop();
			OnStoppedSequence?.Execute(context);
			return true;
		}

		protected abstract void OnStop();

		/// <summary>
		/// Pauses the coroutine, preserving current elapsed time.
		/// Returns true if the coroutine was running (indicating Paused event should fire).
		/// </summary>
		internal Boolean Pause(ILunyScriptContext context = null)
		{
			if (_state != CoroutineState.Running)
				return false;

			_state = CoroutineState.Paused;
			OnPausedSequence?.Execute(context);
			return true;
		}

		/// <summary>
		/// Resumes a paused coroutine.
		/// Returns true if the coroutine was paused (indicating Resumed event should fire).
		/// </summary>
		internal Boolean Resume(ILunyScriptContext context = null)
		{
			if (_state != CoroutineState.Paused)
				return false;

			_state = CoroutineState.Running;
			OnResumedSequence?.Execute(context);
			return true;
		}

		/// <summary>
		/// Stop coroutine when object is destroyed.
		/// </summary>
		internal void OnObjectDestroyed(ILunyScriptContext context) => Stop(context);

		/// <summary>
		/// Updates coroutine heartbeat state. Returns true if coroutine elapsed.
		/// </summary>
		internal Boolean ProcessHeartbeat(ILunyScriptContext context)
		{
			if (_state != CoroutineState.Running)
				return false;

			var elapsed = OnHeartbeat();
			return ResolveState(elapsed, context);
		}

		/// <summary>
		/// Updates coroutine frame update state. Returns true if coroutine elapsed.
		/// </summary>
		internal Boolean ProcessFrameUpdate(ILunyScriptContext context)
		{
			if (_state != CoroutineState.Running)
				return false;

			var elapsed = OnFrameUpdate();
			return ResolveState(elapsed, context);
		}

		private Boolean ResolveState(Boolean elapsed, ILunyScriptContext context)
		{
			if (!elapsed)
				return false;

			if (ContinuationMode == CoroutineContinuationMode.Repeating)
				Start(context);
			else
				Stop(context);

			return true;
		}

		protected abstract Boolean OnFrameUpdate();
		protected abstract Boolean OnHeartbeat();
		public abstract override String ToString(); // force implementation of ToString()
	}
}
