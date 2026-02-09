using Luny;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific duration in seconds.
	/// </summary>
	internal sealed class TimerCoroutine : Coroutine
	{
		private Timer _timer;
		private Boolean _elapsedThisTick;

		internal override Double TimeScale
		{
			get => _timer.TimeScale;
			set => _timer.TimeScale = Math.Max(0.0, value);
		}

		public TimerCoroutine(in CoroutineConfig config)
			: base(config)
		{
			var duration = Math.Max(0.0, config.TimerInterval);
			_timer = Timer.FromSeconds(duration);
			_timer.AutoRepeat = config.ContinuationMode == CoroutineContinuationMode.Repeating;
			_timer.OnElapsed += () => _elapsedThisTick = true;
			ContinuationMode = config.ContinuationMode;
		}

		protected override void OnStart() => _timer.Start();
		protected override void OnStop()
		{
			_timer.Stop();
		}

		protected override Boolean OnFrameUpdate()
		{
			_elapsedThisTick = false;
			_timer.Tick(LunyEngine.Instance.Time.DeltaTime);
			return _elapsedThisTick;
		}

		public override String ToString()
		{
			var progress = $"{_timer.Current:F2}s/{_timer.Duration:F2}s";
			return $"{GetType().Name}({Name}, {State}, {progress})";
		}
	}
}
