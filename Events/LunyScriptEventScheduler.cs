using Luny.Engine.Bridge.Enums;
using LunyScript.Blocks;
using System;
using System.Collections.Generic;

namespace LunyScript.Events
{
	/// <summary>
	/// Schedules and manages sequences for various event types.
	/// </summary>
	internal sealed class LunyScriptEventScheduler
	{
		private static readonly Int32 s_ObjectEventCount = Enum.GetNames(typeof(LunyObjectEvent)).Length;
		private static readonly Int32 s_SceneEventCount = Enum.GetNames(typeof(LunySceneEvent)).Length;

		// Fast array-based storage for lifecycle events (hot path)
		private List<IScriptSequenceBlock>[] _objectEventSequences;
		private List<IScriptSequenceBlock>[] _sceneEventSequences;
		private List<(TimeSpan interval, IScriptSequenceBlock sequence)> _intervalSequences;

		internal IReadOnlyList<(TimeSpan interval, IScriptSequenceBlock sequence)> IntervalSequences => _intervalSequences;

		//~LunyScriptEventScheduler() => LunyTraceLogger.LogInfoFinalized(this);

		private static Boolean HasBlocks(IReadOnlyList<IScriptActionBlock> blocks) => blocks?.Count > 0;

		private static IScriptSequenceBlock CreateSequence(IReadOnlyList<IScriptActionBlock> blocks) =>
			HasBlocks(blocks) ? new SequenceBlock(blocks) : null;

		private static IScriptSequenceBlock ScheduleSequence(ref List<IScriptSequenceBlock>[] sequencesRef, IScriptSequenceBlock sequence,
			Int32 eventIndex, Int32 eventCount)
		{
			if (sequence != null && !sequence.IsEmpty)
			{
				sequencesRef ??= new List<IScriptSequenceBlock>[eventCount];
				sequencesRef[eventIndex] ??= new List<IScriptSequenceBlock>();
				sequencesRef[eventIndex].Add(sequence);
			}

			return sequence;
		}

		internal IScriptSequenceBlock ScheduleSequence(IScriptActionBlock[] blocks, LunyObjectEvent objectEvent) =>
			ScheduleSequence(ref _objectEventSequences, CreateSequence(blocks), (Int32)objectEvent, s_ObjectEventCount);

		internal IScriptSequenceBlock ScheduleSequence(IScriptActionBlock[] blocks, LunySceneEvent sceneEvent) =>
			ScheduleSequence(ref _sceneEventSequences, CreateSequence(blocks), (Int32)sceneEvent, s_SceneEventCount);

		internal IScriptSequenceBlock ScheduleSequence(IScriptActionBlock[] blocks, TimeSpan timeSpan)
		{
			var sequence = CreateSequence(blocks);
			if (sequence != null && !sequence.IsEmpty)
			{
				_intervalSequences ??= new List<(TimeSpan, IScriptSequenceBlock)>();
				_intervalSequences.Add((timeSpan, sequence));
			}

			return sequence;
		}

		/// <summary>
		/// Gets all sequences scheduled for a specific lifecycle event.
		/// </summary>
		internal IEnumerable<IScriptSequenceBlock> GetSequences(LunyObjectEvent objectEvent) =>
			IsObserving((Int32)objectEvent, ref _objectEventSequences) ? _objectEventSequences[(Int32)objectEvent] : null;

		internal IEnumerable<IScriptSequenceBlock> GetSequences(LunySceneEvent sceneEvent) =>
			IsObserving((Int32)sceneEvent, ref _objectEventSequences) ? _objectEventSequences[(Int32)sceneEvent] : null;

		internal Boolean IsObserving(Int32 eventIndex, ref List<IScriptSequenceBlock>[] sequencesRef)
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
	}
}
