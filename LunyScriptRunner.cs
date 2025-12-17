using Luny;
using System;

namespace LunyScript
{
	/// <summary>
	/// Engine-agnostic script execution runner.
	/// Implements the lifecycle contract and will be discovered/reflected by
	/// the <see cref="Luny.EngineLifecycleDispatcher"/> at startup.
	/// </summary>
	public sealed class LunyScriptRunner : IEngineLifecycle
	{
		public void OnStartup() => LunyLogger.LogInfo("OnStartup()", this);

		// Initialize runner systems here (state machines, behavior trees, etc.)
		public void OnUpdate(Double deltaTime) =>
			// Process per-frame logic
			LunyLogger.LogInfo($"OnUpdate(deltaTime={deltaTime:0.000})", this);

		public void OnLateUpdate(Double deltaTime) => LunyLogger.LogInfo($"OnLateUpdate(deltaTime={deltaTime:0.000})", this);

		public void OnFixedStep(Double fixedDeltaTime) =>
			// Process fixed timestep logic
			LunyLogger.LogInfo($"OnFixedStep(fixedDeltaTime={fixedDeltaTime:0.000})", this);

		public void OnShutdown() =>
			// Cleanup runner systems
			LunyLogger.LogInfo("OnShutdown()", this);
	}
}
