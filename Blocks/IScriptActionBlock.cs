using LunyScript.Execution;
using System;

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
}
