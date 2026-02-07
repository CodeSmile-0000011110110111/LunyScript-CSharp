using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Configuration options for creating a coroutine.
	/// </summary>
	internal record CoroutineOptions
	{
		private static Int32 s_UniqueNameCounter;
		public String Name { get; init; }
		public Double TimeInterval { get; init; } // Used by Time-based
		public Int32 TargetCount { get; init; } // Used by Count-based
		public Int32 TimeSliceInterval { get; init; }
		public Int32 TimeSliceOffset { get; init; }
		public Boolean IsRepeating { get; init; }
		public Boolean IsTimerOrCounter { get; init; }
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

		public static CoroutineOptions ForTimer(String name, Double duration, Boolean isRepeating, IScriptActionBlock[] onElapsed) => new()
			{ Name = name, TimeInterval = duration, IsRepeating = isRepeating, OnElapsed = onElapsed, IsTimerOrCounter = true };

		public static CoroutineOptions ForCountTimer(String name, Int32 targetCount, Boolean isRepeating, IScriptActionBlock[] onElapsed) =>
			new() { Name = name, TargetCount = targetCount, IsRepeating = isRepeating, OnElapsed = onElapsed, IsTimerOrCounter = true };

		public static CoroutineOptions ForCoroutine(String name) => new() { Name = name, IsTimerOrCounter = false };

		public static CoroutineOptions ForDuration(String name, Double duration, Boolean isRepeating, Boolean isCountBased, Boolean isTimer) =>
			new()
			{
				Name = name,
				TimeInterval = !isCountBased ? duration : 0,
				TargetCount = isCountBased ? (Int32)duration : 0,
				IsRepeating = isRepeating,
				IsTimerOrCounter = isTimer,
			};

		public static CoroutineOptions ForEvery(String name, Int32 everyInterval, EveryCoroutineType type, Int32 delay,
			IScriptActionBlock[] doBlocks) => new()
		{
			Name = name ?? GenerateUniqueName(everyInterval, delay, type),
			TimeSliceInterval = everyInterval,
			TimeSliceOffset = delay,
			OnUpdate = type == EveryCoroutineType.Frames ? doBlocks : null,
			OnHeartbeat = type == EveryCoroutineType.Heartbeats ? doBlocks : null,
			IsTimerOrCounter = false,
		};

		private static String GenerateUniqueName(Int32 interval, Int32 offset, EveryCoroutineType type) =>
			$"<{++s_UniqueNameCounter}> Every({interval}).{type}().DelayBy({offset})";
	}
}
