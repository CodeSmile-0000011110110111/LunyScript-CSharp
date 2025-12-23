using Luny.Proxies;
using LunyScript.Diagnostics;
using LunyScript.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LunyScript
{
	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered runnables.
	/// </summary>
	public sealed class ScriptContext
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
		public LunyObject EngineObject { get; }

		/// <summary>
		/// Whether the underlying object is still valid (not destroyed).
		/// </summary>
		public Boolean IsEngineObjectValid => EngineObject.IsValid;

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
		internal BlockProfiler BlockProfiler { get; }

		/// <summary>
		/// Runnables registered to execute on FixedStep.
		/// </summary>
		internal List<IRunnable> RunnablesScheduledInFixedStep { get; }

		/// <summary>
		/// Runnables registered to execute on Update.
		/// </summary>
		internal List<IRunnable> RunnablesScheduledInUpdate { get; }

		/// <summary>
		/// Runnables registered to execute on LateUpdate.
		/// </summary>
		internal List<IRunnable> RunnablesScheduledInLateUpdate { get; }

		public ScriptContext(ScriptID scriptID, Type scriptType, LunyObject obj, Variables globalVariables)
		{
			ScriptID = scriptID;
			ScriptType = scriptType ?? throw new ArgumentNullException(nameof(scriptType));
			EngineObject = obj ?? throw new ArgumentNullException(nameof(obj));
			GlobalVariables = globalVariables ?? throw new ArgumentNullException(nameof(globalVariables));

			Variables = new Variables();
			InspectorVariables = new Variables();
			DebugHooks = new DebugHooks();
			BlockProfiler = new BlockProfiler();

			RunnablesScheduledInFixedStep = new List<IRunnable>();
			RunnablesScheduledInUpdate = new List<IRunnable>();
			RunnablesScheduledInLateUpdate = new List<IRunnable>();
		}

		public override String ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"RunContext: {ScriptType.Name} ({ScriptID}) -> {EngineObject}");
			sb.AppendLine($"  Valid: {IsEngineObjectValid}");
			sb.AppendLine($"  FixedStep Runnables: {RunnablesScheduledInFixedStep.Count}");
			sb.AppendLine($"  Update Runnables: {RunnablesScheduledInUpdate.Count}");
			sb.AppendLine($"  LateUpdate Runnables: {RunnablesScheduledInLateUpdate.Count}");

			if (Variables.Count > 0)
				sb.Append($"  {Variables}");

			return sb.ToString();
		}
	}
}
