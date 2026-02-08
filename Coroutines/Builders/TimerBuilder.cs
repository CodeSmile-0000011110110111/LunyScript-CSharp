using LunyScript.Blocks;
using LunyScript.Blocks.Coroutines;
using System;

namespace LunyScript.Coroutines.Builders
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
			_name = !String.IsNullOrEmpty(name) ? name : throw new ArgumentException("Timer name is null or empty", nameof(name));
		}

		/// <summary>
		/// Sets the timer to fire once after the specified duration.
		/// </summary>
		public TimerDurationBuilder In(Double duration) => new(_script, _name, duration, CoroutineContinuationMode.Finite);

		/// <summary>
		/// Sets the timer to fire repeatedly at the specified interval.
		/// </summary>
		public TimerDurationBuilder Every(Double interval) => new(_script, _name, interval, CoroutineContinuationMode.Repeating);
	}

	/// <summary>
	/// Builder step after duration amount is set. Next: specify time unit.
	/// </summary>
	public readonly struct TimerDurationBuilder
	{
		private readonly DurationBuilder<TimerFinalBuilder> _builder;

		internal TimerDurationBuilder(ILunyScript script, String name, Double amount, CoroutineContinuationMode continuation) => _builder =
			new DurationBuilder<TimerFinalBuilder>(name, amount, config => TimerFinalBuilder.FromOptions(script, config), continuation);

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

		/// <summary>
		/// Duration in frames (count-based, counts frames).
		/// </summary>
		public TimerFinalBuilder Frames() => _builder.Frames();
	}

	/// <summary>
	/// Final builder step. Provides terminal methods to complete the timer.
	/// </summary>
	public readonly struct TimerFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly CoroutineConfig _config;

		private TimerFinalBuilder(ILunyScript script, in CoroutineConfig config)
		{
			_script = script;
			_config = config;
		}

		internal static TimerFinalBuilder FromOptions(ILunyScript script, in CoroutineConfig config) => new(script, config);

		/// <summary>
		/// Completes the timer and specifies blocks to run when elapsed.
		/// </summary>
		public IScriptTimerBlock Do(params IScriptActionBlock[] blocks)
		{
			var options = _config with { OnElapsed = blocks };
			var scriptInternal = (ILunyScriptInternal)_script;
			var instance = scriptInternal.Context.Coroutines.Register(in options);
			return new CoroutineBlock(instance);
		}
	}
}
