using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Debugging hooks for execution tracing and breakpoints.
	/// Events and tracing are only invoked when DEBUG or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	public sealed class ScriptDebugHooks
	{
		/// <summary>
		/// Fired before a block executes. Only invoked in debug builds.
		/// </summary>
		public event Action<ScriptExecutionTrace> OnBlockExecute;

		/// <summary>
		/// Fired after a block completes successfully. Only invoked in debug builds.
		/// </summary>
		public event Action<ScriptExecutionTrace> OnBlockComplete;

		/// <summary>
		/// Fired when a block throws an exception. Only invoked in debug builds.
		/// </summary>
		public event Action<ScriptExecutionTrace> OnBlockError;
		private List<ScriptExecutionTrace> _traces;

		/// <summary>
		/// When true, execution traces are recorded in the Traces list.
		/// </summary>
		public Boolean EnableTracing { get; set; }

		/// <summary>
		/// Gets the recorded execution traces. Returns empty array if tracing is disabled or no traces recorded.
		/// </summary>
		public IReadOnlyList<ScriptExecutionTrace> Traces =>
			_traces ?? (IReadOnlyList<ScriptExecutionTrace>)Array.Empty<ScriptExecutionTrace>();

		/// <summary>
		/// Clears all recorded execution traces.
		/// </summary>
		public void ClearTraces() => _traces?.Clear();

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		internal void NotifyBlockExecute(in ScriptExecutionTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			RecordTrace(trace);
			OnBlockExecute?.Invoke(trace);
#endif
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		internal void NotifyBlockComplete(in ScriptExecutionTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			OnBlockComplete?.Invoke(trace);
#endif
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		internal void NotifyBlockError(in ScriptExecutionTrace trace)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			OnBlockError?.Invoke(trace);
#endif
		}

		private void RecordTrace(in ScriptExecutionTrace trace)
		{
			if (!EnableTracing)
				return;

			_traces ??= new List<ScriptExecutionTrace>();
			_traces.Add(trace);
		}
	}
}
