using Luny;
using Luny.Engine.Bridge.Enums;
using LunyScript.Blocks;
using System;
using System.Collections.Generic;

namespace LunyScript.Events
{
	/// <summary>
	/// Schedules and manages sequences for various event types.
	/// </summary>
	internal sealed class ScriptEventScheduler
	{
		private static readonly Int32 s_ObjectEventCount = Enum.GetNames(typeof(LunyObjectEvent)).Length;
		private static readonly Int32 s_SceneEventCount = Enum.GetNames(typeof(LunySceneEvent)).Length;

		// Fast array-based storage for lifecycle events (hot path)
		private List<ScriptSequenceBlock>[] _objectEventSequences;
		private List<ScriptSequenceBlock>[] _sceneEventSequences;

		private static ScriptSequenceBlock ScheduleSequence(ref List<ScriptSequenceBlock>[] sequencesRef, ScriptSequenceBlock sequence,
			Int32 eventIndex, Int32 eventCount)
		{
			if (sequence != null && !sequence.IsEmpty)
			{
				sequencesRef ??= new List<ScriptSequenceBlock>[eventCount];
				sequencesRef[eventIndex] ??= new List<ScriptSequenceBlock>();
				sequencesRef[eventIndex].Add(sequence);
			}

			return sequence;
		}

		~ScriptEventScheduler() => LunyTraceLogger.LogInfoFinalized(this);

		internal ScriptSequenceBlock ScheduleSequence(ScriptActionBlock[] blocks, LunyObjectEvent objectEvent) =>
			ScheduleSequence(ref _objectEventSequences, SequenceBlock.TryCreate(blocks), (Int32)objectEvent, s_ObjectEventCount);

		internal ScriptSequenceBlock ScheduleSequence(ScriptActionBlock[] blocks, LunySceneEvent sceneEvent) =>
			ScheduleSequence(ref _sceneEventSequences, SequenceBlock.TryCreate(blocks), (Int32)sceneEvent, s_SceneEventCount);

		/// <summary>
		/// Gets all sequences scheduled for a specific lifecycle event.
		/// </summary>
		internal IEnumerable<ScriptSequenceBlock> GetSequences(LunyObjectEvent objectEvent) =>
			IsObserving((Int32)objectEvent, ref _objectEventSequences) ? _objectEventSequences[(Int32)objectEvent] : null;

		internal IEnumerable<ScriptSequenceBlock> GetSequences(LunySceneEvent sceneEvent) =>
			IsObserving((Int32)sceneEvent, ref _objectEventSequences) ? _objectEventSequences[(Int32)sceneEvent] : null;

		internal Boolean IsObserving(Int32 eventIndex, ref List<ScriptSequenceBlock>[] sequencesRef)
		{
			if (sequencesRef == null)
				return false;

			var eventSequences = sequencesRef[eventIndex];
			return eventSequences != null && eventSequences.Count > 0;
		}

		internal Boolean IsObservingAnyOf(Type enumType)
		{
			switch (enumType)
			{
				case not null when enumType == typeof(LunyObjectEvent):
					return _objectEventSequences != null;
				case not null when enumType == typeof(LunySceneEvent):
					return _sceneEventSequences != null;

				default:
					throw new ArgumentOutOfRangeException(nameof(enumType), enumType?.ToString());
			}
		}

		internal void Unschedule(LunyObjectEvent objectEvent)
		{
			if (_objectEventSequences == null)
				return;

			_objectEventSequences[(Int32)objectEvent] = null;
		}

		public void Shutdown() => GC.SuppressFinalize(this);
	}
}
