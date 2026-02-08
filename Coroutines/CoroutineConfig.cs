using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Configuration options for creating a coroutine.
	/// </summary>
	internal record CoroutineConfig
	{
		private static Int32 s_UniqueNameCounter;

		public String Name { get; init; }
		public Double TimerInterval { get; init; } // Used only by TimerCoroutine
		public Int32 CounterTarget { get; init; } // Used only by CounterCoroutine
		public Int32 CounterTimeSliceInterval { get; init; } // Used only by CounterCoroutine
		public Int32 CounterTimeSliceOffset { get; init; } // Used only by CounterCoroutine
		public CoroutineContinuationMode ContinuationMode { get; init; }

		// Computed properties
		public Boolean IsTimer => TimerInterval > 0d;
		public Boolean IsCounter => CounterTarget > 0 || IsCounterTimeSliced;
		public Boolean IsCounterTimeSliced => CounterTimeSliceInterval != 0;

		// Handlers
		public IScriptActionBlock[] OnFrameUpdate { get; init; }
		public IScriptActionBlock[] OnHeartbeat { get; init; }
		public IScriptActionBlock[] OnElapsed { get; init; }
		public IScriptActionBlock[] OnStarted { get; init; }
		public IScriptActionBlock[] OnStopped { get; init; }
		public IScriptActionBlock[] OnPaused { get; init; }
		public IScriptActionBlock[] OnResumed { get; init; }

		public static CoroutineConfig ForOpenEnded(String name) => new() { Name = name };

		public static CoroutineConfig ForTimer(String name, Double duration, CoroutineContinuationMode continuationMode) => new()
		{
			Name = name,
			TimerInterval = duration,
			ContinuationMode = continuationMode,
		};

		public static CoroutineConfig ForCounter(String name, Int32 count, CoroutineContinuationMode continuationMode) => new()
		{
			Name = name,
			CounterTarget = count,
			ContinuationMode = continuationMode,
		};

		public static CoroutineConfig ForEveryInterval(String name, Int32 everyInterval, CoroutineCountMode countMode, Int32 delay,
			IScriptActionBlock[] doBlocks) => new()
		{
			Name = name ?? GenerateUniqueName(everyInterval, delay, countMode),
			CounterTimeSliceInterval = everyInterval < 0 ? 2 : everyInterval,
			CounterTimeSliceOffset = everyInterval == LunyScript.Odd ? 1 : delay,
			OnFrameUpdate = countMode == CoroutineCountMode.Frames ? doBlocks : null,
			OnHeartbeat = countMode == CoroutineCountMode.Heartbeats ? doBlocks : null,
		};

		private static String GenerateUniqueName(Int32 interval, Int32 delay, CoroutineCountMode countMode) =>
			$"[{++s_UniqueNameCounter}]__Every({interval}).{countMode}().DelayBy({delay})";
	}
}
