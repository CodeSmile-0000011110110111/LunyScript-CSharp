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
		public Double Duration { get; init; } // Used by Time-based
		public Int32 TargetCount { get; init; } // Used by Count-based
		public Int32 TimeSliceInterval { get; init; }
		public Int32 TimeSliceOffset { get; init; }
		public Boolean IsRepeating { get; init; }
		public Boolean IsTimer { get; init; }

		// Handlers
		public IScriptActionBlock[] OnUpdate { get; init; }
		public IScriptActionBlock[] OnHeartbeat { get; init; }
		public IScriptActionBlock[] OnElapsed { get; init; }
		public IScriptActionBlock[] OnStarted { get; init; }
		public IScriptActionBlock[] OnStopped { get; init; }
		public IScriptActionBlock[] OnPaused { get; init; }
		public IScriptActionBlock[] OnResumed { get; init; }

		public static CoroutineOptions ForTimer(String name, Double duration, Boolean isRepeating, IScriptActionBlock[] onElapsed) => new()
			{ Name = name, Duration = duration, IsRepeating = isRepeating, OnElapsed = onElapsed, IsTimer = true };

		public static CoroutineOptions ForCountTimer(String name, Int32 targetCount, Boolean isRepeating, IScriptActionBlock[] onElapsed) =>
			new() { Name = name, TargetCount = targetCount, IsRepeating = isRepeating, OnElapsed = onElapsed, IsTimer = true };

		public static CoroutineOptions ForCoroutine(String name) => new() { Name = name, IsTimer = false };

		public static CoroutineOptions ForDuration(String name, Double duration, Boolean isRepeating, Boolean isCountBased, Boolean isTimer) =>
			new()
			{
				Name = name,
				Duration = !isCountBased ? duration : 0,
				TargetCount = isCountBased ? (Int32)duration : 0,
				IsRepeating = isRepeating,
				IsTimer = isTimer,
			};

		public static CoroutineOptions ForEvery(String name, Int32 interval, Int32 offset, IScriptActionBlock[] doBlocks, Boolean isHeartbeat) =>
			new()
			{
				Name = name ?? GenerateUniqueName(interval, offset, isHeartbeat),
				TimeSliceInterval = interval,
				TimeSliceOffset = offset,
				OnUpdate = !isHeartbeat ? doBlocks : null,
				OnHeartbeat = isHeartbeat ? doBlocks : null,
				IsTimer = false,
			};

		private static String GenerateUniqueName(Int32 interval, Int32 offset, Boolean isHeartbeat) =>
			$"__every_{interval}_{(isHeartbeat ? "hb" : "fr")}_{offset}_{++s_UniqueNameCounter}";
	}
}
