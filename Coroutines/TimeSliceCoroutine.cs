using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific number of heartbeats/ticks, 
	/// and only executes its sequences on specific heartbeat intervals (time-sliced).
	/// </summary>
	internal sealed class TimeSliceCoroutine : Coroutine
	{
		private CountProgress _progress;

		internal override Int32 TimeSliceInterval { get; }
		internal override Int32 TimeSliceOffset { get; }
		internal override Boolean IsTimeSliced => true;
		internal override Boolean IsCounter => true;

		public TimeSliceCoroutine(in CoroutineConfig config)
			: base(config)
		{
			TimeSliceInterval = config.TimeSliceInterval;
			TimeSliceOffset = Math.Max(0, config.TimeSliceOffset);
			_progress.Target = Math.Max(0, config.CounterTarget);
			ContinuationMode = config.ContinuationMode;
		}

		protected override void ResetState() => _progress.Reset();

		protected override Boolean OnHeartbeat()
		{
			_progress.IncrementCount();
			return _progress.IsElapsed;
		}

		public override String ToString()
		{
			var progress = _progress.IsElapsed ? $"Elapsed: {_progress.Target:F2}s" : $"{_progress.Current:F2}s/{_progress.Target:F2}s";
			return $"{GetType().Name}({Name}, {State}, {progress}, Interval: {TimeSliceInterval}, Offset: {TimeSliceOffset})";
		}
	}
}
