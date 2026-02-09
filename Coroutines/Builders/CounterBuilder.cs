using LunyScript.Blocks;
using LunyScript.Blocks.Coroutines;
using System;

namespace LunyScript.Coroutines.Builders
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
		public CounterDurationBuilder In(Int32 count) => new(_script, _name, count, CoroutineContinuationMode.Finite);

		/// <summary>
		/// Sets the counter to fire repeatedly at the specified interval.
		/// </summary>
		public CounterDurationBuilder Every(Int32 interval) => new(_script, _name, interval, CoroutineContinuationMode.Repeating);
	}

	/// <summary>
	/// Builder step after counter amount is set. Next: specify unit (Frames/Heartbeats).
	/// </summary>
	public readonly struct CounterDurationBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;
		private readonly Int32 _amount;
		private readonly CoroutineContinuationMode _continuation;

		internal CounterDurationBuilder(ILunyScript script, String name, Int32 amount, CoroutineContinuationMode continuation)
		{
			_script = script;
			_name = name;
			_amount = amount;
			_continuation = continuation;
		}

		private CounterFinalBuilder CreateFinal(CoroutineConfig config) => CounterFinalBuilder.FromOptions(_script, config);

		/// <summary>
		/// Duration in frames (count-based).
		/// </summary>
		public CounterFinalBuilder Frames() => CreateFinal(CoroutineConfig.ForCounter(_name, _amount, _continuation));

		/// <summary>
		/// Duration in heartbeats (count-based).
		/// </summary>
		public CounterFinalBuilder Heartbeats() => CreateFinal(CoroutineConfig.ForCounter(_name, _amount, _continuation));
	}

	/// <summary>
	/// Final builder step for counters.
	/// </summary>
	public readonly struct CounterFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly CoroutineConfig _config;

		private CounterFinalBuilder(ILunyScript script, in CoroutineConfig config)
		{
			_script = script;
			_config = config;
		}

		internal static CounterFinalBuilder FromOptions(ILunyScript script, in CoroutineConfig config) => new(script, config);

		/// <summary>
		/// Completes the counter and specifies blocks to run when elapsed.
		/// </summary>
		public IScriptCoroutineCounterBlock Do(params IScriptActionBlock[] blocks)
		{
			var options = _config with { OnElapsed = blocks };
			var scriptInternal = (ILunyScriptInternal)_script;
			var instance = scriptInternal.Context.Coroutines.Register(in options);
			return CoroutineBlock.Create<IScriptCoroutineCounterBlock>(instance);
		}
	}
}
