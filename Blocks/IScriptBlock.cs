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
	/// Container blocks that can be executed by LunyScriptRunner.
	/// Sequences have IDs and can contain child blocks.
	/// </summary>
	public interface IScriptSequenceBlock : IScriptActionBlock
	{
		LunyScriptRunID ID { get; }
		IReadOnlyList<IScriptActionBlock> Blocks { get; }
		Boolean IsEmpty { get; }
	}
}
