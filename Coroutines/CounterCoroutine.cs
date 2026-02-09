using Luny;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific number of heartbeats/ticks.
	/// </summary>
	internal sealed class CounterCoroutine : Coroutine
	{
		private Counter _counter;
		private Boolean _elapsedThisTick;

		internal override Boolean IsCounter => true;

		public CounterCoroutine(in CoroutineConfig config)
			: base(config)
		{
			_counter = new Counter(Math.Max(0, config.CounterTarget));
			_counter.AutoRepeat = config.ContinuationMode == CoroutineContinuationMode.Repeating;
			_counter.OnElapsed += () => _elapsedThisTick = true;
			ContinuationMode = config.ContinuationMode;
		}

		protected override void OnStart() => _counter.Start();

		protected override void OnStop()
		{
			_counter.Stop();
		}

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
