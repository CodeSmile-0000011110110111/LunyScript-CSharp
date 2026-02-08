using Luny;
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

		private static Boolean ShouldRunTickBlocks(CoroutineBase coroutine, Int64 tickCount) => !coroutine.IsTimeSliced ||
			(tickCount - coroutine.TimeSliceOffset) % coroutine.TimeSliceInterval == 0;

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
		internal void OnHeartbeat(LunyScriptContext context)
		{
			_heartbeatCount++;

			foreach (var coroutine in _coroutines.Values)
			{
				if (!coroutine.CanProcess)
					continue;

				coroutine.Init(context);

				// Run OnHeartbeat sequence if any (pre-created, no allocation)
				// Time-sliced coroutines only run when interval matches
				var tickBlocks = coroutine.OnHeartbeatSequence;
				if (tickBlocks != null && ShouldRunTickBlocks(coroutine, _heartbeatCount))
					LunyScriptRunner.Run(tickBlocks, context);

				// Advance count-based coroutines on each heartbeat
				if (coroutine.IsCounter)
				{
					var elapsed = coroutine.ProcessHeartbeat(context);
					if (elapsed)
						LunyScriptRunner.Run(coroutine.OnElapsedSequence, context);
				}
			}
		}

		/// <summary>
		/// Called on frame update. Advances all running time-based coroutines.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnFrameUpdate(LunyScriptContext context)
		{
			_frameCount++;

			foreach (var coroutine in _coroutines.Values)
			{
				if (!coroutine.CanProcess)
					continue;

				coroutine.Init(context);

				// Run OnUpdate sequence if any (pre-created, no allocation)
				// Time-sliced coroutines only run when interval matches
				var tickBlocks = coroutine.OnFrameUpdateSequence;
				if (tickBlocks != null && ShouldRunTickBlocks(coroutine, _frameCount))
					LunyScriptRunner.Run(tickBlocks, context);

				// Advance time-based coroutines (count-based advance in OnHeartbeat)
				if (coroutine.IsTimer)
				{
					var elapsed = coroutine.ProcessFrameUpdate(context);
					if (elapsed)
						LunyScriptRunner.Run(coroutine.OnElapsedSequence, context);
				}
			}
		}

		/// <summary>
		/// Stops all coroutines when object is destroyed.
		/// </summary>
		public void OnObjectDestroyed(ILunyScriptContext context)
		{
			foreach (var coroutine in _coroutines.Values)
				coroutine.OnObjectDestroyed(context);

			_coroutines.Clear();
		}

		~CoroutineRunner() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
