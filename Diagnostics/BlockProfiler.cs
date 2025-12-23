using Luny;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Concrete implementation of block-level profiling for LunyScript execution.
	/// Tracks execution time for each runnable/block with configurable rolling average.
	/// Public methods use [Conditional] attributes - completely stripped in release builds unless DEBUG/LUNY_DEBUG/LUNYSCRIPT_DEBUG defined.
	/// </summary>
	public sealed class BlockProfiler
	{
		private readonly Dictionary<RunnableID, BlockMetrics> _metrics = new Dictionary<RunnableID, BlockMetrics>();
		private readonly Dictionary<RunnableID, Stopwatch> _activeBlocks = new Dictionary<RunnableID, Stopwatch>();
		private Int32 _rollingAverageWindow = 60;

		public Int32 RollingAverageWindow
		{
			get => _rollingAverageWindow;
			set => _rollingAverageWindow = Math.Max(1, value); // Clamp to minimum 1
		}

		[Conditional("DEBUG")]
		[Conditional("LUNY_DEBUG")]
		[Conditional("LUNYSCRIPT_DEBUG")]
		public void BeginBlock(RunnableID runnableID, String blockType)
		{
#if DEBUG || LUNY_DEBUG || LUNYSCRIPT_DEBUG
			if (!_activeBlocks.TryGetValue(runnableID, out var sw))
			{
				sw = new Stopwatch();
				_activeBlocks[runnableID] = sw;
			}
			sw.Restart();
#endif
		}

		[Conditional("DEBUG")]
		[Conditional("LUNY_DEBUG")]
		[Conditional("LUNYSCRIPT_DEBUG")]
		public void EndBlock(RunnableID runnableID, String blockType)
		{
#if DEBUG || LUNY_DEBUG || LUNYSCRIPT_DEBUG
			if (!_activeBlocks.TryGetValue(runnableID, out var sw))
				return;

			sw.Stop();
			var elapsed = sw.Elapsed.TotalMilliseconds;

			if (!_metrics.TryGetValue(runnableID, out var metrics))
			{
				metrics = new BlockMetrics
				{
					RunnableID = runnableID,
					BlockType = blockType
				};
				_metrics[runnableID] = metrics;
			}

			UpdateMetrics(metrics, elapsed);
#endif
		}

		[Conditional("DEBUG")]
		[Conditional("LUNY_DEBUG")]
		[Conditional("LUNYSCRIPT_DEBUG")]
		public void RecordError(RunnableID runnableID, Exception ex)
		{
#if DEBUG || LUNY_DEBUG || LUNYSCRIPT_DEBUG
			if (_metrics.TryGetValue(runnableID, out var metrics))
				metrics.ErrorCount++;
#endif
		}

		public BlockProfilerSnapshot TakeSnapshot()
		{
#if DEBUG || LUNY_DEBUG || LUNYSCRIPT_DEBUG
			return new BlockProfilerSnapshot
			{
				BlockMetrics = _metrics.Values.ToList(),
				Timestamp = DateTime.UtcNow
			};
#else
			return new BlockProfilerSnapshot
			{
				BlockMetrics = Array.Empty<BlockMetrics>(),
				Timestamp = DateTime.UtcNow
			};
#endif
		}

		[Conditional("DEBUG")]
		[Conditional("LUNY_DEBUG")]
		[Conditional("LUNYSCRIPT_DEBUG")]
		public void Reset()
		{
#if DEBUG || LUNY_DEBUG || LUNYSCRIPT_DEBUG
			_metrics.Clear();
			_activeBlocks.Clear();
#endif
		}

		private void UpdateMetrics(BlockMetrics metrics, Double newSample)
		{
			metrics.CallCount++;
			metrics.TotalMs += newSample;

			// Rolling average: disabled if window <= 1
			if (_rollingAverageWindow <= 1)
			{
				metrics.AverageMs = newSample; // No averaging, just use current sample
			}
			else
			{
				// Simple rolling average calculation
				var window = Math.Min(_rollingAverageWindow, metrics.CallCount);
				metrics.AverageMs = (metrics.AverageMs * (window - 1) + newSample) / window;
			}

			// Update min/max
			if (metrics.CallCount == 1)
			{
				metrics.MinMs = newSample;
				metrics.MaxMs = newSample;
			}
			else
			{
				metrics.MinMs = Math.Min(metrics.MinMs, newSample);
				metrics.MaxMs = Math.Max(metrics.MaxMs, newSample);
			}
		}
	}
}
