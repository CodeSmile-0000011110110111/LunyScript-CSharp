using LunyScript.Interfaces;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Executes a custom action/lambda.
	/// Useful for quick tests and one-off logic.
	/// </summary>
	public sealed class ActionBlock : IBlock
	{
		private readonly Action<RunContext> _action;

		public ActionBlock(Action<RunContext> action)
		{
			_action = action ?? throw new ArgumentNullException(nameof(action));
		}

		public void Execute(RunContext context)
		{
			_action(context);
		}
	}
}
