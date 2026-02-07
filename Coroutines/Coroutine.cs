using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that never elapses automatically (runs until explicitly stopped).
	/// Base for all other coroutine types that utilize full lifecycle events.
	/// </summary>
	internal class Coroutine : CoroutineBase
	{
		protected readonly IScriptSequenceBlock _onUpdateSequence;
		protected readonly IScriptSequenceBlock _onHeartbeatSequence;
		protected readonly IScriptSequenceBlock _onStartedSequence;
		protected readonly IScriptSequenceBlock _onStoppedSequence;
		protected readonly IScriptSequenceBlock _onPausedSequence;
		protected readonly IScriptSequenceBlock _onResumedSequence;

		internal override IScriptSequenceBlock OnUpdateSequence => _onUpdateSequence;
		internal override IScriptSequenceBlock OnHeartbeatSequence => _onHeartbeatSequence;
		internal override IScriptSequenceBlock OnStartedSequence => _onStartedSequence;
		internal override IScriptSequenceBlock OnStoppedSequence => _onStoppedSequence;
		internal override IScriptSequenceBlock OnPausedSequence => _onPausedSequence;
		internal override IScriptSequenceBlock OnResumedSequence => _onResumedSequence;

		public Coroutine(in CoroutineOptions options)
			: base(options)
		{
			_onUpdateSequence = SequenceBlock.TryCreate(options.OnUpdate);
			_onHeartbeatSequence = SequenceBlock.TryCreate(options.OnHeartbeat);
			_onStartedSequence = SequenceBlock.TryCreate(options.OnStarted);
			_onStoppedSequence = SequenceBlock.TryCreate(options.OnStopped);
			_onPausedSequence = SequenceBlock.TryCreate(options.OnPaused);
			_onResumedSequence = SequenceBlock.TryCreate(options.OnResumed);
		}

		public override String ToString() => $"Coroutine({Name}, {State}, Infinite)";
	}

	/// <summary>
	/// Coroutine that elapses after a specific duration in seconds.
	/// </summary>
	internal sealed class TimerCoroutine : Coroutine
	{
		private readonly Boolean _isRepeating;
		private TimeProgress _progress;

		internal override Double TimeScale => _progress.Scale;
		protected override Boolean IsRepeating => _isRepeating;

		public TimerCoroutine(in CoroutineOptions options)
			: base(options)
		{
			_progress.Duration = Math.Max(0, options.TimeInterval);
			_progress.Scale = 1.0;
			_isRepeating = options.IsRepeating;
		}

		internal override void SetTimeScale(Double scale) => _progress.Scale = Math.Max(0, scale);

		protected override void ResetState() => _progress.Reset();

		protected override void AccumulateTime(Double deltaTime) => _progress.Step(deltaTime);
		protected override Boolean HasElapsed() => _progress.IsElapsed;

		public override String ToString() => $"Coroutine({Name}, {State}, {_progress.Elapsed:F2}/{_progress.Duration:F2}s)";
	}

	/// <summary>
	/// Coroutine that elapses after a specific number of heartbeats/ticks.
	/// </summary>
	internal sealed class CounterCoroutine : Coroutine
	{
		private readonly Int32 _timeSliceInterval;
		private readonly Int32 _timeSliceOffset;
		private readonly Boolean _isRepeating;
		private CountProgress _progress;

		internal override Boolean IsCountBased => true;
		internal override Boolean IsTimeSliced => _timeSliceInterval != 0;
		internal override Int32 TimeSliceInterval => _timeSliceInterval;
		internal override Int32 TimeSliceOffset => _timeSliceOffset;
		protected override Boolean IsRepeating => _isRepeating;

		public CounterCoroutine(in CoroutineOptions options)
			: base(options)
		{
			_progress.Target = Math.Max(0, options.TargetCount);
			_timeSliceInterval = options.TimeSliceInterval;
			_timeSliceOffset = Math.Max(0, options.TimeSliceOffset);
			_isRepeating = options.IsRepeating;
		}

		protected override void ResetState() => _progress.Reset();

		protected override void AccumulateHeartbeat() => _progress.Step();
		protected override Boolean HasElapsed() => _progress.IsElapsed;

		public override String ToString() => $"Coroutine({Name}, {State}, {_progress.Elapsed}/{_progress.Target} heartbeats)";
	}
}
