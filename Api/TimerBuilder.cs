using LunyScript.Blocks;
using LunyScript.Coroutines;
using LunyScript.Execution;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Entry point for the Timer fluent builder chain.
	/// Usage: Timer("name").In(3).Seconds().Do(blocks);
	/// </summary>
	public readonly struct TimerBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;

		internal TimerBuilder(ILunyScript script, String name)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrEmpty(name) ? name : throw new ArgumentException("Timer name cannot be null or empty", nameof(name));
		}

		/// <summary>
		/// Sets the timer to fire once after the specified duration.
		/// </summary>
		public TimerDurationBuilder In(Int32 amount) => new(_script, _name, amount, isRepeating: false);

		/// <summary>
		/// Sets the timer to fire repeatedly at the specified interval.
		/// </summary>
		public TimerDurationBuilder Every(Int32 amount) => new(_script, _name, amount, isRepeating: true);
	}

	/// <summary>
	/// Builder step after duration amount is set. Next: specify time unit.
	/// </summary>
	public readonly struct TimerDurationBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;
		private readonly Int32 _amount;
		private readonly Boolean _isRepeating;

		internal TimerDurationBuilder(ILunyScript script, String name, Int32 amount, Boolean isRepeating)
		{
			_script = script;
			_name = name;
			_amount = amount;
			_isRepeating = isRepeating;
		}

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public TimerFinalBuilder Seconds() => TimerFinalBuilder.TimeBased(_script, _name, _amount, _isRepeating);

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public TimerFinalBuilder Milliseconds() => TimerFinalBuilder.TimeBased(_script, _name, _amount / 1000.0, _isRepeating);

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public TimerFinalBuilder Minutes() => TimerFinalBuilder.TimeBased(_script, _name, _amount * 60.0, _isRepeating);

		/// <summary>
		/// Duration in heartbeats (count-based, counts fixed steps).
		/// </summary>
		public TimerFinalBuilder Heartbeats() => TimerFinalBuilder.HeartbeatBased(_script, _name, _amount, _isRepeating);
	}

	/// <summary>
	/// Final builder step. Provides terminal methods to complete the timer.
	/// </summary>
	public readonly struct TimerFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;
		private readonly Double _durationSeconds;
		private readonly Int32 _heartbeatCount;
		private readonly Boolean _isCountBased;
		private readonly Boolean _isRepeating;

		private TimerFinalBuilder(ILunyScript script, String name, Double durationSeconds, Int32 heartbeatCount, Boolean isCountBased, Boolean isRepeating)
		{
			_script = script;
			_name = name;
			_durationSeconds = durationSeconds;
			_heartbeatCount = heartbeatCount;
			_isCountBased = isCountBased;
			_isRepeating = isRepeating;
		}

		internal static TimerFinalBuilder TimeBased(ILunyScript script, String name, Double durationSeconds, Boolean isRepeating) =>
			new(script, name, durationSeconds, 0, isCountBased: false, isRepeating);

		internal static TimerFinalBuilder HeartbeatBased(ILunyScript script, String name, Int32 heartbeatCount, Boolean isRepeating) =>
			new(script, name, 0, heartbeatCount, isCountBased: true, isRepeating);

		/// <summary>
		/// Completes the timer and specifies blocks to run when elapsed.
		/// </summary>
		public IScriptTimerBlock Do(params IScriptActionBlock[] blocks)
		{
			var scriptInternal = (ILunyScriptInternal)_script;
			var context = scriptInternal.Context;

			if (context == null)
				return null;

			var instance = context.Coroutines.Register(_name);

			// Set duration based on type
			if (_isCountBased)
				instance.SetHeartbeatCount(_heartbeatCount);
			else
				instance.SetDuration(_durationSeconds);

			instance.SetOnElapsedBlocks(blocks);

			// For repeating timers, we need to handle restart logic in the runner
			// For now, we mark the instance somehow (future enhancement)

			return new CoroutineBlock(instance);
		}
	}
}
