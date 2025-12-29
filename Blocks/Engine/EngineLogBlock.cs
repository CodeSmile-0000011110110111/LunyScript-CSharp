using Luny.Diagnostics;
using LunyScript.Interfaces;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logs a message to the engine's logging system.
	/// </summary>
	internal sealed class EngineLogBlock : IBlock
	{
		private readonly String _message;

		public EngineLogBlock(String message) => _message = message ?? throw new ArgumentNullException(nameof(message));

		public void Execute(IScriptContext context) => LunyLogger.LogInfo(_message, this);
	}
}
