using Luny.Core;
using System;

namespace LunyScript.Debugging
{
	/// <summary>
	/// Captures execution information for a single block execution.
	/// Used by debug hooks to track execution flow with frame/time information.
	/// </summary>
	public struct ExecutionTrace
	{
		public Int64 FrameCount;
		public Double ElapsedSeconds;
		public RunnableID RunnableID;
		public String BlockType;
		public String BlockDescription;
		public Exception Error;

		public override String ToString()
		{
			var errorSuffix = Error != null ? $" [ERROR: {Error.Message}]" : "";
			return $"[Frame {FrameCount:D8}] [{ElapsedSeconds:F3}s] Runnable#{RunnableID} {BlockType}{errorSuffix}";
		}
	}
}
