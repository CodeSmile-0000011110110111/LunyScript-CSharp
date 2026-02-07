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

		public Coroutine(in CoroutineConfig config)
			: base(config)
		{
			_onUpdateSequence = SequenceBlock.TryCreate(config.OnUpdate);
			_onHeartbeatSequence = SequenceBlock.TryCreate(config.OnHeartbeat);
			_onStartedSequence = SequenceBlock.TryCreate(config.OnStarted);
			_onStoppedSequence = SequenceBlock.TryCreate(config.OnStopped);
			_onPausedSequence = SequenceBlock.TryCreate(config.OnPaused);
			_onResumedSequence = SequenceBlock.TryCreate(config.OnResumed);
		}

		public override String ToString() => $"{GetType().Name}({Name}, {State})";
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

		public TimerCoroutine(in CoroutineConfig config)
			: base(config)
		{
			_progress.Duration = Math.Max(0.0, config.TimeInterval);
			_progress.Scale = 1.0;
			_isRepeating = config.IsRepeating;
		}

		internal override void SetTimeScale(Double scale) => _progress.Scale = Math.Max(0.0, scale);

		protected override void ResetState() => _progress.Reset();

		protected override void AdvanceTime(Double deltaTime) => _progress.AddDeltaTime(deltaTime);
		protected override Boolean HasElapsed() => _progress.IsElapsed;

		public override String ToString()
		{
			var progress = HasElapsed() ? $"Elapsed: {_progress.Duration:F2}s" : $"{_progress.Progress:F2}/{_progress.Duration:F2}";
			return $"{GetType().Name}({Name}, {State}, {progress})";
		}
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

		internal override Int32 TimeSliceInterval => _timeSliceInterval;
		internal override Int32 TimeSliceOffset => _timeSliceOffset;
		internal override Boolean IsTimeSliced => _timeSliceInterval != 0;
		protected override Boolean IsRepeating => _isRepeating;
		internal override Boolean IsCounter => true;

		public CounterCoroutine(in CoroutineConfig config)
			: base(config)
		{
			_timeSliceInterval = config.TimeSliceInterval;
			_timeSliceOffset = Math.Max(0, config.TimeSliceOffset);
			_progress.Target = Math.Max(0, config.TargetCount);
			_isRepeating = config.IsRepeating;
		}

		protected override void ResetState() => _progress.Reset();

		protected override void IncrementCount() => _progress.Increment();
		protected override Boolean HasElapsed() => _progress.IsElapsed;

		public override String ToString() => $"{GetType().Name}({Name}, {State}, {_progress.Progress}/{_progress.Target} heartbeats)";
	}
}
