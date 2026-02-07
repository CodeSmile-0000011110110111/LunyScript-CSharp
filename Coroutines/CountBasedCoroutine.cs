using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific number of heartbeats/ticks.
	/// </summary>
	internal sealed class CountBasedCoroutine : Coroutine
	{
		private Int32 _elapsedCount;
		private readonly Int32 _targetCount;
		private readonly Int32 _timeSliceInterval;
		private readonly Int32 _timeSliceOffset;
		private readonly Boolean _isRepeating;

		public CountBasedCoroutine(in CoroutineOptions options) : base(options)
		{
			_targetCount = Math.Max(0, options.TargetCount);
			_timeSliceInterval = options.TimeSliceInterval;
			_timeSliceOffset = Math.Max(0, options.TimeSliceOffset);
			_isRepeating = options.IsRepeating;
		}

		internal override Boolean IsCountBased => true;
		internal override Boolean IsTimeSliced => _timeSliceInterval != 0;
		internal override Int32 TimeSliceInterval => _timeSliceInterval;
		internal override Int32 TimeSliceOffset => _timeSliceOffset;

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

		public override String ToString() => $"Coroutine({Name}, {State}, {_elapsedCount}/{_targetCount} heartbeats)";
	}
}
