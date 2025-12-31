using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Executes a custom method or lambda (System.Action).
	/// Useful for quick tests and one-off logic.
	/// Prefer writing custom IBlock implementations for cleaner code and best reusability.
	/// </summary>
	internal sealed class RunActionBlock : IBlock
	{
		private readonly Action<IScriptContext> _action;

		public RunActionBlock(Action<IScriptContext> action) => _action = action ?? throw new ArgumentNullException(nameof(action));

		public void Execute(IScriptContext context) => _action(context);
	}
}
