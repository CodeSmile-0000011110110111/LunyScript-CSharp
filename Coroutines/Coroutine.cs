using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Represents the execution state of a coroutine or timer.
	/// </summary>
	internal enum CoroutineState
	{
		/// <summary>
		/// Coroutine is not running and has no accumulated time.
		/// </summary>
		Stopped,

		/// <summary>
		/// Coroutine is actively running and accumulating time.
		/// </summary>
		Running,

		/// <summary>
		/// Coroutine is frozen at current time, will resume when unpaused.
		/// </summary>
		Paused,
	}

	/// <summary>
	/// Configuration options for creating a coroutine.
	/// </summary>
	internal record CoroutineOptions
	{
		public String Name { get; init; }
		public Double Duration { get; init; }      // Used by Time-based
		public Int32 TargetCount { get; init; }    // Used by Count-based
		public Int32 TimeSliceInterval { get; init; }
		public Int32 TimeSliceOffset { get; init; }
		public Boolean IsRepeating { get; init; }
		public Boolean IsTimer { get; init; }

		// Handlers
		public IScriptActionBlock[] OnUpdate { get; init; }
		public IScriptActionBlock[] OnHeartbeat { get; init; }
		public IScriptActionBlock[] OnElapsed { get; init; }
		public IScriptActionBlock[] OnStarted { get; init; }
		public IScriptActionBlock[] OnStopped { get; init; }
		public IScriptActionBlock[] OnPaused { get; init; }
		public IScriptActionBlock[] OnResumed { get; init; }

		public static CoroutineOptions ForTimer(String name, Double duration, Boolean isRepeating, IScriptActionBlock[] onElapsed) =>
			new() { Name = name, Duration = duration, IsRepeating = isRepeating, OnElapsed = onElapsed, IsTimer = true };

		public static CoroutineOptions ForCountTimer(String name, Int32 targetCount, Boolean isRepeating, IScriptActionBlock[] onElapsed) =>
			new() { Name = name, TargetCount = targetCount, IsRepeating = isRepeating, OnElapsed = onElapsed, IsTimer = true };

		public static CoroutineOptions ForEvery(String name, Int32 interval, Int32 offset, IScriptActionBlock[] blocks, Boolean isHeartbeat) =>
			new()
			{
				Name = name,
				TimeSliceInterval = interval,
				TimeSliceOffset = offset,
				OnUpdate = !isHeartbeat ? blocks : null,
				OnHeartbeat = isHeartbeat ? blocks : null,
				IsTimer = false
			};

		public static CoroutineOptions ForCoroutine(String name) => new() { Name = name, IsTimer = false };

		private static Int32 s_everyCounter;
		public static String GenerateEveryName(Int32 interval, Boolean isHeartbeat, Int32 offset) =>
			$"__every_{interval}_{(isHeartbeat ? "hb" : "fr")}_{offset}_{System.Threading.Interlocked.Increment(ref s_everyCounter)}";
	}

	/// <summary>
	/// Base class for coroutines and timers with runtime state and control methods.
	/// </summary>
	internal abstract class CoroutineBase
	{
		private readonly String _name;
		protected CoroutineState _state = CoroutineState.Stopped;
		private Boolean _wasPausedByDisable;

		protected readonly IScriptSequenceBlock _onElapsedSequence;

		internal String Name => _name;
		internal CoroutineState State => _state;
		internal Boolean WasPausedByDisable => _wasPausedByDisable;

		internal virtual IScriptSequenceBlock OnUpdateSequence => null;
		internal virtual IScriptSequenceBlock OnHeartbeatSequence => null;
		internal virtual IScriptSequenceBlock OnElapsedSequence => _onElapsedSequence;
		internal virtual IScriptSequenceBlock OnStartedSequence => null;
		internal virtual IScriptSequenceBlock OnStoppedSequence => null;
		internal virtual IScriptSequenceBlock OnPausedSequence => null;
		internal virtual IScriptSequenceBlock OnResumedSequence => null;

		internal virtual Boolean IsCountBased => false;
		internal virtual Boolean IsTimeSliced => false;
		internal virtual Int32 TimeSliceInterval => 0;
		internal virtual Int32 TimeSliceOffset => 0;
		internal virtual Double TimeScale => 1.0;

		/// <summary>
		/// Factory method to create specialized coroutine instances.
		/// </summary>
		public static CoroutineBase Create(in CoroutineOptions options)
		{
			if (options.IsTimer)
			{
				if (options.TargetCount > 0)
					return new CountTimer(options);

				return new TimeTimer(options);
			}

			if (options.TargetCount > 0 || options.TimeSliceInterval != 0)
				return new CountCoroutine(options);

			if (options.Duration > 0)
				return new TimeCoroutine(options);

			return new Coroutine(options);
		}

		protected CoroutineBase(in CoroutineOptions options)
		{
			if (String.IsNullOrEmpty(options.Name))
				throw new ArgumentException("Coroutine name cannot be null or empty", nameof(options.Name));

			_name = options.Name;
			_onElapsedSequence = CreateSequenceIfNotEmpty(options.OnElapsed);
		}

		protected static IScriptSequenceBlock CreateSequenceIfNotEmpty(IScriptActionBlock[] blocks) =>
			blocks != null && blocks.Length > 0 ? new SequenceBlock(blocks) : null;

		/// <summary>
		/// Starts or restarts the coroutine. Resets elapsed time/count.
		/// Returns true if started fresh (not restarting), indicating Started event should fire.
		/// </summary>
		internal virtual Boolean Start()
		{
			Stop();
			_state = CoroutineState.Running;
			return true;
		}

		/// <summary>
		/// Stops the coroutine and resets state.
		/// Returns true if the coroutine was running or paused (indicating Stopped event should fire).
		/// </summary>
		internal virtual Boolean Stop()
		{
			if (_state == CoroutineState.Stopped)
				return false;

			_state = CoroutineState.Stopped;
			_wasPausedByDisable = false;
			ResetState();
			return true;
		}

		protected virtual void ResetState() {}

		/// <summary>
		/// Pauses the coroutine, preserving current elapsed time.
		/// Returns true if the coroutine was running (indicating Paused event should fire).
		/// </summary>
		internal Boolean Pause()
		{
			if (_state != CoroutineState.Running)
				return false;

			_state = CoroutineState.Paused;
			return true;
		}

		/// <summary>
		/// Resumes a paused coroutine.
		/// Returns true if the coroutine was paused (indicating Resumed event should fire).
		/// </summary>
		internal Boolean Resume()
		{
			if (_state != CoroutineState.Paused)
				return false;

			_state = CoroutineState.Running;
			_wasPausedByDisable = false;
			return true;
		}

		/// <summary>
		/// Sets the time scale. Clamped to >= 0 (no negative time).
		/// </summary>
		internal virtual void SetTimeScale(Double scale) {}

		/// <summary>
		/// Auto-pause when object is disabled.
		/// </summary>
		internal void PauseByDisable()
		{
			if (_state != CoroutineState.Running)
				return;

			_state = CoroutineState.Paused;
			_wasPausedByDisable = true;
		}

		/// <summary>
		/// Auto-resume when object is re-enabled (only if was paused by disable).
		/// </summary>
		internal void ResumeByEnable()
		{
			if (_state != CoroutineState.Paused || !_wasPausedByDisable)
				return;

			_state = CoroutineState.Running;
			_wasPausedByDisable = false;
		}

		/// <summary>
		/// Advances a time-based coroutine by deltaTime. Returns true if elapsed (duration reached).
		/// </summary>
		internal virtual Boolean AdvanceTime(Double deltaTime) => false;

		/// <summary>
		/// Advances a count-based coroutine by one heartbeat. Returns true if elapsed (count reached).
		/// </summary>
		internal virtual Boolean AdvanceHeartbeat() => false;

		public abstract override String ToString();
	}

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

		public Coroutine(in CoroutineOptions options) : base(options)
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
	internal sealed class TimeCoroutine : Coroutine
	{
		private Double _elapsedTime;
		private Double _timeScale = 1.0;
		private readonly Double _duration;
		private readonly Boolean _isRepeating;

		public TimeCoroutine(in CoroutineOptions options) : base(options)
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

	/// <summary>
	/// Coroutine that elapses after a specific number of heartbeats/ticks.
	/// </summary>
	internal sealed class CountCoroutine : Coroutine
	{
		private Int32 _elapsedCount;
		private readonly Int32 _targetCount;
		private readonly Int32 _timeSliceInterval;
		private readonly Int32 _timeSliceOffset;
		private readonly Boolean _isRepeating;

		public CountCoroutine(in CoroutineOptions options) : base(options)
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

	/// <summary>
	/// Base class for timers which only run Do() blocks when elapsed.
	/// </summary>
	internal abstract class TimerBase : CoroutineBase
	{
		protected TimerBase(in CoroutineOptions options) : base(options) {}
	}

	/// <summary>
	/// Timer that elapses after a specific duration in seconds.
	/// </summary>
	internal sealed class TimeTimer : TimerBase
	{
		private Double _elapsedTime;
		private Double _timeScale = 1.0;
		private readonly Double _duration;
		private readonly Boolean _isRepeating;

		public TimeTimer(in CoroutineOptions options) : base(options)
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
	internal sealed class CountTimer : TimerBase
	{
		private Int32 _elapsedCount;
		private readonly Int32 _targetCount;
		private readonly Boolean _isRepeating;

		public CountTimer(in CoroutineOptions options) : base(options)
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
