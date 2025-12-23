using Luny.Proxies;
using LunyScript.Interfaces;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logs a message to the engine's logging system.
	/// </summary>
	public sealed class LogMessageBlock : IBlock
	{
		private readonly String _message;

		public LogMessageBlock(String message) => _message = message ?? throw new ArgumentNullException(nameof(message));

		public void Execute(ScriptContext context) => LunyLogger.LogInfo(_message, this);
	}
}
