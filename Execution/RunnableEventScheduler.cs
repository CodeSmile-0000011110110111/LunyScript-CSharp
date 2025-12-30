using Luny;
using System;
using System.Collections.Generic;

namespace LunyScript.Execution
{
	/// <summary>
	/// Schedules and manages runnables for various event types.
	/// Uses optimized array storage for lifecycle events and typed dictionaries for other event categories.
	/// </summary>
	internal sealed class RunnableEventScheduler
	{
		// Fast array-based storage for lifecycle events (hot path)
		// ObjectLifecycleEvents uses sequential indexing (0-7)
		private List<IRunnable>[] _lifecycleRunnables;

		// Future: Add typed dictionaries for other event categories
		// private Dictionary<InputEventKey, List<IRunnable>> _inputRunnables;
		// private Dictionary<CollisionEventKey, List<IRunnable>> _collisionRunnables;

		/// <summary>
		/// Schedules a runnable to execute on a specific lifecycle event.
		/// </summary>
		internal void Schedule(IRunnable runnable, ObjectLifecycleEvents lifecycleEvent)
		{
			if (runnable == null || runnable.IsEmpty)
				return;

			_lifecycleRunnables ??= new List<IRunnable>[8]; // 8 lifecycle events defined in enum

			var index = (Int32)lifecycleEvent;
			_lifecycleRunnables[index] ??= new List<IRunnable>();
			_lifecycleRunnables[index].Add(runnable);
		}

		/// <summary>
		/// Gets all runnables scheduled for a specific lifecycle event.
		/// </summary>
		internal IEnumerable<IRunnable> GetScheduled(ObjectLifecycleEvents lifecycleEvent)
		{
			if (_lifecycleRunnables == null)
				return null;

			var index = (Int32)lifecycleEvent;
			return _lifecycleRunnables[index];
		}
	}
}
