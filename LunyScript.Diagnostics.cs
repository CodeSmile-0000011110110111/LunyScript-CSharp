using LunyScript.Blocks;
using LunyScript.Interfaces;
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
		protected static IBlock Log(String message) => new LogMessageBlock(message);

		/// <summary>
		/// Logs a debug message that is completely stripped in release builds.
		/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
		/// </summary>
		protected static IBlock DebugLog(String message) => new DebugLogBlock(message);

		/// <summary>
		/// Triggers a debugger breakpoint (if debugger is attached).
		/// Completely stripped in release builds.
		/// Only breaks when DEBUG or LUNYSCRIPT_DEBUG is defined.
		/// </summary>
		protected static IBlock DebugBreak(String message = null) => new DebugBreakBlock(message);

		protected static class Debug
		{
			/// <summary>
			/// Logs a debug message that is completely stripped in release builds.
			/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
			/// </summary>
			public static IBlock Log(String message) => new DebugLogBlock(message);

			/// <summary>
			/// Triggers a debugger breakpoint (if debugger is attached).
			/// Completely stripped in release builds.
			/// Only breaks when DEBUG or LUNYSCRIPT_DEBUG is defined.
			/// </summary>
			public static IBlock Break(String message = null) => new DebugBreakBlock(message);
		}
	}
}
