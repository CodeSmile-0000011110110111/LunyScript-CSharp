using Luny;
using Luny.Proxies;
using LunyScript.Diagnostics;
using LunyScript.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LunyScript
{
	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered runnables.
	/// </summary>
	public interface IScriptContext
	{
		ScriptID ScriptID { get; }
		Type ScriptType { get; }
		ILunyEngine Engine { get; }
		ILunyObject EngineObject { get; }
		IVariables GlobalVariables { get; }
		IVariables LocalVariables { get; }
	}

	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered runnables.
	/// </summary>
	internal sealed class ScriptContext : IScriptContext
	{
		private static readonly Variables _GlobalVariables = new();

		private readonly IScriptDefinition _scriptDef;
		private readonly ILunyObject _engineObject;
		private readonly IVariables _localVariables;

		/// <summary>
		/// The ID of the script definition this context executes.
		/// </summary>
		public ScriptID ScriptID => _scriptDef.ScriptID;

		/// <summary>
		/// The C# Type of the script (for hot reload matching).
		/// </summary>
		public Type ScriptType => _scriptDef.Type;
		/// <summary>
		/// The LunyEngine instance.
		/// </summary>
		public ILunyEngine Engine => LunyEngine.Instance;
		/// <summary>
		/// The engine object/node this script operates on.
		/// </summary>
		public ILunyObject EngineObject => _engineObject;

		/// <summary>
		/// Global variables shared across all scripts.
		/// </summary>
		public IVariables GlobalVariables { get; } = _GlobalVariables;
		/// <summary>
		/// Per-object variables for this script instance.
		/// </summary>
		public IVariables LocalVariables => _localVariables;

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
		internal IList<IRunnable> RunnablesScheduledInFixedStep { get; }

		/// <summary>
		/// Runnables registered to execute on Update.
		/// </summary>
		internal IList<IRunnable> RunnablesScheduledInUpdate { get; }

		/// <summary>
		/// Runnables registered to execute on LateUpdate.
		/// </summary>
		internal IList<IRunnable> RunnablesScheduledInLateUpdate { get; }

		internal static void ClearGlobalVariables() => _GlobalVariables?.Clear();

		internal static IVariables GetGlobalVariables() => _GlobalVariables;

		public ScriptContext(IScriptDefinition definition, ILunyObject engineObject)
		{
			_scriptDef = definition ?? throw new ArgumentNullException(nameof(definition));
			_engineObject = engineObject ?? throw new ArgumentNullException(nameof(engineObject));

			_localVariables = new Variables();

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

			if (LocalVariables.Count() > 0)
				sb.Append($"  {LocalVariables}");

			return sb.ToString();
		}
	}
}
