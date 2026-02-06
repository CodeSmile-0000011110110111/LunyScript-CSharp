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
	internal sealed class LunyScriptCoroutineRunner
	{
		private readonly Dictionary<String, CoroutineInstance> _coroutines = new();

		/// <summary>
		/// Registers a new coroutine. Throws if name already exists.
		/// </summary>
		internal CoroutineInstance Register(String name)
		{
			if (_coroutines.ContainsKey(name))
				throw new InvalidOperationException($"Coroutine '{name}' already exists. Duplicate names are not allowed.");

			var instance = new CoroutineInstance(name);
			_coroutines[name] = instance;
			return instance;
		}

		/// <summary>
		/// Gets an existing coroutine by name. Returns null if not found.
		/// </summary>
		internal CoroutineInstance Get(String name) =>
			_coroutines.TryGetValue(name, out var instance) ? instance : null;

		/// <summary>
		/// Checks if a coroutine with the given name exists.
		/// </summary>
		internal Boolean Exists(String name) => _coroutines.ContainsKey(name);

		/// <summary>
		/// Called on frame update. Advances all running coroutines with OnUpdate sequences.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnUpdate(Single deltaTime, LunyScriptContext context)
		{
			foreach (var coroutine in _coroutines.Values)
			{
				if (coroutine.State != CoroutineState.Running)
					continue;

				// Run OnUpdate sequence if any (pre-created, no allocation)
				RunSequence(coroutine.OnUpdateSequence, context);

				// Advance and check if elapsed (only for duration-based coroutines without heartbeat)
				if (coroutine.HasDuration && coroutine.OnHeartbeatSequence == null)
				{
					var elapsed = coroutine.Advance(deltaTime);
					if (elapsed)
						RunSequence(coroutine.OnElapsedSequence, context);
				}
			}
		}

		/// <summary>
		/// Called on fixed step (heartbeat). Advances all running coroutines with OnHeartbeat sequences.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnFixedStep(Single fixedDeltaTime, LunyScriptContext context)
		{
			foreach (var coroutine in _coroutines.Values)
			{
				if (coroutine.State != CoroutineState.Running)
					continue;

				// Run OnHeartbeat sequence if any (pre-created, no allocation)
				RunSequence(coroutine.OnHeartbeatSequence, context);

				// Advance and check if elapsed (for heartbeat-based coroutines)
				if (coroutine.HasDuration && coroutine.OnHeartbeatSequence != null)
				{
					var elapsed = coroutine.Advance(fixedDeltaTime);
					if (elapsed)
						RunSequence(coroutine.OnElapsedSequence, context);
				}
			}
		}

		private static void RunSequence(IScriptSequenceBlock sequence, LunyScriptContext context)
		{
			if (sequence != null)
				LunyScriptRunner.Run(new[] { sequence }, context);
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

		/// <summary>
		/// Gets the count of registered coroutines.
		/// </summary>
		internal Int32 Count => _coroutines.Count;

		/// <summary>
		/// Gets all registered coroutine names.
		/// </summary>
		internal IEnumerable<String> Names => _coroutines.Keys;

		~LunyScriptCoroutineRunner() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
