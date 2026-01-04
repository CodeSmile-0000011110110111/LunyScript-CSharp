using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Debugging hooks for execution tracing and breakpoints.
	/// Events and tracing are only invoked when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	public sealed class LunyScriptDebugHooks
	{
		/// <summary>
		/// Fired before a block executes. Only invoked in debug builds.
		/// </summary>
		public event Action<LunyScriptExecutionTrace> OnBlockExecute;

		/// <summary>
		/// Fired after a block completes successfully. Only invoked in debug builds.
		/// </summary>
		public event Action<LunyScriptExecutionTrace> OnBlockComplete;

		/// <summary>
		/// Fired when a block throws an exception. Only invoked in debug builds.
		/// </summary>
		public event Action<LunyScriptExecutionTrace> OnBlockError;
		private List<LunyScriptExecutionTrace> _traces;

		/// <summary>
		/// When true, execution traces are recorded in the Traces list.
		/// </summary>
		public Boolean EnableTracing { get; set; }

		/// <summary>
		/// Gets the recorded execution traces. Returns empty array if tracing is disabled or no traces recorded.
		/// </summary>
		public IReadOnlyList<LunyScriptExecutionTrace> Traces =>
			_traces ?? (IReadOnlyList<LunyScriptExecutionTrace>)Array.Empty<LunyScriptExecutionTrace>();

		/// <summary>
		/// Clears all recorded execution traces.
		/// </summary>
		public void ClearTraces() => _traces?.Clear();

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		internal void NotifyBlockExecute(in LunyScriptExecutionTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			RecordTrace(trace);
			OnBlockExecute?.Invoke(trace);
#endif
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		internal void NotifyBlockComplete(in LunyScriptExecutionTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			OnBlockComplete?.Invoke(trace);
#endif
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		internal void NotifyBlockError(in LunyScriptExecutionTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			OnBlockError?.Invoke(trace);
#endif
		}

		private void RecordTrace(in LunyScriptExecutionTrace trace)
		{
			if (!EnableTracing)
				return;

			_traces ??= new List<LunyScriptExecutionTrace>();
			_traces.Add(trace);
		}
	}
}
