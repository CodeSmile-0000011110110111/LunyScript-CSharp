using System;
using System.Collections.Generic;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Immutable snapshot of block profiler state at a specific point in time.
	/// Useful for querying performance metrics without blocking the profiler.
	/// </summary>
	public sealed class ScriptBlockProfilerSnapshot
	{
		public IReadOnlyList<ScriptBlockMetrics> BlockMetrics;
		public DateTime Timestamp;

		public override String ToString() => $"LunyScriptBlockProfilerSnapshot @ {Timestamp:HH:mm:ss.fff}: {BlockMetrics.Count} blocks";
	}
}
