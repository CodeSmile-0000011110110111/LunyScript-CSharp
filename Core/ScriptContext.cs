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
		/// Reference to global variables shared across all scripts.
		/// </summary>
		public static Variables GlobalVariables { get; } = new();

		/// <summary>
		/// The script definition this context uses.
		/// </summary>
		public ScriptDefinition ScriptDef { get; }

		/// <summary>
		/// The ID of the script definition this context executes.
		/// </summary>
		public ScriptID ScriptID => ScriptDef.ScriptID;

		/// <summary>
		/// The C# Type of the script (for hot reload matching).
		/// </summary>
		public Type ScriptType => ScriptDef.Type;

		/// <summary>
		/// The LunyEngine instance.
		/// </summary>
		public ILunyEngine Engine { get; set; }

		/// <summary>
		/// The engine object/node this script operates on.
		/// </summary>
		public LunyObject EngineObject { get; }

		/// <summary>
		/// Per-object variables for this script instance.
		/// </summary>
		public Variables LocalVariables { get; }

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

		public ScriptContext(ScriptDefinition definition, LunyObject engineObject, ILunyEngine engine)
		{
			ScriptDef = definition ?? throw new ArgumentNullException(nameof(definition));
			EngineObject = engineObject ?? throw new ArgumentNullException(nameof(engineObject));
			Engine = engine ?? throw new ArgumentNullException(nameof(engine));

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
			sb.AppendLine($"  Valid: {EngineObject.IsValid}");
			sb.AppendLine($"  FixedStep Runnables: {RunnablesScheduledInFixedStep.Count}");
			sb.AppendLine($"  Update Runnables: {RunnablesScheduledInUpdate.Count}");
			sb.AppendLine($"  LateUpdate Runnables: {RunnablesScheduledInLateUpdate.Count}");

			if (LocalVariables.Count > 0)
				sb.Append($"  {LocalVariables}");

			return sb.ToString();
		}
	}
}
