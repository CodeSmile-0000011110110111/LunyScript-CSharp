using System;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Performance metrics for a single block (runnable or individual block).
	/// Tracks execution time statistics and error counts.
	/// </summary>
	public sealed class BlockMetrics
	{
		public RunnableID RunnableID;
		public String BlockType;
		public Int32 CallCount;
		public Double TotalMs;
		public Double AverageMs;
		public Double MinMs;
		public Double MaxMs;
		public Int32 ErrorCount;

		public override String ToString() =>
			$"Runnable#{RunnableID} {BlockType}: {CallCount} calls, {AverageMs:F2}ms avg ({MinMs:F2}-{MaxMs:F2}ms), {ErrorCount} errors";
	}
}
