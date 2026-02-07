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
			_onUpdateSequence = CreateSequenceIfNotEmpty(options.OnUpdate);
			_onHeartbeatSequence = CreateSequenceIfNotEmpty(options.OnHeartbeat);
			_onStartedSequence = CreateSequenceIfNotEmpty(options.OnStarted);
			_onStoppedSequence = CreateSequenceIfNotEmpty(options.OnStopped);
			_onPausedSequence = CreateSequenceIfNotEmpty(options.OnPaused);
			_onResumedSequence = CreateSequenceIfNotEmpty(options.OnResumed);
		}

		public override String ToString() => $"Coroutine({Name}, {State}, Infinite)";
	}

	/// <summary>
	/// Coroutine that elapses after a specific duration in seconds.
	/// </summary>
	internal sealed class TimeBasedCoroutine : Coroutine
	{
		private readonly Boolean _isRepeating;
		private TimeProgress _progress;

		internal override Double TimeScale => _progress.Scale;
		protected override Boolean IsRepeating => _isRepeating;

		public TimeBasedCoroutine(in CoroutineOptions options)
			: base(options)
		{
			_progress.Duration = Math.Max(0, options.Duration);
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
	internal sealed class CountBasedCoroutine : Coroutine
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

		public CountBasedCoroutine(in CoroutineOptions options)
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

	/*
/// <summary>
/// Base class for timers which only run Do() blocks when elapsed.
/// </summary>
internal abstract class TimerCoroutineBase : CoroutineBase
{
	protected TimerCoroutineBase(in CoroutineOptions options)
		: base(options) {}
}
*/

	/// <summary>
	/// Timer that elapses after a specific duration in seconds.
	/// </summary>
	internal sealed class TimerCoroutine : CoroutineBase
	{
		private readonly Boolean _isRepeating;
		private TimeProgress _progress;

		internal override Double TimeScale => _progress.Scale;
		protected override Boolean IsRepeating => _isRepeating;

		public TimerCoroutine(in CoroutineOptions options)
			: base(options)
		{
			_progress.Duration = Math.Max(0, options.Duration);
			_progress.Scale = 1.0;
			_isRepeating = options.IsRepeating;
		}

		internal override void SetTimeScale(Double scale) => _progress.Scale = Math.Max(0, scale);

		protected override void ResetState() => _progress.Reset();

		protected override void AccumulateTime(Double deltaTime) => _progress.Step(deltaTime);
		protected override Boolean HasElapsed() => _progress.IsElapsed;

		public override String ToString() => $"Timer({Name}, {State}, {_progress.Elapsed:F2}/{_progress.Duration:F2}s)";
	}

	/// <summary>
	/// Timer that elapses after a specific number of heartbeats/frames.
	/// </summary>
	internal sealed class CounterCoroutine : CoroutineBase
	{
		private readonly Boolean _isRepeating;
		private CountProgress _progress;

		internal override Boolean IsCountBased => true;
		protected override Boolean IsRepeating => _isRepeating;

		public CounterCoroutine(in CoroutineOptions options)
			: base(options)
		{
			_progress.Target = Math.Max(0, options.TargetCount);
			_isRepeating = options.IsRepeating;
		}

		protected override void ResetState() => _progress.Reset();

		protected override void AccumulateHeartbeat() => _progress.Step();
		protected override Boolean HasElapsed() => _progress.IsElapsed;

		public override String ToString() => $"Timer({Name}, {State}, {_progress.Elapsed}/{_progress.Target} heartbeats)";
	}
}
