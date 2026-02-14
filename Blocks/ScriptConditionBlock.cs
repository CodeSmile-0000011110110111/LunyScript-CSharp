using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for condition blocks that evaluate to a boolean result.
	/// </summary>
	public abstract class ScriptConditionBlock : ScriptBlock
	{
		public abstract Boolean Evaluate(IScriptRuntimeContext runtimeContext);
	}
}
