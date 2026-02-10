using Luny;
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
		private readonly Dictionary<String, Coroutine> _coroutines = new();

		/// <summary>
		/// Gets the count of registered coroutines.
		/// </summary>
		internal Int32 Count => _coroutines.Count;

		/// <summary>
		/// Gets all registered coroutine names.
		/// </summary>
		internal IEnumerable<String> Names => _coroutines.Keys;

		private static Boolean ShouldRunTickBlocks(Coroutine coroutine, Int64 tickCount) => !coroutine.IsTimeSliced ||
		                                                                                    (tickCount - coroutine.TimeSliceOffset) %
		                                                                                    coroutine.TimeSliceInterval == 0;

		/// <summary>
		/// Registers a new coroutine. Throws if name already exists.
		/// </summary>
		internal Coroutine Register(in Coroutine.Options options)
		{
			if (_coroutines.ContainsKey(options.Name))
				throw new InvalidOperationException($"Coroutine '{options.Name}' already exists. Duplicate names are not allowed.");

			var instance = Coroutine.Create(options);
			_coroutines[options.Name] = instance;
			return instance;
		}

		/// <summary>
		/// Gets an existing coroutine by name. Returns null if not found.
		/// </summary>
		internal Coroutine Get(String name) => _coroutines.TryGetValue(name, out var instance) ? instance : null;

		/// <summary>
		/// Checks if a coroutine with the given name exists.
		/// </summary>
		internal Boolean Exists(String name) => _coroutines.ContainsKey(name);

		/// <summary>
		/// Called on fixed step (heartbeat). Advances all running coroutines with OnHeartbeat sequences.
		/// Also advances count-based (heartbeat) coroutines.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnHeartbeat(ScriptRuntimeContext runtimeContext)
		{
			var heartbeatCount = LunyEngine.Instance.Time.HeartbeatCount;

			foreach (var coroutine in _coroutines.Values)
			{
				if (!coroutine.ShouldProcess)
					continue;

				// Run OnHeartbeat sequence if any (pre-created, no allocation)
				// Time-sliced coroutines only run when interval matches
				var tickBlocks = coroutine.OnHeartbeatSequence;
				if (tickBlocks != null && ShouldRunTickBlocks(coroutine, heartbeatCount))
					LunyScriptBlockRunner.Run(tickBlocks, runtimeContext);

				// Advance count-based coroutines on each heartbeat
				if (coroutine.IsCounter)
				{
					var elapsed = coroutine.ProcessHeartbeat(runtimeContext);
					if (elapsed)
						LunyScriptBlockRunner.Run(coroutine.OnElapsedSequence, runtimeContext);
				}
			}
		}

		/// <summary>
		/// Called on frame update. Advances all running time-based coroutines.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnFrameUpdate(ScriptRuntimeContext runtimeContext)
		{
			var frameCount = LunyEngine.Instance.Time.FrameCount;

			foreach (var coroutine in _coroutines.Values)
			{
				if (!coroutine.ShouldProcess)
					continue;

				// Run OnUpdate sequence if any (pre-created, no allocation)
				// Time-sliced coroutines only run when interval matches
				var tickBlocks = coroutine.OnFrameUpdateSequence;
				if (tickBlocks != null && ShouldRunTickBlocks(coroutine, frameCount))
					LunyScriptBlockRunner.Run(tickBlocks, runtimeContext);

				// Advance time-based coroutines (count-based advance in OnHeartbeat)
				if (coroutine.IsTimer)
				{
					var elapsed = coroutine.ProcessFrameUpdate(runtimeContext);
					if (elapsed)
						LunyScriptBlockRunner.Run(coroutine.OnElapsedSequence, runtimeContext);
				}
			}
		}

		/// <summary>
		/// Stops all coroutines when object is destroyed.
		/// </summary>
		public void OnObjectDestroyed(IScriptRuntimeContext runtimeContext)
		{
			foreach (var coroutine in _coroutines.Values)
				coroutine.OnObjectDestroyed(runtimeContext);

			_coroutines.Clear();
		}

		~LunyScriptCoroutineRunner() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
