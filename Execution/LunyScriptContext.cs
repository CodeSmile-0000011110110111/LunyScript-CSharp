using Luny;
using Luny.Engine.Bridge;
using LunyScript.Diagnostics;
using LunyScript.Events;
using System;
using System.Collections.Generic;

namespace LunyScript.Execution
{
	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered sequences.
	/// </summary>
	public interface ILunyScriptContext
	{
		LunyScriptID ScriptID { get; }
		Type ScriptType { get; }
		ILunyObject LunyObject { get; }
		ITable GlobalVariables { get; }
		ITable LocalVariables { get; }
		Stack<Int32> LoopStack { get; }
		Int32 LoopCount { get; }
	}

	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered sequences.
	/// </summary>
	internal sealed class LunyScriptContext : ILunyScriptContext
	{
		private static readonly ITable s_GlobalVariables = new Table();

		private readonly ILunyScriptDefinition _scriptDef;
		private readonly ILunyObject _lunyObject;

		private List<Double> _intervalTimers;

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
		public ITable GlobalVariables { get; } = s_GlobalVariables;
		/// <summary>
		/// Per-object variables for this script instance.
		/// </summary>
		public ITable LocalVariables { get; } = new Table();
		/// <summary>
		/// Stack for loop iteration counters.
		/// </summary>
		public Stack<Int32> LoopStack { get; } = new();
		/// <summary>
		/// Current loop iteration count. Returns 0 outside of loops.
		/// </summary>
		public Int32 LoopCount => LoopStack.Count > 0 ? LoopStack.Peek() : 0;

		/// <summary>
		/// Debugging hooks for execution tracing and breakpoints.
		/// </summary>
		internal LunyScriptDebugHooks DebugHooks { get; }

		/// <summary>
		/// Block-level profiler for tracking blocks performance.
		/// </summary>
		internal LunyScriptBlockProfiler BlockProfiler { get; }

		/// <summary>
		/// Event scheduler for managing sequences across all event types.
		/// </summary>
		internal LunyScriptEventScheduler Scheduler { get; }

		internal static void ClearGlobalVariables() => s_GlobalVariables?.RemoveAll();

		internal static ITable GetGlobalVariables() => s_GlobalVariables;

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
