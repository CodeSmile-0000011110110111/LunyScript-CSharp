using System;
using System.Collections.Generic;
using System.Text;
using Luny.Core;
using Luny.Proxies;
using LunyScript.Debugging;
using LunyScript.Diagnostics;

namespace LunyScript
{
	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered runnables.
	/// </summary>
	public sealed class RunContext
	{
		/// <summary>
		/// The ID of the script definition this context executes.
		/// </summary>
		public ScriptID ScriptID { get; }

		/// <summary>
		/// The C# Type of the script (for hot reload matching).
		/// </summary>
		public Type ScriptType { get; }

		/// <summary>
		/// The engine object/node this script operates on.
		/// </summary>
		public LunyObject Object { get; }

		/// <summary>
		/// Per-object variables for this script instance.
		/// </summary>
		public Variables Variables { get; }

		/// <summary>
		/// Reference to global variables shared across all scripts.
		/// </summary>
		public Variables GlobalVariables { get; }

		/// <summary>
		/// Inspector-set variables (populated by engine-specific bridge).
		/// </summary>
		public Variables InspectorVariables { get; }

		/// <summary>
		/// Debugging hooks for execution tracing and breakpoints.
		/// </summary>
		public DebugHooks DebugHooks { get; }

		/// <summary>
		/// Block-level profiler for tracking runnable performance.
		/// </summary>
		public BlockProfiler BlockProfiler { get; }

		/// <summary>
		/// Runnables registered to execute on Update.
		/// </summary>
		public List<IRunnable> UpdateRunnables { get; }

		/// <summary>
		/// Runnables registered to execute on FixedStep.
		/// </summary>
		public List<IRunnable> FixedStepRunnables { get; }

		/// <summary>
		/// Runnables registered to execute on LateUpdate.
		/// </summary>
		public List<IRunnable> LateUpdateRunnables { get; }

		public RunContext(ScriptID scriptID, Type scriptType, LunyObject obj, Variables globalVariables)
		{
			ScriptID = scriptID;
			ScriptType = scriptType ?? throw new ArgumentNullException(nameof(scriptType));
			Object = obj ?? throw new ArgumentNullException(nameof(obj));
			GlobalVariables = globalVariables ?? throw new ArgumentNullException(nameof(globalVariables));

			Variables = new Variables();
			InspectorVariables = new Variables();
			DebugHooks = new DebugHooks();
			BlockProfiler = new BlockProfiler();

			UpdateRunnables = new List<IRunnable>();
			FixedStepRunnables = new List<IRunnable>();
			LateUpdateRunnables = new List<IRunnable>();
		}

		/// <summary>
		/// Whether the underlying object is still valid (not destroyed).
		/// </summary>
		public Boolean IsValid => Object.IsValid;

		public override String ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"RunContext: {ScriptType.Name} ({ScriptID}) -> {Object}");
			sb.AppendLine($"  Valid: {IsValid}");
			sb.AppendLine($"  Update Runnables: {UpdateRunnables.Count}");
			sb.AppendLine($"  FixedStep Runnables: {FixedStepRunnables.Count}");
			sb.AppendLine($"  LateUpdate Runnables: {LateUpdateRunnables.Count}");

			if (Variables.Count > 0)
				sb.Append($"  {Variables}");

			return sb.ToString();
		}
	}
}
