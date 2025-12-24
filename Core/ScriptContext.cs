using Luny.Interfaces;
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
		/// The LunyEngine instance.
		/// </summary>
		public ILunyEngine Engine { get; set; }

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
		public Variables LocalVariables { get; }

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
		internal DebugHooks DebugHooks { get; }

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

		public ScriptContext(ScriptID scriptID, Type scriptType, ILunyEngine engine, LunyObject engineObject, Variables globalVariables)
		{
			// TODO: ScriptType is unnecessary?
			ScriptID = scriptID;
			ScriptType = scriptType ?? throw new ArgumentNullException(nameof(scriptType));
			Engine = engine ?? throw new ArgumentNullException(nameof(engine));
			EngineObject = engineObject ?? throw new ArgumentNullException(nameof(engineObject));

			GlobalVariables = globalVariables ?? throw new ArgumentNullException(nameof(globalVariables));
			LocalVariables = new Variables();
			InspectorVariables = new Variables();

			// TODO: don't create these unless enabled
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

			if (LocalVariables.Count > 0)
				sb.Append($"  {LocalVariables}");

			return sb.ToString();
		}
	}
}
