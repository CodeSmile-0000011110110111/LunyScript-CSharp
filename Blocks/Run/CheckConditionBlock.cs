using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Evaluates a custom method or lambda (Func returning Boolean).
	/// Useful for quick tests and one-off conditions.
	/// </summary>
	internal sealed class CheckConditionBlock : IScriptConditionBlock
	{
		private readonly Func<ILunyScriptContext, Boolean> _func;

		public static IScriptConditionBlock Create(Func<ILunyScriptContext, Boolean> func) => new CheckConditionBlock(func);

		private CheckConditionBlock(Func<ILunyScriptContext, Boolean> func) => _func = func ?? throw new ArgumentNullException(nameof(func));

		public Boolean Evaluate(ILunyScriptContext context) => _func(context);
	}
}
