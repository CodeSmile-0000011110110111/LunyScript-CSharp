using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logs a message to the engine's logging system.
	/// </summary>
	internal sealed class EngineLogBlock : ILunyScriptBlock
	{
		private readonly String _message;

		public static ILunyScriptBlock Create(String message) => new EngineLogBlock(message);

		private EngineLogBlock(String message) => _message = message ?? throw new ArgumentNullException(nameof(message));

		public void Execute(ILunyScriptContext context) => LunyLogger.LogInfo(_message, this);
	}
}
