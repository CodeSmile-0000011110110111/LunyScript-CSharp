using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Debugging hooks for execution tracing and breakpoints.
	/// Events and tracing are only invoked when DEBUG, LUNY_DEBUG, or LUNYSCRIPT_DEBUG is defined.
	/// </summary>
	public sealed class DebugHooks
	{
		private List<ExecutionTrace> _traces;

		/// <summary>
		/// Fired before a block executes. Only invoked in debug builds.
		/// </summary>
		public event Action<ExecutionTrace> OnBlockExecute;

		/// <summary>
		/// Fired after a block completes successfully. Only invoked in debug builds.
		/// </summary>
		public event Action<ExecutionTrace> OnBlockComplete;

		/// <summary>
		/// Fired when a block throws an exception. Only invoked in debug builds.
		/// </summary>
		public event Action<ExecutionTrace> OnBlockError;

		/// <summary>
		/// When true, execution traces are recorded in the Traces list.
		/// </summary>
		public Boolean EnableTracing { get; set; }

		/// <summary>
		/// Gets the recorded execution traces. Returns empty array if tracing is disabled or no traces recorded.
		/// </summary>
		public IReadOnlyList<ExecutionTrace> Traces =>
			_traces ?? (IReadOnlyList<ExecutionTrace>)Array.Empty<ExecutionTrace>();

		/// <summary>
		/// Clears all recorded execution traces.
		/// </summary>
		public void ClearTraces() => _traces?.Clear();

		[Conditional("DEBUG")]
		[Conditional("LUNY_DEBUG")]
		[Conditional("LUNYSCRIPT_DEBUG")]
		internal void NotifyBlockExecute(ExecutionTrace trace)
		{
#if DEBUG || LUNY_DEBUG || LUNYSCRIPT_DEBUG
			RecordTrace(trace);
			OnBlockExecute?.Invoke(trace);
#endif
		}

		[Conditional("DEBUG")]
		[Conditional("LUNY_DEBUG")]
		[Conditional("LUNYSCRIPT_DEBUG")]
		internal void NotifyBlockComplete(ExecutionTrace trace)
		{
#if DEBUG || LUNY_DEBUG || LUNYSCRIPT_DEBUG
			OnBlockComplete?.Invoke(trace);
#endif
		}

		[Conditional("DEBUG")]
		[Conditional("LUNY_DEBUG")]
		[Conditional("LUNYSCRIPT_DEBUG")]
		internal void NotifyBlockError(ExecutionTrace trace)
		{
#if DEBUG || LUNY_DEBUG || LUNYSCRIPT_DEBUG
			OnBlockError?.Invoke(trace);
#endif
		}

		private void RecordTrace(ExecutionTrace trace)
		{
			if (!EnableTracing)
				return;

			_traces ??= new List<ExecutionTrace>();
			_traces.Add(trace);
		}
	}
}
