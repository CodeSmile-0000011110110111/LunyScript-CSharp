using Luny.Engine.Bridge.Enums;
using LunyScript.Blocks;
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

		private static Boolean HasBlocks(IReadOnlyList<ILunyScriptBlock> blocks) => blocks?.Count > 0;

		private static ILunyScriptRunnable CreateSequence(IReadOnlyList<ILunyScriptBlock> blocks) =>
			HasBlocks(blocks) ? new LunyScriptBlockSequence(blocks) : null;

		private static ILunyScriptRunnable ScheduleRunnable(ref List<ILunyScriptRunnable>[] runnablesRef, ILunyScriptRunnable runnable,
			Int32 eventIndex, Int32 eventCount)
		{
			if (runnable != null && !runnable.IsEmpty)
			{
				runnablesRef ??= new List<ILunyScriptRunnable>[eventCount];
				runnablesRef[eventIndex] ??= new List<ILunyScriptRunnable>();
				runnablesRef[eventIndex].Add(runnable);
			}

			return runnable;
		}

		internal ILunyScriptRunnable ScheduleSequence(ILunyScriptBlock[] blocks, LunyObjectEvent objectEvent) =>
			ScheduleRunnable(ref _objectEventRunnables, CreateSequence(blocks), (Int32)objectEvent, s_ObjectEventCount);

		internal ILunyScriptRunnable ScheduleSequence(ILunyScriptBlock[] blocks, LunySceneEvent sceneEvent) =>
			ScheduleRunnable(ref _sceneEventRunnables, CreateSequence(blocks), (Int32)sceneEvent, s_SceneEventCount);

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
