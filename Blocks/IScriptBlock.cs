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
}
