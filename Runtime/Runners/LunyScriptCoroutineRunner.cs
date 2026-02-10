using Luny;
using LunyScript.Blocks;
using LunyScript.Blocks.Coroutines;
using LunyScript.Coroutines;
using System;
using System.Collections.Generic;

namespace LunyScript.Runners
{
	/// <summary>
	/// Manages coroutines and timers for a script context.
	/// Handles registration, advancing, and lifecycle integration.
	/// Called by LunyScriptRunner after non-coroutine updates.
	/// </summary>
	internal sealed class LunyScriptCoroutineRunner
	{
		private readonly Dictionary<String, CoroutineEntry> _registry = new();
		private readonly List<CoroutineEntry> _heartbeatOnly = new();
		private readonly List<CoroutineEntry> _frameOnly = new();
		private readonly List<CoroutineEntry> _always = new();

		/// <summary>
		/// Gets the count of registered coroutines.
		/// </summary>
		internal Int32 Count => _registry.Count;

		/// <summary>
		/// Gets all registered coroutine names.
		/// </summary>
		internal IEnumerable<String> Names => _registry.Keys;

		/// <summary>
		/// Registers a new coroutine. Throws if name already exists.
		/// </summary>
		internal T Register<T>(in Coroutine.Options options) where T : class, IScriptCoroutineBlock
		{
			if (_registry.ContainsKey(options.Name))
				throw new InvalidOperationException($"Coroutine '{options.Name}' already exists. Duplicate names are not allowed.");

			if (options.IsTimeSliced && options.ProcessMode == Coroutine.Process.Always)
				throw new ArgumentException("Time-slicing is not supported for Coroutine.Process.Always mode.", nameof(options));

			var instance = Coroutine.Create(options);
			var entry = new CoroutineEntry(instance, options);
			_registry[options.Name] = entry;

			switch (options.ProcessMode)
			{
				case Coroutine.Process.Heartbeat:
					_heartbeatOnly.Add(entry);
					break;
				case Coroutine.Process.FrameUpdate:
					_frameOnly.Add(entry);
					break;
				case Coroutine.Process.Always:
					_always.Add(entry);
					break;
			}

			return CoroutineBlock.Create<T>(instance);
		}

		/// <summary>
		/// Gets an existing coroutine by name. Returns null if not found.
		/// </summary>
		internal Coroutine Get(String name) => _registry.TryGetValue(name, out var entry) ? entry.Instance : null;

		/// <summary>
		/// Checks if a coroutine with the given name exists.
		/// </summary>
		internal Boolean Exists(String name) => _registry.ContainsKey(name);

		/// <summary>
		/// Called on fixed step (heartbeat). Advances all running coroutines with OnHeartbeat sequences.
		/// Also advances count-based (heartbeat) coroutines.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnHeartbeat(ScriptRuntimeContext runtimeContext)
		{
			var time = LunyEngine.Instance.Time;
			var heartbeatCount = time.HeartbeatCount;

			for (var i = 0; i < _heartbeatOnly.Count; i++)
			{
				var entry = _heartbeatOnly[i];
				if (ShouldProcess(entry, heartbeatCount, Coroutine.Process.Heartbeat))
					RunSequences(entry, entry.Instance.ProcessHeartbeat(), runtimeContext);
			}

			for (var i = 0; i < _always.Count; i++)
			{
				var entry = _always[i];
				RunSequences(entry, entry.Instance.ProcessHeartbeat(), runtimeContext);
			}
		}

		/// <summary>
		/// Called on frame update. Advances all running time-based coroutines.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnFrameUpdate(ScriptRuntimeContext runtimeContext)
		{
			var time = LunyEngine.Instance.Time;
			var frameCount = time.FrameCount;

			for (var i = 0; i < _frameOnly.Count; i++)
			{
				var entry = _frameOnly[i];
				if (ShouldProcess(entry, frameCount, Coroutine.Process.FrameUpdate))
					RunSequences(entry, entry.Instance.ProcessFrameUpdate(), runtimeContext);
			}

			for (var i = 0; i < _always.Count; i++)
			{
				var entry = _always[i];
				RunSequences(entry, entry.Instance.ProcessFrameUpdate(), runtimeContext);
			}
		}

		private static Boolean ShouldProcess(in CoroutineEntry entry, Int64 tickCount, Coroutine.Process mode)
		{
			if (!entry.IsTimeSliced)
				return true;

			if (entry.SliceMode != mode)
				return true; // slicing applies only to the designated mode

			return (tickCount - entry.TimeSliceOffset) % entry.TimeSliceInterval == 0;
		}

		private static void RunSequences(in CoroutineEntry entry, CoroutineEvents events, ScriptRuntimeContext context)
		{
			if (events == CoroutineEvents.None)
				return;

			if (events.Has(CoroutineEvents.Started))
				LunyScriptBlockRunner.Run(entry.Sequences[0], context);
			if (events.Has(CoroutineEvents.Resumed))
				LunyScriptBlockRunner.Run(entry.Sequences[1], context);
			if (events.Has(CoroutineEvents.Heartbeat))
				LunyScriptBlockRunner.Run(entry.Sequences[2], context);
			if (events.Has(CoroutineEvents.FrameUpdate))
				LunyScriptBlockRunner.Run(entry.Sequences[3], context);
			if (events.Has(CoroutineEvents.Paused))
				LunyScriptBlockRunner.Run(entry.Sequences[4], context);
			if (events.Has(CoroutineEvents.Stopped))
				LunyScriptBlockRunner.Run(entry.Sequences[5], context);
			if (events.Has(CoroutineEvents.Elapsed))
				LunyScriptBlockRunner.Run(entry.Sequences[6], context);
		}

		/// <summary>
		/// Stops all coroutines when object is destroyed.
		/// </summary>
		public void OnObjectDestroyed(IScriptRuntimeContext runtimeContext)
		{
			foreach (var entry in _registry.Values)
				entry.Instance.OnObjectDestroyed();

			_registry.Clear();
			_heartbeatOnly.Clear();
			_frameOnly.Clear();
			_always.Clear();
		}

		~LunyScriptCoroutineRunner() => LunyTraceLogger.LogInfoFinalized(this);

		private sealed class CoroutineEntry
		{
			public readonly Coroutine Instance;
			public readonly IScriptSequenceBlock[] Sequences;
			public readonly Boolean IsTimeSliced;
			public readonly Int32 TimeSliceInterval;
			public readonly Int32 TimeSliceOffset;
			public readonly Coroutine.Process SliceMode;

			public CoroutineEntry(Coroutine instance, in Coroutine.Options options)
			{
				Instance = instance;
				IsTimeSliced = options.IsTimeSliced;
				TimeSliceInterval = Math.Max(1, options.TimeSliceInterval);
				TimeSliceOffset = Math.Max(0, options.TimeSliceOffset);
				SliceMode = options.ProcessMode;

				Sequences = new IScriptSequenceBlock[7];
				Sequences[0] = SequenceBlock.TryCreate(options.OnStarted);
				Sequences[1] = SequenceBlock.TryCreate(options.OnResumed);
				Sequences[2] = SequenceBlock.TryCreate(options.OnHeartbeat);
				Sequences[3] = SequenceBlock.TryCreate(options.OnFrameUpdate);
				Sequences[4] = SequenceBlock.TryCreate(options.OnPaused);
				Sequences[5] = SequenceBlock.TryCreate(options.OnStopped);
				Sequences[6] = SequenceBlock.TryCreate(options.OnElapsed);
			}
		}
	}
}
