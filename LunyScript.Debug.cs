using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Provides diagnostics blocks which are omitted from release builds,
		/// unless the scripting symbol LUNYSCRIPT_DEBUG is defined.
		/// </summary>
		public static class Debug
		{
			/// <summary>
			/// Logs a debug message that is completely stripped in release builds.
			/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
			/// </summary>
			public static ILunyScriptBlock LogInfo(String message)
			{
#if DEBUG || LUNYSCRIPT_DEBUG
				return DebugLogInfoBlock.Create(message);
#else
				return null;
#endif
			}

			/// <summary>
			/// Logs a debug "warning" (yellow text) message.
			/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined, stripped in release builds.
			/// </summary>
			public static ILunyScriptBlock LogWarning(String message)
			{
#if DEBUG || LUNYSCRIPT_DEBUG
				return DebugLogWarningBlock.Create(message);
#else
				return null;
#endif
			}

			/// <summary>
			/// Logs a debug "error" (red text) message.
			/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined, stripped in release builds.
			/// </summary>
			public static ILunyScriptBlock LogError(String message)
			{
#if DEBUG || LUNYSCRIPT_DEBUG
				return DebugLogErrorBlock.Create(message);
#else
				return null;
#endif
			}

			/// <summary>
			/// Triggers a debugger breakpoint if debugger is attached by calling System.Diagnostics.Debugger.Break().
			/// Completely stripped in release builds.
			/// Only breaks when DEBUG or LUNYSCRIPT_DEBUG is defined.
			/// </summary>
			public static ILunyScriptBlock Break(String message = null)
			{
#if DEBUG || LUNYSCRIPT_DEBUG
				return DebugBreakBlock.Create(message);
#else
				return null;
#endif
			}
		}
	}
}
