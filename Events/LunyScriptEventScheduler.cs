using Luny.Engine.Bridge.Enums;
using LunyScript.Runnables;
using System;
using System.Collections.Generic;

namespace LunyScript.Events
{
	/// <summary>
	/// Schedules and manages runnables for various event types.
	/// </summary>
	internal sealed class LunyScriptEventScheduler
	{
		private static readonly Int32 s_ObjectEventCount = Enum.GetNames(typeof(LunyObjectEvent)).Length;
		private static readonly Int32 s_SceneEventCount = Enum.GetNames(typeof(LunySceneEvent)).Length;

		// Fast array-based storage for lifecycle events (hot path)
		private List<ILunyScriptRunnable>[] _objectEventRunnables;
		private List<ILunyScriptRunnable>[] _sceneEventRunnables;

		//~LunyScriptEventScheduler() => LunyTraceLogger.LogInfoFinalized(this);

		/// <summary>
		/// Schedules a runnable to execute on a specific lifecycle event.
		/// </summary>
		internal ILunyScriptRunnable Schedule(ILunyScriptRunnable runnable, LunyObjectEvent objectEvent) =>
			Schedule(runnable, ref _objectEventRunnables, (Int32)objectEvent, s_ObjectEventCount);

		internal ILunyScriptRunnable Schedule(ILunyScriptRunnable runnable, LunySceneEvent sceneEvent) =>
			Schedule(runnable, ref _sceneEventRunnables, (Int32)sceneEvent, s_SceneEventCount);

		private ILunyScriptRunnable Schedule(ILunyScriptRunnable runnable, ref List<ILunyScriptRunnable>[] runnables, Int32 eventIndex,
			Int32 eventCount)
		{
			if (runnable == null || runnable.IsEmpty)
				return runnable;

			if (runnables == null)
				runnables = new List<ILunyScriptRunnable>[eventCount];

			runnables[eventIndex] ??= new List<ILunyScriptRunnable>();
			runnables[eventIndex].Add(runnable);

			return runnable;
		}

		/// <summary>
		/// Gets all runnables scheduled for a specific lifecycle event.
		/// </summary>
		internal IEnumerable<ILunyScriptRunnable> GetScheduled(LunyObjectEvent objectEvent) =>
			IsObserving((Int32)objectEvent, ref _objectEventRunnables) ? _objectEventRunnables[(Int32)objectEvent] : null;

		internal IEnumerable<ILunyScriptRunnable> GetScheduled(LunySceneEvent sceneEvent) =>
			IsObserving((Int32)sceneEvent, ref _objectEventRunnables) ? _objectEventRunnables[(Int32)sceneEvent] : null;

		internal Boolean IsObserving(Int32 eventIndex, ref List<ILunyScriptRunnable>[] runnables)
		{
			if (runnables == null)
				return false;

			var eventRunnables = runnables[eventIndex];
			return eventRunnables != null && eventRunnables.Count > 0;
		}

		internal Boolean IsObservingAnyOf(Type enumType)
		{
			switch (enumType)
			{
				case not null when enumType == typeof(LunyObjectEvent):
					return _objectEventRunnables != null;
				case not null when enumType == typeof(LunySceneEvent):
					return _sceneEventRunnables != null;

				default:
					throw new ArgumentOutOfRangeException(nameof(enumType), enumType?.ToString());
			}
		}

		internal void Unschedule(LunyObjectEvent objectEvent)
		{
			if (_objectEventRunnables == null)
				return;

			_objectEventRunnables[(Int32)objectEvent] = null;
		}
	}
}
