using LunyScript.Interfaces;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Executes a custom action/lambda.
	/// Useful for quick tests and one-off logic.
	/// </summary>
	internal sealed class ActionBlock : IBlock
	{
		private readonly Action<ScriptContext> _action;

		public ActionBlock(Action<ScriptContext> action) => _action = action ?? throw new ArgumentNullException(nameof(action));

		public void Execute(ScriptContext context) => _action(context);
	}
}
