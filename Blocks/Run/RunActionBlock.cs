using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Executes a custom method or lambda (System.Action).
	/// Useful for quick tests and one-off logic.
	/// Prefer writing custom IBlock implementations for cleaner code and best reusability.
	/// </summary>
	internal sealed class RunActionBlock : IScriptActionBlock
	{
		private readonly Action<IScriptRuntimeContext> _action;
		public static IScriptActionBlock Create(Action<IScriptRuntimeContext> action) => new RunActionBlock(action);

		private RunActionBlock(Action<IScriptRuntimeContext> action) => _action = action ?? throw new ArgumentNullException(nameof(action));

		public void Execute(IScriptRuntimeContext runtimeContext) => _action(runtimeContext);
	}
}
