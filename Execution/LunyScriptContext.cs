using Luny;
using Luny.Engine.Bridge;
using LunyScript.Diagnostics;
using LunyScript.Events;
using System;

namespace LunyScript.Execution
{
	// alias required within LunyScript due to namespace/class clash
	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered runnables.
	/// </summary>
	public interface ILunyScriptContext
	{
		LunyScriptID ScriptID { get; }
		Type ScriptType { get; }
		ILunyObject LunyObject { get; }
		ILunyScriptVariables GlobalVariables { get; }
		ILunyScriptVariables LocalVariables { get; }
	}

	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered runnables.
	/// </summary>
	internal sealed class LunyScriptContext : ILunyScriptContext
	{
		private static readonly LunyScriptVariables _GlobalVariables = new();

		private readonly ILunyScriptDefinition _scriptDef;
		private readonly ILunyObject _lunyObject;

		/// <summary>
		/// The ID of the script definition this context executes.
		/// </summary>
		public LunyScriptID ScriptID => _scriptDef.ScriptID;
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
		public ILunyScriptVariables GlobalVariables { get; } = _GlobalVariables;
		/// <summary>
		/// Per-object variables for this script instance.
		/// </summary>
		public ILunyScriptVariables LocalVariables { get; } = new LunyScriptVariables();

		/// <summary>
		/// Debugging hooks for execution tracing and breakpoints.
		/// </summary>
		internal LunyScriptDebugHooks DebugHooks { get; }

		/// <summary>
		/// Block-level profiler for tracking runnable performance.
		/// </summary>
		internal LunyScriptBlockProfiler BlockProfiler { get; }

		/// <summary>
		/// Event scheduler for managing runnables across all event types.
		/// </summary>
		internal LunyScriptEventScheduler Scheduler { get; }

		internal static void ClearGlobalVariables() => _GlobalVariables?.Clear();

		internal static ILunyScriptVariables GetGlobalVariables() => _GlobalVariables;

		public LunyScriptContext(ILunyScriptDefinition definition, ILunyObject lunyObject)
		{
			_scriptDef = definition ?? throw new ArgumentNullException(nameof(definition));
			_lunyObject = lunyObject ?? throw new ArgumentNullException(nameof(lunyObject));

			// TODO: don't create these unless enabled
			DebugHooks = new LunyScriptDebugHooks();
			BlockProfiler = new LunyScriptBlockProfiler();
			Scheduler = new LunyScriptEventScheduler();
		}

		~LunyScriptContext() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Activate() => _lunyObject.ActivateOnceBeforeUse();

		public override String ToString() => $"{ScriptID} -> {LunyObject}";
	}
}
