using Luny.Diagnostics;
using Luny.Proxies;
using LunyScript.Diagnostics;
using System;

namespace LunyScript.Execution
{
	// alias required within LunyScript due to namespace/class clash
	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered runnables.
	/// </summary>
	public interface IScriptContext
	{
		ScriptID ScriptID { get; }
		Type ScriptType { get; }
		ILunyObject LunyObject { get; }
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
		private readonly ILunyObject _lunyObject;

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
		internal ScriptEventScheduler Scheduler { get; }

		internal static void ClearGlobalVariables() => _GlobalVariables?.Clear();

		internal static IVariables GetGlobalVariables() => _GlobalVariables;

		public ScriptContext(IScriptDefinition definition, ILunyObject lunyObject)
		{
			_scriptDef = definition ?? throw new ArgumentNullException(nameof(definition));
			_lunyObject = lunyObject ?? throw new ArgumentNullException(nameof(lunyObject));

			// TODO: don't create these unless enabled
			DebugHooks = new DebugHooks();
			BlockProfiler = new BlockProfiler();
			Scheduler = new ScriptEventScheduler();
		}

		// ~ScriptContext() => LunyLogger.LogInfo($"finalized {GetHashCode()}", this);

		internal void Activate()
		{
			var lunyObject = (LunyObject)_lunyObject;
			//lunyObject.Name = lunyObject.ToString();
			lunyObject.Activate();
		}

		public override String ToString() => $"{ScriptID} -> {LunyObject}";
	}
}
