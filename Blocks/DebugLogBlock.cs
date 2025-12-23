using Luny.Proxies;
using LunyScript.Interfaces;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only logging block. Only logs when DEBUG/LUNY_DEBUG/LUNYSCRIPT_DEBUG is defined.
	/// Posts to both Luny internal log and engine logging.
	/// </summary>
	public sealed class DebugLogBlock : IBlock
	{
		private readonly String _message;

		public DebugLogBlock(String message)
		{
			_message = message ?? throw new ArgumentNullException(nameof(message));
		}

		public void Execute(RunContext context)
		{
			DoLog(context);
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoLog(RunContext context)
		{
#if DEBUG || LUNY_DEBUG || LUNYSCRIPT_DEBUG
			LunyLogger.LogInfo(_message, context.Object);
#endif
		}

		public override String ToString() => $"DebugLog(\"{_message}\")";
	}
}
