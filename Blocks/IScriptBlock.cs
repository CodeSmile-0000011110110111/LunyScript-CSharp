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
	/// Interface for executable blocks that perform an action that may alter game state.
	/// </summary>
	public interface IScriptActionBlock : IScriptBlock
	{
		void Execute(IScriptRuntimeContext runtimeContext);
	}

	/// <summary>
	/// Interface for condition blocks that evaluate to a boolean result. These do not alter game state.
	/// </summary>
	public interface IScriptConditionBlock : IScriptBlock
	{
		Boolean Evaluate(IScriptRuntimeContext runtimeContext);
	}

	/// <summary>
	/// Interface for blocks that evaluate to a runtime Variable.
	/// </summary>
	public interface IScriptVariableBlock : IScriptBlock
	{
		Variable GetValue(IScriptRuntimeContext runtimeContext);
	}

	/// <summary>
	/// Container blocks that can be executed by LunyScriptRunner.
	/// Sequences have IDs and can contain child blocks.
	/// </summary>
	public interface IScriptSequenceBlock : IScriptActionBlock
	{
		ScriptBlockID ID { get; }
		IReadOnlyList<IScriptActionBlock> Blocks { get; }
		Boolean IsEmpty { get; }
	}

	/// <summary>
	/// Represents a coroutine block that runs perpetually (indefinitely).
	/// Coroutines can be started, stopped, paused, resumed.
	/// </summary>
	public interface IScriptCoroutineBlock : IScriptActionBlock
	{
		/// <summary>
		/// Starts or restarts the coroutine.
		/// </summary>
		IScriptActionBlock Start();

		/// <summary>
		/// Stops the coroutine and resets its state.
		/// </summary>
		IScriptActionBlock Stop();

		/// <summary>
		/// Pauses the coroutine, preserving current state.
		/// </summary>
		IScriptActionBlock Pause();

		/// <summary>
		/// Resumes a paused coroutine.
		/// </summary>
		IScriptActionBlock Resume();
	}

	/// <summary>
	/// Represents a coroutine timer block. Timers fire after a duration elapses.
	/// </summary>
	public interface IScriptTimerCoroutineBlock : IScriptCoroutineBlock
	{
		/// <summary>
		/// Sets the time scale. Values >= 0; negative values are clamped to 0.
		/// </summary>
		IScriptActionBlock TimeScale(Double scale);
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
	public abstract class ScriptActionBlock : ScriptBlock, IScriptActionBlock
	{
		public abstract void Execute(IScriptRuntimeContext runtimeContext);
	}

	/// <summary>
	/// Abstract base for condition blocks that evaluate to a boolean result.
	/// </summary>
	public abstract class ScriptConditionBlock : ScriptBlock, IScriptConditionBlock
	{
		public abstract Boolean Evaluate(IScriptRuntimeContext runtimeContext);
	}

	/// <summary>
	/// Abstract base for variable blocks that evaluate to a runtime Variable.
	/// Extends ScriptConditionBlock because variables are implicitly usable as conditions
	/// (via AsBoolean conversion).
	/// </summary>
	public abstract class ScriptVariableBlock : ScriptConditionBlock, IScriptVariableBlock
	{
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			GetValue(runtimeContext).AsBoolean();

		public abstract Variable GetValue(IScriptRuntimeContext runtimeContext);
	}

	/// <summary>
	/// Abstract base for sequence blocks that contain child action blocks.
	/// </summary>
	public abstract class ScriptSequenceBlock : ScriptActionBlock, IScriptSequenceBlock
	{
		public abstract ScriptBlockID ID { get; }
		public abstract IReadOnlyList<IScriptActionBlock> Blocks { get; }
		public abstract Boolean IsEmpty { get; }
	}

	/// <summary>
	/// Abstract base for coroutine blocks.
	/// </summary>
	public abstract class ScriptCoroutineBlock : ScriptActionBlock, IScriptCoroutineBlock
	{
		public abstract IScriptActionBlock Start();
		public abstract IScriptActionBlock Stop();
		public abstract IScriptActionBlock Pause();
		public abstract IScriptActionBlock Resume();
	}
}
