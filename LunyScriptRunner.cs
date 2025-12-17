using System;
using Luny;

namespace LunyScript
{
    /// <summary>
    /// Engine-agnostic script execution runner.
    /// Implements the lifecycle contract and will be discovered/reflected by
    /// the <see cref="Luny.EngineLifecycleDispatcher"/> at startup.
    /// </summary>
    public sealed class LunyScriptRunner : IEngineLifecycle
    {
        public void OnStartup()
        {
            LunyLogger.LogInfo("[LunyScriptRunner] OnStartup()");
            // Initialize runner systems here (state machines, behavior trees, etc.)
        }

        public void OnUpdate(double deltaTime)
        {
            // Process per-frame logic
            LunyLogger.LogInfo($"[LunyScriptRunner] OnUpdate(deltaTime={deltaTime:0.000})");
        }

        public void OnLateUpdate(Double deltaTime)
        {
	        LunyLogger.LogInfo($"[LunyScriptRunner] OnLateUpdate(deltaTime={deltaTime:0.000})");
        }

        public void OnFixedStep(double fixedDeltaTime)
        {
            // Process fixed timestep logic
            LunyLogger.LogInfo($"[LunyScriptRunner] OnFixedStep(fixedDeltaTime={fixedDeltaTime:0.000})");
        }

        public void OnShutdown()
        {
            // Cleanup runner systems
            LunyLogger.LogInfo("[LunyScriptRunner] OnShutdown()");
        }
    }
}
