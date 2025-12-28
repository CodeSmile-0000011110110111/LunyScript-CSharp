using Luny;
using Luny.Diagnostics;
using Luny.Interfaces;
using Luny.Proxies;
using LunyScript.Diagnostics;
using LunyScript.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LunyScript
{
	public interface IScriptContext
	{
		ScriptDefinition ScriptDef { get; }
		ScriptID ScriptID { get; }
		Type ScriptType { get; }
		ILunyEngine Engine { get; }
		LunyObject EngineObject { get; }
		Variables GlobalVariables { get; }
		Variables LocalVariables { get; }
	}

	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered runnables.
	/// </summary>
	public sealed class ScriptContext : IScriptContext
	{
		/// <summary>
		/// Reference to global variables shared across all scripts.
		/// </summary>
		private static readonly Variables _GlobalVariables = new();

		public Variables GlobalVariables { get; } = _GlobalVariables;

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
		internal Variables InspectorVariables { get; }

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

		internal static void ClearGlobalVariables() => _GlobalVariables?.Clear();

		internal static IVariables GetGlobalVariables() => _GlobalVariables;

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

		internal void ScheduleRunnable(IRunnable runnable, ObjectLifecycleEvents lifecycleEvent)
		{
			if (runnable == null)
				return;

			switch (lifecycleEvent)
			{
				case ObjectLifecycleEvents.OnFixedStep:
					RunnablesScheduledInFixedStep.Add(runnable);
					break;
				case ObjectLifecycleEvents.OnUpdate:
					RunnablesScheduledInUpdate.Add(runnable);
					break;
				case ObjectLifecycleEvents.OnLateUpdate:
					RunnablesScheduledInLateUpdate.Add(runnable);
					break;

				case ObjectLifecycleEvents.OnCreate:
				case ObjectLifecycleEvents.OnDestroy:
				case ObjectLifecycleEvents.OnEnable:
				case ObjectLifecycleEvents.OnDisable:
				case ObjectLifecycleEvents.OnReady:
				default:
					throw new ArgumentOutOfRangeException(nameof(lifecycleEvent), lifecycleEvent,
						"Scheduling of this event type is not implemented yet");
			}
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
