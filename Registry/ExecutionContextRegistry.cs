using System;
using System.Collections.Generic;
using System.Linq;
using Luny;
using Luny.Core;

namespace LunyScript
{
	/// <summary>
	/// Manages execution contexts and their binding to objects.
	/// Provides deterministic iteration order based on ObjectID.
	/// </summary>
	public sealed class ExecutionContextRegistry
	{
		private readonly Dictionary<ObjectID, ExecutionContext> _contextsByObjectID = new Dictionary<ObjectID, ExecutionContext>();
		private ExecutionContext[] _sortedContexts = Array.Empty<ExecutionContext>();
		private Boolean _needsSort = false;

		/// <summary>
		/// Gets all execution contexts in deterministic order (sorted by ObjectID).
		/// </summary>
		public IReadOnlyList<ExecutionContext> AllContexts
		{
			get
			{
				if (_needsSort)
				{
					RebuildSortedArray();
				}
				return _sortedContexts;
			}
		}

		/// <summary>
		/// Gets the number of registered contexts.
		/// </summary>
		public Int32 Count => _contextsByObjectID.Count;

		/// <summary>
		/// Registers a new execution context.
		/// </summary>
		public void Register(ExecutionContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			var objectID = context.Object.ID;

			if (_contextsByObjectID.ContainsKey(objectID))
			{
				LunyLogger.LogWarning($"Context for object {context.Object.Name} ({objectID}) already registered, replacing", this);
			}

			_contextsByObjectID[objectID] = context;
			_needsSort = true;

			LunyLogger.LogInfo($"Registered context: {context.ScriptID} -> {context.Object.Name} ({objectID})", this);
		}

		/// <summary>
		/// Unregisters a context by ObjectID.
		/// </summary>
		public Boolean Unregister(ObjectID objectID)
		{
			if (_contextsByObjectID.Remove(objectID))
			{
				_needsSort = true;
				LunyLogger.LogInfo($"Unregistered context for object {objectID}", this);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a context by ObjectID.
		/// </summary>
		public ExecutionContext GetByObjectID(ObjectID objectID)
		{
			_contextsByObjectID.TryGetValue(objectID, out var context);
			return context;
		}

		/// <summary>
		/// Checks if a context exists for the given ObjectID.
		/// </summary>
		public Boolean HasContext(ObjectID objectID) => _contextsByObjectID.ContainsKey(objectID);

		/// <summary>
		/// Removes all invalid contexts (where Object.IsValid == false).
		/// </summary>
		public Int32 RemoveInvalidContexts()
		{
			var invalidIDs = _contextsByObjectID
				.Where(kvp => !kvp.Value.IsValid)
				.Select(kvp => kvp.Key)
				.ToList();

			foreach (var id in invalidIDs)
			{
				_contextsByObjectID.Remove(id);
			}

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
			_contextsByObjectID.Clear();
			_sortedContexts = Array.Empty<ExecutionContext>();
			_needsSort = false;
		}

		private void RebuildSortedArray()
		{
			_sortedContexts = _contextsByObjectID.Values
				.OrderBy(ctx => ctx.Object.ID)
				.ToArray();
			_needsSort = false;
		}
	}
}
