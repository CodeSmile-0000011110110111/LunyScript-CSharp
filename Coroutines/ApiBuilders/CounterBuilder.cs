using LunyScript.Blocks;
using LunyScript.Blocks.Coroutines;
using System;

namespace LunyScript.Coroutines.ApiBuilders
{
	/// <summary>
	/// Entry point for the Counter fluent builder chain.
	/// Usage: Counter("name").In(5).Frames().Do(blocks);
	/// </summary>
	public readonly struct CounterBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;

		internal CounterBuilder(ILunyScript script, String name)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrEmpty(name) ? name : throw new ArgumentException("Counter name is null or empty", nameof(name));
		}

		/// <summary>
		/// Sets the counter to fire once after the specified count.
		/// </summary>
		public CounterDurationBuilder In(Int32 count) => new(_script, _name, count, Coroutine.Continuation.Finite);

		/// <summary>
		/// Sets the counter to fire repeatedly at the specified interval.
		/// </summary>
		public CounterDurationBuilder Every(Int32 interval) => new(_script, _name, interval, Coroutine.Continuation.Repeating);
	}

	/// <summary>
	/// Builder step after counter amount is set. Next: specify unit (Frames/Heartbeats).
	/// </summary>
	public readonly struct CounterDurationBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;
		private readonly Int32 _amount;
		private readonly Coroutine.Continuation _continuation;

		internal CounterDurationBuilder(ILunyScript script, String name, Int32 amount, Coroutine.Continuation continuation)
		{
			_script = script;
			_name = name;
			_amount = amount;
			_continuation = continuation;
		}

		private CounterFinalBuilder CreateFinal(Coroutine.Options options) => CounterFinalBuilder.FromOptions(_script, options);

		/// <summary>
		/// Duration in frames (count-based).
		/// </summary>
		public CounterFinalBuilder Frames() => CreateFinal(Coroutine.Options.ForCounter(_name, _amount, _continuation, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in heartbeats (count-based).
		/// </summary>
		public CounterFinalBuilder Heartbeats() => CreateFinal(Coroutine.Options.ForCounter(_name, _amount, _continuation, Coroutine.Process.Heartbeat));
	}

	/// <summary>
	/// Final builder step for counters.
	/// </summary>
	public readonly struct CounterFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly Coroutine.Options _options;

		private CounterFinalBuilder(ILunyScript script, in Coroutine.Options options)
		{
			_script = script;
			_options = options;
		}

		internal static CounterFinalBuilder FromOptions(ILunyScript script, in Coroutine.Options options) => new(script, options);

		/// <summary>
		/// Completes the counter and specifies blocks to run when elapsed.
		/// </summary>
		public IScriptCoroutineCounterBlock Do(params IScriptActionBlock[] blocks)
		{
			var options = _options with { OnElapsed = blocks };
			var scriptInternal = (ILunyScriptInternal)_script;
			return scriptInternal.RuntimeContext.Coroutines.Register<IScriptCoroutineCounterBlock>(in options);
		}
	}
}
