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

		internal override Boolean IsCounter => true;

		public CounterCoroutine(in Options options)
			: base(options)
		{
			_counter = new Counter(Math.Max(0, options.CounterTarget));
			_counter.AutoRepeat = options.ContinuationMode == Continuation.Repeating;
			_counter.OnElapsed += () => _elapsedThisTick = true;
			ContinuationMode = options.ContinuationMode;
		}

		protected override void OnStart() => _counter.Start();

		protected override void OnStop() => _counter.Stop();

		protected override Boolean OnHeartbeat()
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
