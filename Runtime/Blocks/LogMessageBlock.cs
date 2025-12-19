using System;
using Luny;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logs a message to the engine's logging system.
	/// </summary>
	public sealed class LogMessageBlock : IBlock
	{
		private readonly String _message;

		public LogMessageBlock(String message)
		{
			_message = message ?? throw new ArgumentNullException(nameof(message));
		}

		public void Execute(RunContext context)
		{
			LunyLogger.LogInfo(_message, this);
		}
	}
}
