using LunyScript.Blocks;
using LunyScript.Blocks.Coroutines;
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
		private readonly TimeUnitBuilder<TimerFinalBuilder> _builder;

		internal TimerDurationBuilder(ILunyScript script, String name, Int32 amount, Boolean isRepeating)
		{
			_builder = new TimeUnitBuilder<TimerFinalBuilder>(script, name, amount, isRepeating, true,
				options => TimerFinalBuilder.FromOptions(script, options));
		}

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public TimerFinalBuilder Seconds() => _builder.Seconds();

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public TimerFinalBuilder Milliseconds() => _builder.Milliseconds();

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public TimerFinalBuilder Minutes() => _builder.Minutes();

		/// <summary>
		/// Duration in heartbeats (count-based, counts fixed steps).
		/// </summary>
		public TimerFinalBuilder Heartbeats() => _builder.Heartbeats();
	}

	/// <summary>
	/// Final builder step. Provides terminal methods to complete the timer.
	/// </summary>
	public readonly struct TimerFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly CoroutineOptions _options;

		private TimerFinalBuilder(ILunyScript script, in CoroutineOptions options)
		{
			_script = script;
			_options = options;
		}

		internal static TimerFinalBuilder FromOptions(ILunyScript script, in CoroutineOptions options) => new(script, options);

		internal static TimerFinalBuilder TimeBased(ILunyScript script, String name, Double durationSeconds, Boolean isRepeating) =>
			new(script, CoroutineOptions.ForTimer(name, durationSeconds, isRepeating, null));

		internal static TimerFinalBuilder HeartbeatBased(ILunyScript script, String name, Int32 heartbeatCount, Boolean isRepeating) =>
			new(script, CoroutineOptions.ForCountTimer(name, heartbeatCount, isRepeating, null));

		/// <summary>
		/// Completes the timer and specifies blocks to run when elapsed.
		/// </summary>
		public IScriptTimerBlock Do(params IScriptActionBlock[] blocks)
		{
			var scriptInternal = (ILunyScriptInternal)_script;
			var context = scriptInternal.Context;

			if (context == null)
				return null;

			var options = _options with { OnElapsed = blocks };
			var instance = context.Coroutines.Register(in options);
			return new CoroutineBlock(instance);
		}
	}
}
