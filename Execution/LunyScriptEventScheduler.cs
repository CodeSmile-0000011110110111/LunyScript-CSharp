using Luny;
using Luny.Engine.Bridge.Enums;
using LunyScript.Runnables;
using System;
using System.Collections.Generic;

namespace LunyScript.Execution
{
	/// <summary>
	/// Schedules and manages runnables for various event types.
	/// </summary>
	internal sealed class LunyScriptEventScheduler
	{
		private static readonly Int32 s_LifecycleEventCount = Enum.GetNames(typeof(LunyObjectLifecycleEvents)).Length;

		// Fast array-based storage for lifecycle events (hot path)
		private List<ILunyScriptRunnable>[] _runnables;

		// Future: Add typed dictionaries for other event categories
		// private Dictionary<InputEventKey, List<IRunnable>> _inputRunnables;
		// private Dictionary<CollisionEventKey, List<IRunnable>> _collisionRunnables;

		~LunyScriptEventScheduler() => LunyTraceLogger.LogInfoFinalized(this);

		/// <summary>
		/// Schedules a runnable to execute on a specific lifecycle event.
		/// </summary>
		internal ILunyScriptRunnable Schedule(ILunyScriptRunnable runnable, LunyObjectLifecycleEvents lifecycleEvent)
		{
			if (runnable == null || runnable.IsEmpty)
				return runnable;

			// TODO: consider pre-allocating only the frequently scheduled "update" methods
			if (_runnables == null)
				_runnables = new List<ILunyScriptRunnable>[s_LifecycleEventCount];

			var index = (Int32)lifecycleEvent;
			_runnables[index] ??= new List<ILunyScriptRunnable>();
			_runnables[index].Add(runnable);

			return runnable;
		}

		/// <summary>
		/// Gets all runnables scheduled for a specific lifecycle event.
		/// </summary>
		internal IEnumerable<ILunyScriptRunnable> GetScheduled(LunyObjectLifecycleEvents lifecycleEvent) =>
			IsObserving(lifecycleEvent) ? _runnables[(Int32)lifecycleEvent] : null;

		internal Boolean IsObserving(LunyObjectLifecycleEvents lifecycleEvent)
		{
			if (_runnables == null)
				return false;

			var runnables = _runnables[(Int32)lifecycleEvent];
			return runnables != null && runnables.Count > 0;
		}

		public void Clear() => _runnables = null;

		public void Clear(LunyObjectLifecycleEvents lifecycleEvent)
		{
			if (_runnables == null)
				return;

			_runnables[(Int32)lifecycleEvent] = null;
		}
	}
}
