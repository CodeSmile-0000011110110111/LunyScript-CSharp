using LunyScript.Blocks;
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
			_name = !String.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Counter name is null or empty", nameof(name));
		}

		/// <summary>
		/// Sets the counter to fire once after the specified count.
		/// </summary>
		public CounterDurationBuilder In(Int32 targetCount) => new(_script, _name, targetCount, Coroutine.Continuation.Finite);

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

			if (amount < 0)
				throw new ArgumentException($"Counter duration must be 0 or greater, got: {amount}");
		}

		/// <summary>
		/// Duration in frames (count-based).
		/// </summary>
		public CounterFinalBuilder Frames() =>
			new(_script, Coroutine.Options.ForCounter(_name, _amount, _continuation, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in heartbeats (count-based).
		/// </summary>
		public CounterFinalBuilder Heartbeats() =>
			new(_script, Coroutine.Options.ForCounter(_name, _amount, _continuation, Coroutine.Process.Heartbeat));
	}

	/// <summary>
	/// Final builder step for counters.
	/// </summary>
	public readonly struct CounterFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly Coroutine.Options _options;

		internal CounterFinalBuilder(ILunyScript script, in Coroutine.Options options)
		{
			_script = script;
			_options = options;
		}

		/// <summary>
		/// Completes the counter and specifies blocks to run when elapsed.
		/// </summary>
		public IScriptCounterCoroutineBlock Do(params IScriptActionBlock[] blocks)
		{
			var options = _options with { OnElapsed = blocks };
			var scriptInternal = (ILunyScriptInternal)_script;
			var coroutineBlock = scriptInternal.RuntimeContext.Coroutines.Register(_script, in options);
			return (IScriptCounterCoroutineBlock)coroutineBlock;
		}
	}
}
