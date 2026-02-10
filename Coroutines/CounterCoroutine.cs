using Luny;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific number of heartbeats/ticks.
	/// </summary>
	internal sealed class CounterCoroutine : PerpetualCoroutine
	{
		private Counter _counter;
		private Boolean _elapsedThisTick;

		internal override Boolean IsCounterStyle => true;

		public CounterCoroutine(in Options options)
			: base(options)
		{
			// Note: `CounterTarget + 1` because we process the current frame,
			// but the meaning of `In(1)` is "next frame" and `In(0)` means "current frame" (asap)
			_counter = new Counter(Math.Max(0, options.CounterTarget + 1));
			_counter.AutoRepeat = options.ContinuationMode == Continuation.Repeating;
			_counter.OnElapsed += () => _elapsedThisTick = true;
		}

		protected override void OnStart() => _counter.Start();

		protected override void OnStop() => _counter.Stop();

		protected override Boolean OnHeartbeat() => IncrementCounter();

		protected override Boolean OnFrameUpdate() => IncrementCounter();

		private Boolean IncrementCounter()
		{
			_elapsedThisTick = false;
			_counter.Increment();
			return _elapsedThisTick;
		}

		public override String ToString()
		{
			var progress = $"{_counter.Current}/{_counter.Target}";
			return $"{GetType().Name}({Name}, {State}, {progress})";
		}
	}
}
