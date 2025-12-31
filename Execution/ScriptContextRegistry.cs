using Luny;
using Luny.Diagnostics;
using Luny.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LunyScript.Execution
{
	/// <summary>
	/// Manages run contexts and their binding to objects.
	/// Provides deterministic iteration order based on ObjectID.
	/// </summary>
	internal sealed class ScriptContextRegistry
	{
		private readonly Dictionary<LunyID, ScriptContext> _contextsByObjectID = new();
		private ScriptContext[] _sortedContexts = Array.Empty<ScriptContext>();
		private Boolean _isSortedContextsDirty;

		/// <summary>
		/// Gets all run contexts in deterministic order (sorted by ObjectID).
		/// </summary>
		public IReadOnlyList<ScriptContext> AllContexts => !_isSortedContextsDirty ? _sortedContexts : _sortedContexts = CreateSortedContexts();

		/// <summary>
		/// Gets the number of registered contexts.
		/// </summary>
		public Int32 Count => _contextsByObjectID.Count;

		public ScriptContext CreateContext(ScriptDefinition scriptDef, ILunyObject sceneObject)
		{
			var context = new ScriptContext(scriptDef, sceneObject);
			Register(context);
			return context;
		}

		/// <summary>
		/// Registers a new run context.
		/// </summary>
		private void Register(ScriptContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			var objectID = context.LunyObject.LunyID;

			if (_contextsByObjectID.ContainsKey(objectID))
				LunyLogger.LogWarning($"Context for object {context.LunyObject.Name} ({objectID}) already registered, replacing", this);

			_contextsByObjectID[objectID] = context;
			_isSortedContextsDirty = true;

			LunyLogger.LogInfo($"Registered context: {context.ScriptID} -> {context.LunyObject.Name} ({objectID})", this);
		}

		/// <summary>
		/// Unregisters a context by ObjectID.
		/// </summary>
		internal Boolean Unregister(ScriptContext context)
		{
			var lunyID = context.LunyObject.LunyID;
			if (_contextsByObjectID.Remove(lunyID))
			{
				_isSortedContextsDirty = true;
				LunyLogger.LogInfo($"Unregistered context for object {lunyID}", this);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a context by ObjectID.
		/// </summary>
		public ScriptContext GetByLunyID(LunyID lunyID)
		{
			_contextsByObjectID.TryGetValue(lunyID, out var context);
			return context;
		}

		public ScriptContext GetByNativeID(NativeID nativeID)
		{
			foreach (var context in _contextsByObjectID.Values)
			{
				var engineObject = context.LunyObject;
				if (engineObject.IsValid && engineObject.NativeID == nativeID)
					return context;
			}
			return null;
		}

		/// <summary>
		/// Checks if a context exists for the given ObjectID.
		/// </summary>
		public Boolean HasContext(LunyID lunyID) => _contextsByObjectID.ContainsKey(lunyID);

		/// <summary>
		/// Clears all contexts.
		/// </summary>
		public void Clear()
		{
			ScriptContext.ClearGlobalVariables();
			_contextsByObjectID.Clear();
			_sortedContexts = Array.Empty<ScriptContext>();
			_isSortedContextsDirty = false;
		}

		private ScriptContext[] CreateSortedContexts()
		{
			_isSortedContextsDirty = false;
			return _contextsByObjectID.Values
				.OrderBy(ctx => ctx.LunyObject.LunyID)
				.ToArray();
		}
	}
}
