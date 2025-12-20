using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		// User-facing API: Diagnostics

		/// <summary>
		/// Logs a message that appears in both debug and release builds.
		/// Posts to both Luny internal log (if enabled) and engine logging.
		/// </summary>
		protected static LogMessageBlock Log(String message) => new(message);

		/// <summary>
		/// Logs a debug message that is completely stripped in release builds.
		/// Only logs when DEBUG, LUNY_DEBUG, or LUNYSCRIPT_DEBUG is defined.
		/// </summary>
		protected static DebugLogBlock DebugLog(String message) => new(message);

		/// <summary>
		/// Triggers a debugger breakpoint. Completely stripped in release builds.
		/// Only breaks when DEBUG, LUNY_DEBUG, or LUNYSCRIPT_DEBUG is defined.
		/// </summary>
		protected static DebugBreakBlock DebugBreak(String message = null) => new(message);
	}
}
