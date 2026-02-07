using LunyScript.Blocks;
using LunyScript.Blocks.Coroutines;
using System;

namespace LunyScript.Coroutines.Builders
{
	/// <summary>
	/// Entry point for the Every fluent builder chain.
	/// Usage: Every(3).Frames().Do(blocks); or Every(Even).Heartbeats().DelayBy(1).Do(blocks);
	/// </summary>
	public readonly struct EveryBuilder
	{
		private readonly ILunyScript _script;
		private readonly Int32 _interval;

		internal EveryBuilder(ILunyScript script, Int32 interval)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_interval = interval;
		}

		/// <summary>
		/// Selects frame-based execution.
		/// </summary>
		public EveryUnitBuilder Frames() => new(_script, _interval, CoroutineCountMode.Frames);

		/// <summary>
		/// Selects heartbeat-based execution.
		/// </summary>
		public EveryUnitBuilder Heartbeats() => new(_script, _interval, CoroutineCountMode.Heartbeats);
	}

	/// <summary>
	/// Builder step after unit (Frames/Heartbeats) is selected.
	/// </summary>
	public readonly struct EveryUnitBuilder
	{
		private readonly ILunyScript _script;
		private readonly Int32 _interval;
		private readonly CoroutineCountMode _countMode;
		private readonly Int32 _delay;

		internal EveryUnitBuilder(ILunyScript script, Int32 interval, CoroutineCountMode countMode, Int32 delay = 0)
		{
			_script = script;
			_interval = interval;
			_countMode = countMode;
			_delay = delay;
		}

		/// <summary>
		/// Sets the phase offset (delay) for time-sliced execution.
		/// </summary>
		public EveryUnitBuilder DelayBy(Int32 delay)
		{
			if (_delay != 0)
				throw new ArgumentException($"{nameof(DelayBy)}() can't be used twice");

			return new EveryUnitBuilder((ILunyScript)_script, _interval, _countMode, delay);
		}

		/// <summary>
		/// Completes the builder and specifies blocks to run.
		/// </summary>
		public IScriptCoroutineBlock Do(params IScriptActionBlock[] blocks)
		{
			// name = null => generates a unique name for this time-sliced coroutine
			var options = CoroutineConfig.ForEvery(null, _interval, _countMode, _delay, blocks);
			var scriptInternal = (ILunyScriptInternal)_script;
			var instance = scriptInternal.Context.Coroutines.Register(in options);

			// Auto-start time-sliced coroutines
			var block = new CoroutineBlock(instance);
			instance.Start();

			return block;
		}
	}
}
