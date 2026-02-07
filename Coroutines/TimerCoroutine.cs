using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Base class for timers which only run Do() blocks when elapsed.
	/// </summary>
	internal abstract class TimerCoroutineBase : CoroutineBase
	{
		protected TimerCoroutineBase(in CoroutineOptions options) : base(options) {}
	}

	/// <summary>
	/// Timer that elapses after a specific duration in seconds.
	/// </summary>
	internal sealed class TimerCoroutine : TimerCoroutineBase
	{
		private Double _elapsedTime;
		private Double _timeScale = 1.0;
		private readonly Double _duration;
		private readonly Boolean _isRepeating;

		public TimerCoroutine(in CoroutineOptions options) : base(options)
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

		public override String ToString() => $"Timer({Name}, {State}, {_elapsedTime:F2}/{_duration:F2}s)";
	}

	/// <summary>
	/// Timer that elapses after a specific number of heartbeats/ticks.
	/// </summary>
	internal sealed class CounterCoroutine : TimerCoroutineBase
	{
		private Int32 _elapsedCount;
		private readonly Int32 _targetCount;
		private readonly Boolean _isRepeating;

		public CounterCoroutine(in CoroutineOptions options) : base(options)
		{
			_targetCount = Math.Max(0, options.TargetCount);
			_isRepeating = options.IsRepeating;
		}

		internal override Boolean IsCountBased => true;

		protected override void ResetState() => _elapsedCount = 0;

		internal override Boolean AdvanceHeartbeat()
		{
			if (_state != CoroutineState.Running)
				return false;

			_elapsedCount++;

			if (_targetCount > 0 && _elapsedCount >= _targetCount)
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

		public override String ToString() => $"Timer({Name}, {State}, {_elapsedCount}/{_targetCount} heartbeats)";
	}
}
