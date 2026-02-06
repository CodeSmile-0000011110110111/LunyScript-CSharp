using LunyScript.Blocks;
using LunyScript.Coroutines;
using LunyScript.Execution;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Entry point for the Every fluent builder chain.
	/// Usage: Every(3).Frames(blocks); or Every(Even).Heartbeats(blocks);
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
		/// Sets the phase offset (delay) for time-sliced execution.
		/// </summary>
		public EveryDelayedBuilder DelayBy(Int32 offset) => new(_script, _interval, offset);

		/// <summary>
		/// Runs blocks every N frames.
		/// </summary>
		public IScriptCoroutineBlock Frames(params IScriptActionBlock[] blocks) =>
			CreateTimeSlicedCoroutine(blocks, isHeartbeat: false, delayOffset: 0);

		/// <summary>
		/// Runs blocks every N heartbeats (fixed steps).
		/// </summary>
		public IScriptCoroutineBlock Heartbeats(params IScriptActionBlock[] blocks) =>
			CreateTimeSlicedCoroutine(blocks, isHeartbeat: true, delayOffset: 0);

		internal IScriptCoroutineBlock CreateTimeSlicedCoroutine(IScriptActionBlock[] blocks, Boolean isHeartbeat, Int32 delayOffset)
		{
			var scriptInternal = (ILunyScriptInternal)_script;
			var context = scriptInternal.Context;

			if (context == null)
				return null;

			// Generate a unique name for this time-sliced coroutine
			var name = $"__every_{_interval}_{(isHeartbeat ? "hb" : "fr")}_{delayOffset}_{Guid.NewGuid():N}";
			var instance = context.Coroutines.Register(name);

			// Configure as time-sliced coroutine
			instance.SetTimeSliceInterval(_interval);
			instance.SetTimeSliceOffset(delayOffset);

			if (isHeartbeat)
				instance.SetOnHeartbeatBlocks(blocks);
			else
				instance.SetOnUpdateBlocks(blocks);

			// Auto-start time-sliced coroutines
			var block = new CoroutineBlock(instance);
			instance.Start();

			return block;
		}
	}

	/// <summary>
	/// Builder step after DelayBy is set.
	/// </summary>
	public readonly struct EveryDelayedBuilder
	{
		private readonly ILunyScript _script;
		private readonly Int32 _interval;
		private readonly Int32 _offset;

		internal EveryDelayedBuilder(ILunyScript script, Int32 interval, Int32 offset)
		{
			_script = script;
			_interval = interval;
			_offset = offset;
		}

		/// <summary>
		/// Runs blocks every N frames with phase offset.
		/// </summary>
		public IScriptCoroutineBlock Frames(params IScriptActionBlock[] blocks)
		{
			var builder = new EveryBuilder(_script, _interval);
			return builder.CreateTimeSlicedCoroutine(blocks, isHeartbeat: false, _offset);
		}

		/// <summary>
		/// Runs blocks every N heartbeats (fixed steps) with phase offset.
		/// </summary>
		public IScriptCoroutineBlock Heartbeats(params IScriptActionBlock[] blocks)
		{
			var builder = new EveryBuilder(_script, _interval);
			return builder.CreateTimeSlicedCoroutine(blocks, isHeartbeat: true, _offset);
		}
	}
}
