using Luny;
using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Base class for coroutines and timers with runtime state and control methods.
	/// </summary>
	internal abstract class Coroutine
	{
		private static readonly String[] s_StateNames = Enum.GetNames<CoroutineState>();

		private readonly String _name;
		private readonly Process _processMode = Process.FrameUpdate;
		private CoroutineState _state = CoroutineState.New;

		internal String Name => _name;
		internal String State => s_StateNames[(Int32)_state];

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
		protected Continuation ContinuationMode { get; } = Continuation.Finite;

		/// <summary>
		/// Factory method to create specialized coroutine instances.
		/// </summary>
		public static Coroutine Create(in Options options) => options.IsTimeSliced ? new TimeSliceCoroutine(options) :
			options.IsCounter ? new CounterCoroutine(options) :
			options.IsTimer ? new TimerCoroutine(options) :
			new PerpetualCoroutine(options);

		private Coroutine() {} // hide default ctor

		protected Coroutine(in Options options)
		{
			if (String.IsNullOrEmpty(options.Name))
				throw new ArgumentException("Coroutine name cannot be null or empty", nameof(options.Name));

			_name = options.Name;
			ContinuationMode = options.ContinuationMode;
			_processMode = options.ProcessMode;
		}

		/// <summary>
		/// Starts or restarts the coroutine.
		/// </summary>
		internal void Start(IScriptRuntimeContext runtimeContext = null)
		{
			if (_state != CoroutineState.New)
				Stop(runtimeContext);

			LunyLogger.LogInfo($"{_name} => START ({GetType().Name})");
			_state = CoroutineState.Running;
			OnStart();
			OnStartedSequence?.Execute(runtimeContext);
		}

		protected abstract void OnStart();

		/// <summary>
		/// Stops the coroutine and resets state.
		/// Returns true if the coroutine was running or paused (indicating Stopped event should fire).
		/// </summary>
		internal Boolean Stop(IScriptRuntimeContext runtimeContext = null)
		{
			CallStartIfNew(runtimeContext);
			if (_state == CoroutineState.Stopped)
				return false;

			LunyLogger.LogInfo($"{_name} => STOP ({GetType().Name})");
			_state = CoroutineState.Stopped;
			OnStop();
			OnStoppedSequence?.Execute(runtimeContext);
			return true;
		}

		protected abstract void OnStop();

		/// <summary>
		/// Pauses the coroutine, preserving current elapsed time.
		/// Returns true if the coroutine was running (indicating Paused event should fire).
		/// </summary>
		internal Boolean Pause(IScriptRuntimeContext runtimeContext = null)
		{
			CallStartIfNew(runtimeContext);
			if (_state != CoroutineState.Running)
				return false;

			LunyLogger.LogInfo($"{_name} => PAUSE ({GetType().Name})");
			_state = CoroutineState.Paused;
			OnPausedSequence?.Execute(runtimeContext);
			return true;
		}

		/// <summary>
		/// Resumes a paused coroutine.
		/// Returns true if the coroutine was paused (indicating Resumed event should fire).
		/// </summary>
		internal Boolean Resume(IScriptRuntimeContext runtimeContext = null)
		{
			CallStartIfNew(runtimeContext);
			if (_state != CoroutineState.Paused)
				return false;

			LunyLogger.LogInfo($"{_name} => RESUME ({GetType().Name})");
			_state = CoroutineState.Running;
			OnResumedSequence?.Execute(runtimeContext);
			return true;
		}

		/// <summary>
		/// Stop coroutine when object is destroyed.
		/// </summary>
		internal void OnObjectDestroyed(IScriptRuntimeContext runtimeContext) => Stop(runtimeContext);

		/// <summary>
		/// Updates coroutine heartbeat state. Returns true if coroutine elapsed.
		/// </summary>
		internal Boolean ProcessHeartbeat(IScriptRuntimeContext runtimeContext)
		{
			if (!ShouldProcess(runtimeContext, Process.Heartbeat))
				return false;

			var elapsed = OnHeartbeat();
			return ResolveState(elapsed, runtimeContext);
		}

		/// <summary>
		/// Updates coroutine frame update state. Returns true if coroutine elapsed.
		/// </summary>
		internal Boolean ProcessFrameUpdate(IScriptRuntimeContext runtimeContext)
		{
			if (!ShouldProcess(runtimeContext, Process.FrameUpdate))
				return false;

			var elapsed = OnFrameUpdate();
			return ResolveState(elapsed, runtimeContext);
		}

		private Boolean ShouldProcess(IScriptRuntimeContext runtimeContext, Process processMode)
		{
			if (_processMode != Process.Always && _processMode != processMode)
				return false;

			CallStartIfNew(runtimeContext);

			return _state == CoroutineState.Running;
		}

		private void CallStartIfNew(IScriptRuntimeContext runtimeContext)
		{
			if (_state == CoroutineState.New)
				Start(runtimeContext);
		}

		private Boolean ResolveState(Boolean elapsed, IScriptRuntimeContext runtimeContext)
		{
			if (!elapsed)
				return false;

			LunyLogger.LogInfo($"{_name} => ELAPSED ({GetType().Name})");

			if (ContinuationMode == Continuation.Repeating)
				Start(runtimeContext);
			else
				Stop(runtimeContext);

			return true;
		}

		protected abstract Boolean OnFrameUpdate();
		protected abstract Boolean OnHeartbeat();
		public abstract override String ToString(); // force implementation of ToString()

		/// <summary>
		/// Configuration options for creating a coroutine.
		/// </summary>
		internal record Options
		{
			private static Int32 s_UniqueNameID;

			public String Name { get; init; }
			public Double TimerInterval { get; init; } // Used only by TimerCoroutine
			public Int32 CounterTarget { get; init; } // Used by CounterCoroutine and TimeSliceCoroutine
			public Int32 TimeSliceInterval { get; init; } // Used only by TimeSliceCoroutine
			public Int32 TimeSliceOffset { get; init; } // Used only by TimeSliceCoroutine
			public Continuation ContinuationMode { get; init; } = Continuation.Finite;
			public Process ProcessMode { get; init; } = Process.Always;

			// Computed properties
			public Boolean IsTimer => TimerInterval > 0d;
			public Boolean IsCounter => CounterTarget > 0 || IsTimeSliced;
			public Boolean IsTimeSliced => TimeSliceInterval != 0;

			// Handlers
			public IScriptActionBlock[] OnFrameUpdate { get; init; }
			public IScriptActionBlock[] OnHeartbeat { get; init; }
			public IScriptActionBlock[] OnElapsed { get; init; }
			public IScriptActionBlock[] OnStarted { get; init; }
			public IScriptActionBlock[] OnStopped { get; init; }
			public IScriptActionBlock[] OnPaused { get; init; }
			public IScriptActionBlock[] OnResumed { get; init; }

			public static Options ForOpenEnded(String name, Process processMode) => new() { Name = name, ProcessMode = processMode };

			public static Options ForTimer(String name, Double duration, Continuation continuationMode, Process processMode) => new()
			{
				Name = name,
				TimerInterval = duration,
				ContinuationMode = continuationMode,
				ProcessMode = processMode,
			};

			public static Options ForCounter(String name, Int32 count, Continuation continuationMode, Process processMode) => new()
			{
				Name = name,
				CounterTarget = count,
				ContinuationMode = continuationMode,
				ProcessMode = processMode,
			};

			public static Options ForEveryInterval(String name, Int32 everyInterval, Process processMode, Int32 delay,
				IScriptActionBlock[] doBlocks) => new()
			{
				Name = name ?? GenerateUniqueName(everyInterval, delay, processMode),
				TimeSliceInterval = everyInterval == LunyScript.Odd || everyInterval == LunyScript.Even ? 2 : everyInterval,
				TimeSliceOffset = everyInterval == LunyScript.Odd ? 1 : delay,
				OnFrameUpdate = processMode == Process.FrameUpdate ? doBlocks : null,
				OnHeartbeat = processMode == Process.Heartbeat ? doBlocks : null,
			};

			private static String GenerateUniqueName(Int32 interval, Int32 delay, Process process) =>
				$"[{++s_UniqueNameID}]__Every({interval}).{process}().DelayBy({delay})";
		}

		/// <summary>
		/// Represents the execution state of a coroutine or timer.
		/// </summary>
		private enum CoroutineState
		{
			/// <summary>
			/// Coroutine has not run before.
			/// </summary>
			New,

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
		/// For Counter coroutines: Whether it counts frames or heartbeats.
		/// </summary>
		internal enum Process
		{
			Always,
			FrameUpdate,
			Heartbeat,
		}

		/// <summary>
		/// Coroutine behaviour after it ran to completion.
		/// </summary>
		internal enum Continuation
		{
			Finite,
			Repeating,
		}
	}
}
