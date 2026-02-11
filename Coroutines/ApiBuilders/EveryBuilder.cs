using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines.ApiBuilders
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
		public EveryUnitBuilder Frames() => new(_script, _interval, Coroutine.Process.FrameUpdate);

		/// <summary>
		/// Selects heartbeat-based execution.
		/// </summary>
		public EveryUnitBuilder Heartbeats() => new(_script, _interval, Coroutine.Process.Heartbeat);
	}

	/// <summary>
	/// Builder step after unit (Frames/Heartbeats) is selected.
	/// </summary>
	public readonly struct EveryUnitBuilder
	{
		private readonly ILunyScript _script;
		private readonly Int32 _interval;
		private readonly Int32 _delay;
		private readonly Coroutine.Process _process;

		internal EveryUnitBuilder(ILunyScript script, Int32 interval, Coroutine.Process process, Int32 delay = 0)
		{
			_script = script;
			_interval = Math.Max(0, interval);
			_delay = delay;
			_process = process;

			if (interval < 0)
				throw new ArgumentException($"Every duration must be 0 or greater, got: {interval}");
		}

		/// <summary>
		/// Sets the phase offset (delay) for time-sliced execution.
		/// </summary>
		public EveryUnitBuilder DelayBy(Int32 delay)
		{
			if (_delay != 0)
				throw new ArgumentException($"{nameof(DelayBy)}() can't be used twice");

			return new EveryUnitBuilder(_script, _interval, _process, delay);
		}

		/// <summary>
		/// Completes the builder and specifies blocks to run.
		/// </summary>
		public IScriptCounterCoroutineBlock Do(params IScriptActionBlock[] blocks)
		{
			// name = null => generates a unique name for a time-sliced coroutine
			var options = Coroutine.Options.ForEveryInterval(null, _interval, _delay, _process, blocks);
			var scriptInternal = (ILunyScriptInternal)_script;
			var coroutineBlock = scriptInternal.RuntimeContext.Coroutines.Register(_script, in options);
			return (IScriptCounterCoroutineBlock)coroutineBlock;
		}
	}
}
