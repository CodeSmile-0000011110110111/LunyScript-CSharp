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
		public Double TimeInterval { get; init; } // Used only by TimerCoroutine
		public Int32 TargetCount { get; init; } // Used only by CounterCoroutine
		public Int32 TimeSliceInterval { get; init; } // Used only by CounterCoroutine
		public Int32 TimeSliceOffset { get; init; } // Used only by CounterCoroutine
		public Boolean IsRepeating { get; init; }

		// Computed properties
		public Boolean IsTimer => TimeInterval > 0d;
		public Boolean IsCounter => TargetCount > 0 || IsTimeSliced;
		public Boolean IsTimeSliced => TimeSliceInterval != 0;

		// Handlers
		public IScriptActionBlock[] OnUpdate { get; init; }
		public IScriptActionBlock[] OnHeartbeat { get; init; }
		public IScriptActionBlock[] OnElapsed { get; init; }
		public IScriptActionBlock[] OnStarted { get; init; }
		public IScriptActionBlock[] OnStopped { get; init; }
		public IScriptActionBlock[] OnPaused { get; init; }
		public IScriptActionBlock[] OnResumed { get; init; }

		public static CoroutineConfig ForCoroutine(String name) => new() { Name = name };

		public static CoroutineConfig ForTimeInterval(String name, Double duration, Boolean isRepeating) => new()
		{
			Name = name,
			TimeInterval = duration,
			IsRepeating = isRepeating,
		};

		public static CoroutineConfig ForTargetCount(String name, Int32 count, Boolean isRepeating) => new()
		{
			Name = name,
			TargetCount = count,
			IsRepeating = isRepeating,
		};

		public static CoroutineConfig ForEvery(String name, Int32 everyInterval, CoroutineCountMode countMode, Int32 delay,
			IScriptActionBlock[] doBlocks) => new()
		{
			Name = name ?? GenerateUniqueName(everyInterval, delay, countMode),
			TimeSliceInterval = everyInterval,
			TimeSliceOffset = delay,
			OnUpdate = countMode == CoroutineCountMode.Frames ? doBlocks : null,
			OnHeartbeat = countMode == CoroutineCountMode.Heartbeats ? doBlocks : null,
		};

		private static String GenerateUniqueName(Int32 interval, Int32 offset, CoroutineCountMode countMode) =>
			$"[{++s_UniqueNameCounter}]__Every({interval}).{countMode}().DelayBy({offset})";
	}
}
