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
            Log.Info("[LunyScriptRunner] OnStartup()");
            // Initialize runner systems here (state machines, behavior trees, etc.)
        }

        public void OnUpdate(double deltaTime)
        {
            // Process per-frame logic
            Log.Info($"[LunyScriptRunner] OnUpdate(deltaTime={deltaTime:0.000})");
        }

        public void OnLateUpdate(Double deltaTime)
        {
	        Log.Info($"[LunyScriptRunner] OnLateUpdate(deltaTime={deltaTime:0.000})");
        }

        public void OnFixedStep(double fixedDeltaTime)
        {
            // Process fixed timestep logic
            Log.Info($"[LunyScriptRunner] OnFixedStep(fixedDeltaTime={fixedDeltaTime:0.000})");
        }

        public void OnShutdown()
        {
            // Cleanup runner systems
            Log.Info("[LunyScriptRunner] OnShutdown()");
        }
    }
}
