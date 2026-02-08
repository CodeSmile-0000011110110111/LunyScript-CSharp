using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific number of heartbeats/ticks.
	/// </summary>
	internal sealed class CounterCoroutine : PerpetualCoroutine
	{
		private CountProgress _progress;

		internal override Int32 TimeSliceInterval { get; }
		internal override Int32 TimeSliceOffset { get; }
		internal override Boolean IsTimeSliced => TimeSliceInterval != 0;
		internal override Boolean IsCounter { get; } = true;

		public CounterCoroutine(in CoroutineConfig config)
			: base(config)
		{
			TimeSliceInterval = config.CounterTimeSliceInterval;
			TimeSliceOffset = Math.Max(0, config.CounterTimeSliceOffset);
			_progress.Target = Math.Max(0, config.CounterTarget);
			ContinuationMode = config.ContinuationMode;
		}

		protected override void ResetState() => _progress.Reset();

		protected override Boolean OnHeartbeat(Double fixedDeltaTime)
		{
			_progress.IncrementCount();
			return _progress.IsElapsed;
		}

		public override String ToString()
		{
			var progress = _progress.IsElapsed ? $"Elapsed: {_progress.Target:F2}s" : $"{_progress.Current:F2}/{_progress.Target:F2}";
			return $"{GetType().Name}({Name}, {State}, {progress})";
		}

		private struct CountProgress
		{
			public Int32 Current;
			public Int32 Target;
			public void Reset() => Current = 0;
			public void IncrementCount() => Current++;
			public Boolean IsElapsed => Target > 0 && Current >= Target;
		}
	}
}
