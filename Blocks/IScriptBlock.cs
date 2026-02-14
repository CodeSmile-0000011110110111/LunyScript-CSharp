using Luny;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Base interface for LunyScript blocks.
	/// </summary>
	public interface IScriptBlock {}




	/// <summary>
	/// Represents a coroutine block that runs perpetually (indefinitely).
	/// Coroutines can be started, stopped, paused, resumed.
	/// </summary>
	public interface IScriptCoroutineBlock : IScriptBlock
	{
		/// <summary>
		/// Starts or restarts the coroutine.
		/// </summary>
		ScriptActionBlock Start();

		/// <summary>
		/// Stops the coroutine and resets its state.
		/// </summary>
		ScriptActionBlock Stop();

		/// <summary>
		/// Pauses the coroutine, preserving current state.
		/// </summary>
		ScriptActionBlock Pause();

		/// <summary>
		/// Resumes a paused coroutine.
		/// </summary>
		ScriptActionBlock Resume();
	}

	/// <summary>
	/// Represents a coroutine timer block. Timers fire after a duration elapses.
	/// </summary>
	public interface IScriptTimerCoroutineBlock : IScriptCoroutineBlock
	{
		/// <summary>
		/// Sets the time scale. Values >= 0; negative values are clamped to 0.
		/// </summary>
		ScriptActionBlock TimeScale(Double scale);
	}

	/// <summary>
	/// Represents a coroutine counter block. Counters elapse after a specific number of frames/heartbeats have passed.
	/// </summary>
	public interface IScriptCounterCoroutineBlock : IScriptCoroutineBlock {}

	// ── Abstract Base Classes ──────────────────────────────────────────
	// These enable implicit conversions (C# disallows implicit conversion to interfaces)
	// and provide a shared foundation for all block implementations.

	/// <summary>
	/// Abstract base for all LunyScript blocks.
	/// </summary>
	public abstract class ScriptBlock : IScriptBlock {}

	/// <summary>
	/// Abstract base for action blocks that perform an action that may alter game state.
	/// </summary>
	public abstract class ScriptActionBlock : ScriptBlock
	{
		public abstract void Execute(IScriptRuntimeContext runtimeContext);
	}

	/// <summary>
	/// Abstract base for condition blocks that evaluate to a boolean result.
	/// </summary>
	public abstract class ScriptConditionBlock : ScriptBlock
	{
		public abstract Boolean Evaluate(IScriptRuntimeContext runtimeContext);
	}


	/// <summary>
	/// Abstract base for sequence blocks that contain child action blocks.
	/// </summary>
	public abstract class ScriptSequenceBlock : ScriptActionBlock
	{
		public abstract ScriptBlockID ID { get; }
		public abstract IReadOnlyList<ScriptActionBlock> Blocks { get; }
		public abstract Boolean IsEmpty { get; }
	}

	/// <summary>
	/// Abstract base for coroutine blocks.
	/// </summary>
	public abstract class ScriptCoroutineBlock : ScriptActionBlock, IScriptCoroutineBlock
	{
		public abstract ScriptActionBlock Start();
		public abstract ScriptActionBlock Stop();
		public abstract ScriptActionBlock Pause();
		public abstract ScriptActionBlock Resume();
	}
}
