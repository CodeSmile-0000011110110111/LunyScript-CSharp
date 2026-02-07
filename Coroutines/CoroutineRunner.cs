using Luny;
using LunyScript.Blocks;
using LunyScript.Execution;
using System;
using System.Collections.Generic;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Manages coroutines and timers for a script context.
	/// Handles registration, advancing, and lifecycle integration.
	/// Called by LunyScriptRunner after non-coroutine updates.
	/// </summary>
	internal sealed class CoroutineRunner
	{
		private readonly Dictionary<String, CoroutineBase> _coroutines = new();
		private Int64 _frameCount;
		private Int64 _heartbeatCount;

		/// <summary>
		/// Gets the count of registered coroutines.
		/// </summary>
		internal Int32 Count => _coroutines.Count;

		/// <summary>
		/// Gets all registered coroutine names.
		/// </summary>
		internal IEnumerable<String> Names => _coroutines.Keys;

		private static void RunSequence(IScriptSequenceBlock sequence, LunyScriptContext context)
		{
			if (sequence != null)
				LunyScriptRunner.Run(sequence, context);
		}

		private static Boolean ShouldRunThisTick(CoroutineBase coroutine, Int64 tickCount)
		{
			if (!coroutine.IsTimeSliced)
				return true;

			var interval = coroutine.TimeSliceInterval;
			var offset = coroutine.TimeSliceOffset;

			// Handle Even and Odd special values
			if (interval == LunyScript.Even) // Even
				return (tickCount + offset) % 2 == 0;

			if (interval == LunyScript.Odd) // Odd
				return (tickCount + offset) % 2 == 1;

			// Regular interval
			return (tickCount - offset) % interval == 0;
		}

		/// <summary>
		/// Registers a new coroutine. Throws if name already exists.
		/// </summary>
		internal CoroutineBase Register(in CoroutineConfig config)
		{
			if (_coroutines.ContainsKey(config.Name))
				throw new InvalidOperationException($"Coroutine '{config.Name}' already exists. Duplicate names are not allowed.");

			var instance = CoroutineBase.Create(config);
			_coroutines[config.Name] = instance;
			return instance;
		}

		/// <summary>
		/// Gets an existing coroutine by name. Returns null if not found.
		/// </summary>
		internal CoroutineBase Get(String name) => _coroutines.TryGetValue(name, out var instance) ? instance : null;

		/// <summary>
		/// Checks if a coroutine with the given name exists.
		/// </summary>
		internal Boolean Exists(String name) => _coroutines.ContainsKey(name);

		/// <summary>
		/// Called on fixed step (heartbeat). Advances all running coroutines with OnHeartbeat sequences.
		/// Also advances count-based (heartbeat) coroutines.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnHeartbeat(Double fixedDeltaTime, LunyScriptContext context)
		{
			_heartbeatCount++;

			foreach (var coroutine in _coroutines.Values)
			{
				if (coroutine.State != CoroutineState.Running)
					continue;

				// Run OnHeartbeat sequence if any (pre-created, no allocation)
				// Time-sliced coroutines only run when interval matches
				if (ShouldRunThisTick(coroutine, _heartbeatCount))
					RunSequence(coroutine.OnHeartbeatSequence, context);

				// Advance count-based coroutines on each heartbeat
				if (coroutine.IsCounter)
				{
					var elapsed = coroutine.Step();
					if (elapsed)
						RunSequence(coroutine.OnElapsedSequence, context);
				}
			}
		}

		/// <summary>
		/// Called on frame update. Advances all running time-based coroutines.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnFrameUpdate(Double deltaTime, LunyScriptContext context)
		{
			_frameCount++;

			foreach (var coroutine in _coroutines.Values)
			{
				if (coroutine.State != CoroutineState.Running)
					continue;

				// Run OnUpdate sequence if any (pre-created, no allocation)
				// Time-sliced coroutines only run when interval matches
				if (ShouldRunThisTick(coroutine, _frameCount))
					RunSequence(coroutine.OnUpdateSequence, context);

				// Advance time-based coroutines (count-based advance in OnHeartbeat)
				if (!coroutine.IsCounter)
				{
					var elapsed = coroutine.Update(deltaTime);
					if (elapsed)
						RunSequence(coroutine.OnElapsedSequence, context);
				}
			}
		}

		/// <summary>
		/// Auto-pauses all running coroutines when object is disabled.
		/// </summary>
		internal void OnObjectDisabled()
		{
			foreach (var coroutine in _coroutines.Values)
				coroutine.PauseByDisable();
		}

		/// <summary>
		/// Auto-resumes coroutines that were paused by disable when object is re-enabled.
		/// </summary>
		internal void OnObjectEnabled()
		{
			foreach (var coroutine in _coroutines.Values)
				coroutine.ResumeByEnable();
		}

		/// <summary>
		/// Stops all coroutines when object is destroyed.
		/// </summary>
		internal void OnObjectDestroyed()
		{
			foreach (var coroutine in _coroutines.Values)
				coroutine.Stop();

			_coroutines.Clear();
		}

		~CoroutineRunner() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
