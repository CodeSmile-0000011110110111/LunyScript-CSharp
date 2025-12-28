using Luny.Diagnostics;
using LunyScript.Interfaces;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only logging block. Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// Posts to both Luny internal log and engine logging.
	/// </summary>
	internal sealed class DebugLogBlock : IBlock
	{
		private readonly String _message;

		public DebugLogBlock(String message) => _message = message ?? throw new ArgumentNullException(nameof(message));

		public void Execute(ScriptContext context) => DoLog(context);

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoLog(ScriptContext context)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			LunyLogger.LogInfo(_message, context.EngineObject);
#endif
		}

		public override String ToString() => $"DebugLog(\"{_message}\")";
	}
}
