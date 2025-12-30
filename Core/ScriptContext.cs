using Luny;
using Luny.Proxies;
using LunyScript.Diagnostics;
using LunyScript.Exceptions;
using LunyScript.Execution;
using LunyScript.Registries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LunyScript
{
	// alias required within LunyScript due to namespace/class clash
	using ScriptEngine = LunyScriptEngine;

	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered runnables.
	/// </summary>
	public interface IScriptContext
	{
		ScriptID ScriptID { get; }
		Type ScriptType { get; }
		ILunyEngine LunyEngine { get; }
		ILunyScriptEngine LunyScriptEngine { get; }
		ILunyObject LunyObject { get; }
		IVariables GlobalVariables { get; }
		IVariables LocalVariables { get; }
		void SetObjectEnabled(Boolean enabled);
	}

	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered runnables.
	/// </summary>
	internal sealed class ScriptContext : IScriptContext
	{
		private static readonly Variables _GlobalVariables = new();
		private static Boolean EnableDisableEventProcessingSafeguard;

		private readonly IScriptDefinition _scriptDef;
		private readonly ILunyObject _lunyObject;
		//private readonly ScriptContextRegistry _contexts;

		internal Boolean DidRunOnReady { get; set; }
		internal Boolean DidRunOnDestroy { get; set; }

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
		public ILunyEngine LunyEngine => Luny.LunyEngine.Instance;
		/// <summary>
		/// The LunyScriptEngine instance.
		/// </summary>
		public ILunyScriptEngine LunyScriptEngine => ScriptEngine.Instance;
		/// <summary>
		/// The engine object/node this script operates on.
		/// </summary>
		public ILunyObject LunyObject => _lunyObject;

		/// <summary>
		/// Global variables shared across all scripts.
		/// </summary>
		public IVariables GlobalVariables { get; } = _GlobalVariables;
		/// <summary>
		/// Per-object variables for this script instance.
		/// </summary>
		public IVariables LocalVariables { get; } = new Variables();

		/// <summary>
		/// Debugging hooks for execution tracing and breakpoints.
		/// </summary>
		internal DebugHooks DebugHooks { get; }

		/// <summary>
		/// Block-level profiler for tracking runnable performance.
		/// </summary>
		internal BlockProfiler BlockProfiler { get; }

		/// <summary>
		/// Event scheduler for managing runnables across all event types.
		/// </summary>
		internal RunnableEventScheduler Scheduler { get; }

		internal static void ClearGlobalVariables() => _GlobalVariables?.Clear();

		internal static IVariables GetGlobalVariables() => _GlobalVariables;

		public ScriptContext(IScriptDefinition definition, ILunyObject lunyObject, ScriptContextRegistry contexts)
		{
			_scriptDef = definition ?? throw new ArgumentNullException(nameof(definition));
			_lunyObject = lunyObject ?? throw new ArgumentNullException(nameof(lunyObject));
			//_contexts = contexts ?? throw new ArgumentNullException(nameof(contexts));

			// TODO: don't create these unless enabled
			DebugHooks = new DebugHooks();
			BlockProfiler = new BlockProfiler();
			Scheduler = new RunnableEventScheduler();
		}

		public void SetObjectEnabled(Boolean enabled)
		{
			var objectEnabled = _lunyObject.Enabled;
			if (enabled && !objectEnabled || !enabled && objectEnabled)
			{
				_lunyObject.Enabled = enabled;

				if (EnableDisableEventProcessingSafeguard)
				{
					EnableDisableEventProcessingSafeguard = false;
					throw new LunyScriptException(
						$"Disabling in When.Enabled while ALSO enabling in When.Disabled is not allowed (infinite loop). Script: {this}");
				}

				EnableDisableEventProcessingSafeguard = true;
				if (enabled)
					LunyScriptRunner.RunObjectEnabled(this);
				else
					LunyScriptRunner.RunObjectDisabled(this);
				EnableDisableEventProcessingSafeguard = false;
			}
		}

		internal void Schedule(IRunnable runnable, ObjectLifecycleEvents lifecycleEvent) =>
			Scheduler.Schedule(runnable, lifecycleEvent);

		public override String ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"{nameof(ScriptContext)}: {ScriptType.Name} ({ScriptID}) -> {LunyObject}");
			sb.AppendLine($"  Valid: {LunyObject.IsValid}");

			if (LocalVariables.Count() > 0)
				sb.Append($"  {LocalVariables}");

			return sb.ToString();
		}
	}
}
