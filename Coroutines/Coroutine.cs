using Luny;
using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	[Flags]
	internal enum CoroutineEvents
	{
		None = 0,
		Started = 1 << 0,
		Resumed = 1 << 1,
		Heartbeat = 1 << 2,
		FrameUpdate = 1 << 3,
		Paused = 1 << 4,
		Stopped = 1 << 5,
		Elapsed = 1 << 6,
	}

	internal static class CoroutineEventsExtensions
	{
		public static Boolean Has(this CoroutineEvents events, CoroutineEvents flag) => (events & flag) != 0;
	}

	/// <summary>
	/// Base class for coroutines and timers with runtime state and control methods.
	/// </summary>
	internal abstract class Coroutine
	{
		private static readonly String[] s_StateNames = Enum.GetNames<CoroutineState>();

		private readonly String _name;
		private readonly Process _processMode = Process.FrameUpdate;
		private CoroutineState _state = CoroutineState.New;
		private CoroutineEvents _pendingEvents = CoroutineEvents.None;

		internal String Name => _name;
		internal String State => s_StateNames[(Int32)_state];

		/// <summary>
		/// Sets the time scale. Clamped to >= 0 (no negative time).
		/// </summary>
		internal virtual Double TimeScale { get => 1.0; set => throw new NotImplementedException(nameof(TimeScale)); }
		protected Continuation ContinuationMode { get; } = Continuation.Finite;
		internal virtual Boolean IsCounterStyle => false;

		/// <summary>
		/// Factory method to create specialized coroutine instances.
		/// </summary>
		public static Coroutine Create(in Options options) =>
			options.IsTimer ? new TimerCoroutine(options) :
			options.IsCounter ? new CounterCoroutine(options) :
			options.IsTimeSliced ? new PerpetualCounterStyleCoroutine(options) :
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
		internal void Start()
		{
			if (_state != CoroutineState.New && _state != CoroutineState.Stopped)
				Stop();

			LunyLogger.LogInfo($"{_name} => START ({GetType().Name})");
			_state = CoroutineState.Running;
			_pendingEvents |= CoroutineEvents.Started;
			OnStart();
		}

		protected abstract void OnStart();

		/// <summary>
		/// Stops the coroutine and resets state.
		/// Returns true if the coroutine was running or paused (indicating Stopped event should fire).
		/// </summary>
		internal Boolean Stop()
		{
			if (_state == CoroutineState.New)
				Start();

			if (_state == CoroutineState.Stopped)
				return false;

			LunyLogger.LogInfo($"{_name} => STOP ({GetType().Name})");
			_state = CoroutineState.Stopped;
			_pendingEvents |= CoroutineEvents.Stopped;
			OnStop();
			return true;
		}

		protected abstract void OnStop();

		/// <summary>
		/// Pauses the coroutine, preserving current elapsed time.
		/// Returns true if the coroutine was running (indicating Paused event should fire).
		/// </summary>
		internal Boolean Pause()
		{
			if (_state == CoroutineState.New)
				Start();

			if (_state != CoroutineState.Running)
				return false;

			LunyLogger.LogInfo($"{_name} => PAUSE ({GetType().Name})");
			_state = CoroutineState.Paused;
			_pendingEvents |= CoroutineEvents.Paused;
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

			LunyLogger.LogInfo($"{_name} => RESUME ({GetType().Name})");
			_state = CoroutineState.Running;
			_pendingEvents |= CoroutineEvents.Resumed;
			return true;
		}

		/// <summary>
		/// Stop coroutine when object is destroyed.
		/// </summary>
		internal void OnObjectDestroyed() => Stop();

		internal CoroutineEvents PollEvents()
		{
			var events = _pendingEvents;
			_pendingEvents = CoroutineEvents.None;
			return events;
		}

		/// <summary>
		/// Updates coroutine heartbeat state. Returns events that occurred.
		/// </summary>
		internal CoroutineEvents ProcessHeartbeat()
		{
			if (_state == CoroutineState.New)
				Start();

			var events = PollEvents();

			if (_state == CoroutineState.Running)
			{
				events |= CoroutineEvents.Heartbeat;
				if (OnHeartbeat())
				{
					events |= CoroutineEvents.Elapsed;
					if (ContinuationMode == Continuation.Repeating)
						Start();
					else
						Stop();
					events |= PollEvents();
				}
			}

			return events;
		}

		/// <summary>
		/// Updates coroutine frame update state. Returns events that occurred.
		/// </summary>
		internal CoroutineEvents ProcessFrameUpdate()
		{
			if (_state == CoroutineState.New)
				Start();

			var events = PollEvents();

			if (_state == CoroutineState.Running)
			{
				events |= CoroutineEvents.FrameUpdate;
				if (OnFrameUpdate())
				{
					events |= CoroutineEvents.Elapsed;
					if (ContinuationMode == Continuation.Repeating)
						Start();
					else
						Stop();
					events |= PollEvents();
				}
			}

			return events;
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
			public Boolean IsCounter => CounterTarget > 0;
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
				TimeSliceOffset = everyInterval == LunyScript.Odd ? 1 : (everyInterval == LunyScript.Even ? 0 : (delay == 0 ? 1 : delay)),
				ProcessMode = processMode,
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
