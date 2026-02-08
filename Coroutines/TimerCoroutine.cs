using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific duration in seconds.
	/// </summary>
	internal sealed class TimerCoroutine : PerpetualCoroutine
	{
		private TimeProgress _progress;

		internal override Double TimeScale { get => _progress.TimeScale; set => _progress.TimeScale = Math.Max(0.0, value); }

		public TimerCoroutine(in CoroutineConfig config)
			: base(config)
		{
			_progress.Duration = Math.Max(0.0, config.TimerInterval);
			_progress.TimeScale = 1.0;
			ContinuationMode = config.ContinuationMode;
		}

		protected override void ResetState() => _progress.Reset();

		protected override Boolean OnFrameUpdate(Double deltaTime)
		{
			_progress.AddDeltaTime(deltaTime);
			return _progress.IsElapsed;
		}

		public override String ToString()
		{
			var progress = _progress.IsElapsed ? $"Elapsed: {_progress.Duration:F2}s" : $"{_progress.Current:F2}/{_progress.Duration:F2}";
			return $"{GetType().Name}({Name}, {State}, {progress})";
		}

		private struct TimeProgress
		{
			public Double Current;
			public Double Duration;
			public Double TimeScale;
			public void Reset() => Current = 0.0;
			public void AddDeltaTime(Double dt) => Current += dt * TimeScale;
			public Boolean IsElapsed => Duration > 0.0 && Current >= Duration;
		}
	}
}
