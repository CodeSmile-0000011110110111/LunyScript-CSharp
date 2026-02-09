using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific number of heartbeats/ticks.
	/// </summary>
	internal sealed class CounterCoroutine : Coroutine
	{
		private CountProgress _progress;

		internal override Boolean IsCounter => true;

		public CounterCoroutine(in CoroutineConfig config)
			: base(config)
		{
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
			return $"{GetType().Name}({Name}, {State}, {progress})";
		}
	}
}
