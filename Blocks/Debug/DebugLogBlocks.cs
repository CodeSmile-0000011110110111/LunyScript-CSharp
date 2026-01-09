using Luny;
using LunyScript.Execution;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only logging block base class.
	/// </summary>
	internal class DebugLogBlockBase : ILunyScriptBlock
	{
		protected String _message;
		private LogLevel _logLevel;

		private DebugLogBlockBase() {}

		protected DebugLogBlockBase(String message, LogLevel logLevel)
		{
			_message = message;
			_logLevel = logLevel;
		}

		public void Execute(ILunyScriptContext context) => DoLog(context);

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoLog(ILunyScriptContext context)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			switch (_logLevel)
			{
				case LogLevel.Info:
					LunyLogger.LogInfo($"{_message} ({context})", this);
					break;
				case LogLevel.Warning:
					LunyLogger.LogWarning($"{_message} ({context})", this);
					break;
				case LogLevel.Error:
					LunyLogger.LogError($"{_message} ({context})", this);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(_logLevel), _logLevel, context?.ToString());
			}
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
		public static ILunyScriptBlock Create(String message)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogInfoBlock(message);
#else
			return null;
#endif
		}

		private DebugLogInfoBlock(String message)
			: base(message, LogLevel.Info) {}
	}

	/// <summary>
	/// Debug-only logging block for "warning" messages (yellow text).
	/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	internal sealed class DebugLogWarningBlock : DebugLogBlockBase
	{
		public static ILunyScriptBlock Create(String message)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogWarningBlock(message);
#else
			return null;
#endif
		}

		private DebugLogWarningBlock(String message)
			: base(message, LogLevel.Warning) {}
	}

	/// <summary>
	/// Debug-only logging block for "error" messages (red text).
	/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	internal sealed class DebugLogErrorBlock : DebugLogBlockBase
	{
		public static ILunyScriptBlock Create(String message)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			return new DebugLogErrorBlock(message);
#else
			return null;
#endif
		}

		private DebugLogErrorBlock(String message)
			: base(message, LogLevel.Error) {}
	}
}
