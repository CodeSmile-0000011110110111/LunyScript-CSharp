using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Executes a custom method or lambda (System.Action).
	/// Useful for quick tests and one-off logic.
	/// Prefer writing custom IBlock implementations for cleaner code and best reusability.
	/// </summary>
	internal sealed class RunActionBlock : ILunyScriptBlock
	{
		private readonly Action<ILunyScriptContext> _action;
		public static ILunyScriptBlock Create(Action<ILunyScriptContext> action) => new RunActionBlock(action);

		public RunActionBlock(Action<ILunyScriptContext> action) => _action = action ?? throw new ArgumentNullException(nameof(action));

		public void Execute(ILunyScriptContext context) => _action(context);
	}
}
