using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	internal partial class Coroutine
	{
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
			public Boolean IsTimeSliced => TimeSliceInterval > 0;

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

			public static Options ForEveryInterval(String name, Int32 interval, Int32 offset, Process processMode,
				IScriptActionBlock[] doBlocks) => new()
			{
				Name = name ?? GenerateUniqueName(interval, offset, processMode),
				CounterTarget = interval, // time-sliced intervals are always counters
				TimeSliceInterval = Math.Max(1, interval),
				TimeSliceOffset = Math.Max(0, offset),
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
			/// Coroutine has not started yet.
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
