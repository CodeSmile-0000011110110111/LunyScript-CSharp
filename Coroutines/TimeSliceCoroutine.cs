using Luny;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that never elapses and only executes its sequences on specific heartbeat/frame intervals (time-sliced).
	/// </summary>
	internal sealed class TimeSliceCoroutine : PerpetualCoroutine
	{
		private Counter _counter;

		internal override Int32 TimeSliceInterval { get; }
		internal override Int32 TimeSliceOffset { get; }
		internal override Boolean IsTimeSliced => true;
		internal override Boolean IsCounter => true;

		public TimeSliceCoroutine(in Options options)
			: base(options)
		{
			TimeSliceInterval = options.TimeSliceInterval;
			TimeSliceOffset = Math.Max(0, options.TimeSliceOffset);
			ContinuationMode = options.ContinuationMode; // ignored; time-sliced never elapses

			_counter = new Counter(Int32.MaxValue);
			_counter.AutoRepeat = true;
		}

		protected override void OnStart() => _counter.Start();
		protected override void OnStop() => _counter.Stop();

		protected override Boolean OnHeartbeat()
		{
			_counter.Increment();
			return false; // time-sliced coroutines don't elapse
		}

		public override String ToString() =>
			$"{GetType().Name}({Name}, {State}, Count: {_counter.Current}, Interval: {TimeSliceInterval}, Offset: {TimeSliceOffset})";
	}
}
