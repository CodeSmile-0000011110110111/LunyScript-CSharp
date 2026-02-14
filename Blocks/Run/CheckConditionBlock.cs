using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Evaluates a custom method or lambda (Func returning Boolean).
	/// Useful for quick tests and one-off conditions.
	/// </summary>
	internal sealed class CheckConditionBlock : ScriptConditionBlock
	{
		private readonly Func<IScriptRuntimeContext, Boolean> _func;

		public static ScriptConditionBlock Create(Func<IScriptRuntimeContext, Boolean> func) => new CheckConditionBlock(func);

		private CheckConditionBlock(Func<IScriptRuntimeContext, Boolean> func) => _func = func ?? throw new ArgumentNullException(nameof(func));

		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) => _func(runtimeContext);
	}
}
