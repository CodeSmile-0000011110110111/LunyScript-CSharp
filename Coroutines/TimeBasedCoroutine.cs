using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific duration in seconds.
	/// </summary>
	internal sealed class TimeBasedCoroutine : Coroutine
	{
		private Double _elapsedTime;
		private Double _timeScale = 1.0;
		private readonly Double _duration;
		private readonly Boolean _isRepeating;

		public TimeBasedCoroutine(in CoroutineOptions options) : base(options)
		{
			_duration = Math.Max(0, options.Duration);
			_isRepeating = options.IsRepeating;
		}

		internal override Double TimeScale => _timeScale;
		internal override void SetTimeScale(Double scale) => _timeScale = Math.Max(0, scale);

		protected override void ResetState() => _elapsedTime = 0;

		internal override Boolean AdvanceTime(Double deltaTime)
		{
			if (_state != CoroutineState.Running)
				return false;

			_elapsedTime += deltaTime * TimeScale;

			if (_duration > 0 && _elapsedTime >= _duration)
			{
				if (_isRepeating)
				{
					Start();
					return true;
				}

				_state = CoroutineState.Stopped;
				return true; // elapsed
			}

			return false;
		}

		public override String ToString() => $"Coroutine({Name}, {State}, {_elapsedTime:F2}/{_duration:F2}s)";
	}
}
