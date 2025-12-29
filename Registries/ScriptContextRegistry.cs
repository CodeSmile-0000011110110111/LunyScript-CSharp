using Luny;
using Luny.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LunyScript.Registries
{
	/// <summary>
	/// Manages run contexts and their binding to objects.
	/// Provides deterministic iteration order based on ObjectID.
	/// </summary>
	internal sealed class ScriptContextRegistry
	{
		private readonly Dictionary<LunyID, ScriptContext> _contextsByObjectID = new();
		private ScriptContext[] _sortedContexts = Array.Empty<ScriptContext>();
		private Boolean _needsSort;

		/// <summary>
		/// Gets all run contexts in deterministic order (sorted by ObjectID).
		/// </summary>
		public IReadOnlyList<ScriptContext> AllContexts
		{
			get
			{
				if (_needsSort)
					RebuildSortedArray();
				return _sortedContexts;
			}
		}

		/// <summary>
		/// Gets the number of registered contexts.
		/// </summary>
		public Int32 Count => _contextsByObjectID.Count;

		/// <summary>
		/// Registers a new run context.
		/// </summary>
		public void Register(ScriptContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			var objectID = context.LunyObject.LunyID;

			if (_contextsByObjectID.ContainsKey(objectID))
				LunyLogger.LogWarning($"Context for object {context.LunyObject.Name} ({objectID}) already registered, replacing", this);

			_contextsByObjectID[objectID] = context;
			_needsSort = true;

			LunyLogger.LogInfo($"Registered context: {context.ScriptID} -> {context.LunyObject.Name} ({objectID})", this);
		}

		/// <summary>
		/// Unregisters a context by ObjectID.
		/// </summary>
		public Boolean Unregister(LunyID lunyID)
		{
			if (_contextsByObjectID.Remove(lunyID))
			{
				_needsSort = true;
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
		/// Removes all invalid contexts (where Object.IsValid == false).
		/// </summary>
		public Int32 RemoveInvalidContexts()
		{
			// TODO: replace with traditional reverse iteration if LINQ proves to be slow
			var invalidIDs = _contextsByObjectID
				.Where(kvp => !kvp.Value.LunyObject.IsValid)
				.Select(kvp => kvp.Key)
				.ToList();

			foreach (var id in invalidIDs)
				_contextsByObjectID.Remove(id);

			if (invalidIDs.Count > 0)
			{
				_needsSort = true;
				LunyLogger.LogInfo($"Removed {invalidIDs.Count} invalid context(s)", this);
			}

			return invalidIDs.Count;
		}

		/// <summary>
		/// Clears all contexts.
		/// </summary>
		public void Clear()
		{
			ScriptContext.ClearGlobalVariables();
			_contextsByObjectID.Clear();
			_sortedContexts = Array.Empty<ScriptContext>();
			_needsSort = false;
		}

		private void RebuildSortedArray()
		{
			_sortedContexts = _contextsByObjectID.Values
				.OrderBy(ctx => ctx.LunyObject.LunyID)
				.ToArray();
			_needsSort = false;
		}
	}
}
