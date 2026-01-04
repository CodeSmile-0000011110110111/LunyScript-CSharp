using Luny;
using LunyScript.Execution;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only logging block base class.
	/// </summary>
	internal class DebugLogBlockBase : IBlock
	{
		protected String _message;
		private LogLevel _logLevel;

		protected DebugLogBlockBase(String message , LogLevel logLevel)
		{
			_message = message;
			_logLevel = logLevel;
		}

		public void Execute(IScriptContext context) => DoLog(context);

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoLog(IScriptContext context)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			LunyLogger.Log($"{_message} ({context})", this, _logLevel);
#endif
		}

		public override String ToString() => $"{GetType().Name}(\"{_message}\")";
	}

	/// <summary>
	/// Debug-only logging block for "info" messages (gray/white text).
	/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	internal sealed class DebugLogInfoBlock : DebugLogBlockBase
	{
		public DebugLogInfoBlock(String message)
			: base(message, LogLevel.Info) {}
	}

	/// <summary>
	/// Debug-only logging block for "warning" messages (yellow text).
	/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	internal sealed class DebugLogWarningBlock : DebugLogBlockBase
	{
		public DebugLogWarningBlock(String message)
			: base(message, LogLevel.Warning) {}
	}

	/// <summary>
	/// Debug-only logging block for "error" messages (red text).
	/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	internal sealed class DebugLogErrorBlock : DebugLogBlockBase
	{
		public DebugLogErrorBlock(String message)
			: base(message, LogLevel.Error) {}
	}
}
