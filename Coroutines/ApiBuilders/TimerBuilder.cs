using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines.ApiBuilders
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
			_name = !String.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Timer name is null or empty", nameof(name));
		}

		/// <summary>
		/// Sets the timer to fire once after the specified duration.
		/// </summary>
		public TimerDurationBuilder In(Double duration) => new(_script, _name, duration, Coroutine.Continuation.Finite);

		/// <summary>
		/// Sets the timer to fire repeatedly at the specified interval.
		/// </summary>
		public TimerDurationBuilder Every(Double interval) => new(_script, _name, interval, Coroutine.Continuation.Repeating);
	}

	/// <summary>
	/// Builder step after duration amount is set. Next: specify time unit.
	/// </summary>
	public readonly struct TimerDurationBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;
		private readonly Double _amount;
		private readonly Coroutine.Continuation _continuation;

		internal TimerDurationBuilder(ILunyScript script, String name, Double amount, Coroutine.Continuation continuation)
		{
			_script = script;
			_name = name;
			_amount = Math.Max(0, amount);
			_continuation = continuation;

			if (amount < 0)
				throw new ArgumentException($"Timer duration must be 0 or greater, got: {amount}");
		}

		private TimerFinalBuilder CreateFinal(in Coroutine.Options options) => TimerFinalBuilder.FromOptions(_script, options);

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public TimerFinalBuilder Seconds() =>
			CreateFinal(Coroutine.Options.ForTimer(_name, _amount, _continuation, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public TimerFinalBuilder Milliseconds() =>
			CreateFinal(Coroutine.Options.ForTimer(_name, _amount / 1000.0, _continuation, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public TimerFinalBuilder Minutes() =>
			CreateFinal(Coroutine.Options.ForTimer(_name, _amount * 60.0, _continuation, Coroutine.Process.FrameUpdate));
	}

	/// <summary>
	/// Final builder step. Provides terminal methods to complete the timer.
	/// </summary>
	public readonly struct TimerFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly Coroutine.Options _options;

		private TimerFinalBuilder(ILunyScript script, in Coroutine.Options options)
		{
			_script = script;
			_options = options;
		}

		internal static TimerFinalBuilder FromOptions(ILunyScript script, in Coroutine.Options options) => new(script, options);

		/// <summary>
		/// Completes the timer and specifies blocks to run when elapsed.
		/// </summary>
		public IScriptTimerCoroutineBlock Do(params IScriptActionBlock[] blocks)
		{
			var options = _options with { OnElapsed = blocks };
			var scriptInternal = (ILunyScriptInternal)_script;
			var coroutineBlock = scriptInternal.RuntimeContext.Coroutines.Register(_script, in options);
			return (IScriptTimerCoroutineBlock)coroutineBlock;
		}
	}
}
