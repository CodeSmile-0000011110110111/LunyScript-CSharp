using Luny;
using LunyScript.Execution;
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
		void Execute(ILunyScriptContext context);
	}

	/// <summary>
	/// Interface for condition blocks that evaluate to a boolean result. These do not alter game state.
	/// </summary>
	public interface IScriptConditionBlock : IScriptBlock
	{
		Boolean Evaluate(ILunyScriptContext context);
	}

	/// <summary>
	/// Interface for blocks that evaluate to a runtime Variable.
	/// </summary>
	public interface IScriptVariableBlock : IScriptBlock
	{
		Variable GetValue(ILunyScriptContext context);
	}

	/// <summary>
	/// Container blocks that can be executed by LunyScriptRunner.
	/// Sequences have IDs and can contain child blocks.
	/// </summary>
	public interface IScriptSequenceBlock : IScriptActionBlock
	{
		LunyScriptRunID ID { get; }
		IReadOnlyList<IScriptActionBlock> Blocks { get; }
		Boolean IsEmpty { get; }
	}

	/// <summary>
	/// Represents a timer block that can be controlled (started, stopped, paused, resumed).
	/// Timers fire after a duration elapses.
	/// </summary>
	public interface IScriptTimerBlock : IScriptActionBlock
	{
		/// <summary>
		/// Starts or restarts the timer.
		/// </summary>
		IScriptActionBlock Start();

		/// <summary>
		/// Stops the timer and resets its state.
		/// </summary>
		IScriptActionBlock Stop();

		/// <summary>
		/// Pauses the timer, preserving current elapsed time.
		/// </summary>
		IScriptActionBlock Pause();

		/// <summary>
		/// Resumes a paused timer.
		/// </summary>
		IScriptActionBlock Resume();

		/// <summary>
		/// Sets the time scale. Values >= 0; negative values are clamped to 0.
		/// </summary>
		void TimeScale(Double scale);
	}

	/// <summary>
	/// Represents a coroutine block that extends timer functionality with per-frame/heartbeat execution.
	/// Coroutines can run blocks on each update/heartbeat while running.
	/// </summary>
	public interface IScriptCoroutineBlock : IScriptTimerBlock
	{
		// Inherits all timer control methods.
		// Coroutines differ from timers in that they can run blocks per frame/heartbeat,
		// not just when elapsed.
	}
}
