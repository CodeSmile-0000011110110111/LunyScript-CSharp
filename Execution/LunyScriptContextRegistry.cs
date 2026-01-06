using Luny;
using Luny.Engine.Bridge;
using Luny.Engine.Identity;
using LunyScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LunyScript.Execution
{
	/// <summary>
	/// Manages run contexts and their binding to objects.
	/// Provides deterministic iteration order based on ObjectID.
	/// </summary>
	internal sealed class LunyScriptContextRegistry
	{
		private readonly Dictionary<LunyObjectID, LunyScriptContext> _contextsByObjectID = new();
		private readonly Dictionary<LunyNativeObjectID, LunyScriptContext> _contextsByNativeID = new();
		private LunyScriptContext[] _sortedContexts = Array.Empty<LunyScriptContext>();
		private Boolean _isSortedContextsDirty;

		/// <summary>
		/// Gets all run contexts in deterministic order (sorted by ObjectID).
		/// </summary>
		public IReadOnlyList<LunyScriptContext> AllContexts =>
			!_isSortedContextsDirty ? _sortedContexts : _sortedContexts = CreateSortedContexts();

		/// <summary>
		/// Gets the number of registered contexts.
		/// </summary>
		public Int32 Count => _contextsByObjectID.Count;

		~LunyScriptContextRegistry() => LunyTraceLogger.LogInfoFinalized(this);

		public LunyScriptContext CreateContext(LunyScriptDefinition scriptDef, ILunyObject sceneObject)
		{
			var context = new LunyScriptContext(scriptDef, sceneObject);
			//LunyLogger.LogInfo($"{nameof(CreateContext)}: {context.ScriptID} -> {context.LunyObject}", this);
			Register(context);
			return context;
		}

		/// <summary>
		/// Registers a new run context.
		/// </summary>
		private void Register(LunyScriptContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			var lunyID = context.LunyObject.LunyObjectID;
			if (_contextsByObjectID.ContainsKey(lunyID))
				throw new LunyScriptException($"Context for object {context.LunyObject.Name} ({lunyID}) already registered, replacing");

			_contextsByObjectID[lunyID] = context;
			_contextsByNativeID[context.LunyObject.NativeObjectID] = context;
			_isSortedContextsDirty = true;
		}

		/// <summary>
		/// Unregisters a context by ObjectID.
		/// </summary>
		internal Boolean Unregister(LunyScriptContext context)
		{
			var lunyID = context.LunyObject.LunyObjectID;
			if (!_contextsByObjectID.Remove(lunyID))
				return false;

			_contextsByNativeID.Remove(context.LunyObject.NativeObjectID);
			_isSortedContextsDirty = true;
			return true;
		}

		/// <summary>
		/// Gets a context by ObjectID.
		/// </summary>
		public LunyScriptContext GetByLunyID(LunyObjectID lunyObjectID)
		{
			_contextsByObjectID.TryGetValue(lunyObjectID, out var context);
			return context;
		}

		public LunyScriptContext GetByNativeID(LunyNativeObjectID lunyNativeObjectID)
		{
			_contextsByNativeID.TryGetValue(lunyNativeObjectID, out var context);
			return context;
		}

		/// <summary>
		/// Checks if a context exists for the given ObjectID.
		/// </summary>
		public Boolean HasContext(LunyObjectID lunyObjectID) => _contextsByObjectID.ContainsKey(lunyObjectID);

		private LunyScriptContext[] CreateSortedContexts()
		{
			_isSortedContextsDirty = false;
			return _contextsByObjectID.Values
				.OrderBy(ctx => ctx.LunyObject.LunyObjectID)
				.ToArray();
		}

		internal void Shutdown()
		{
			LunyScriptContext.ClearGlobalVariables();
			_contextsByObjectID.Clear();
			_contextsByNativeID.Clear();
			_sortedContexts = Array.Empty<LunyScriptContext>();
		}
	}
}
