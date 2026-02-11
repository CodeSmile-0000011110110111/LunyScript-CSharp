using Luny;
using Luny.Engine.Bridge;
using Luny.Engine.Bridge.Identity;
using LunyScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LunyScript
{
	/// <summary>
	/// Manages run contexts and their binding to objects.
	/// Provides deterministic iteration order based on ObjectID.
	/// </summary>
	internal sealed class ScriptRuntimeContextRegistry
	{
		private readonly Dictionary<LunyObjectID, ScriptRuntimeContext> _contextsByObjectID = new();
		private readonly Dictionary<LunyNativeObjectID, ScriptRuntimeContext> _contextsByNativeID = new();
		private ScriptRuntimeContext[] _sortedContexts = Array.Empty<ScriptRuntimeContext>();
		private Boolean _isSortedContextsDirty;

		/// <summary>
		/// Gets all run contexts in deterministic order (sorted by ObjectID).
		/// </summary>
		public IReadOnlyList<ScriptRuntimeContext> AllContexts =>
			!_isSortedContextsDirty ? _sortedContexts : _sortedContexts = CreateSortedContexts();

		/// <summary>
		/// Gets the number of registered contexts.
		/// </summary>
		public Int32 Count => _contextsByObjectID.Count;

		~ScriptRuntimeContextRegistry() => LunyTraceLogger.LogInfoFinalized(this);

		public ScriptRuntimeContext CreateContext(ScriptDefinition scriptDef, ILunyObject sceneObject)
		{
			var context = new ScriptRuntimeContext(scriptDef, sceneObject);
			Register(context);
			return context;
		}

		/// <summary>
		/// Registers a new run context.
		/// </summary>
		private void Register(ScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext == null)
				throw new ArgumentNullException(nameof(runtimeContext));

			var lunyID = runtimeContext.LunyObject.LunyObjectID;
			if (_contextsByObjectID.ContainsKey(lunyID))
				throw new LunyScriptException($"Context for object {runtimeContext.LunyObject.Name} ({lunyID}) already registered, replacing");

			_contextsByObjectID[lunyID] = runtimeContext;
			_contextsByNativeID[runtimeContext.LunyObject.NativeObjectID] = runtimeContext;
			_isSortedContextsDirty = true;
		}

		/// <summary>
		/// Unregisters a context by ObjectID.
		/// </summary>
		internal Boolean Unregister(ScriptRuntimeContext runtimeContext)
		{
			var lunyID = runtimeContext.LunyObject.LunyObjectID;
			if (!_contextsByObjectID.Remove(lunyID))
				return false;

			_contextsByNativeID.Remove(runtimeContext.LunyObject.NativeObjectID);
			_isSortedContextsDirty = true;
			return true;
		}

		/// <summary>
		/// Gets a context by ObjectID.
		/// </summary>
		public ScriptRuntimeContext GetByLunyObjectID(LunyObjectID lunyObjectID)
		{
			_contextsByObjectID.TryGetValue(lunyObjectID, out var context);
			return context;
		}

		public ScriptRuntimeContext GetByNativeObjectID(LunyNativeObjectID lunyNativeObjectID)
		{
			_contextsByNativeID.TryGetValue(lunyNativeObjectID, out var context);
			return context;
		}

		/// <summary>
		/// Checks if a context exists for the given ObjectID.
		/// </summary>
		public Boolean HasContext(LunyObjectID lunyObjectID) => _contextsByObjectID.ContainsKey(lunyObjectID);

		private ScriptRuntimeContext[] CreateSortedContexts()
		{
			_isSortedContextsDirty = false;
			return _contextsByObjectID.Values
				.OrderBy(ctx => ctx.LunyObject.LunyObjectID)
				.ToArray();
		}

		internal void Shutdown()
		{
			ScriptRuntimeContext.ClearGlobalVariables();
			_contextsByObjectID.Clear();
			_contextsByNativeID.Clear();
			_sortedContexts = Array.Empty<ScriptRuntimeContext>();
		}
	}
}
