using Luny;
using Luny.Providers;
using System;
using System.Diagnostics;

namespace LunyScript
{
	/// <summary>
	/// Engine-agnostic script execution runner.
	/// Implements the lifecycle contract and will be discovered/reflected by
	/// the <see cref="Luny.LunyEngine"/> at startup.
	/// Manages script discovery, object binding, and execution context lifecycle.
	/// </summary>
	public sealed class LunyScriptRunner : IEngineLifecycleObserver
	{
		private ScriptRegistry _scriptRegistry;
		private ExecutionContextRegistry _contextRegistry;
		private ScenePreprocessor _scenePreprocessor;

		public void OnStartup()
		{
			LunyLogger.LogInfo("LunyScriptRunner starting up...", this);

			// Initialize registries
			_scriptRegistry = new ScriptRegistry();
			_contextRegistry = new ExecutionContextRegistry();

			// Discover all LunyScript subclasses via reflection
			_scriptRegistry.DiscoverScripts();

			// Get scene service for object discovery
			var sceneService = LunyEngine.Instance.GetService<ISceneServiceProvider>();
			if (sceneService == null)
			{
				LunyLogger.LogWarning("ISceneServiceProvider not available, script-to-object binding disabled", this);
				return;
			}

			// Initialize scene preprocessor
			_scenePreprocessor = new ScenePreprocessor(_scriptRegistry, _contextRegistry, sceneService);

			// Process current scene to bind scripts to objects
			_scenePreprocessor.ProcessCurrentScene();

			// Initialize all scripts by calling OnStartup()
			ActivateScripts();

			LunyLogger.LogInfo("LunyScriptRunner initialized successfully", this);
		}

		public void OnUpdate(Double deltaTime)
		{
			// Remove any contexts for destroyed objects
			_contextRegistry.RemoveInvalidContexts();

			// TODO: Execute sequences/FSMs/BTs for each context
			// For now, just log that we're processing contexts
			var contexts = _contextRegistry.AllContexts;
			if (contexts.Count > 0)
			{
				LunyLogger.LogInfo($"OnUpdate: Processing {contexts.Count} execution context(s)", this);
			}
		}

		public void OnLateUpdate(Double deltaTime)
		{

			// TODO: Execute late-update sequences
		}

		public void OnFixedStep(Double fixedDeltaTime)
		{

			// TODO: Execute fixed-step sequences
		}

		public void OnShutdown()
		{
			LunyLogger.LogInfo("LunyScriptRunner shutting down...", this);

			// Cleanup
			_contextRegistry?.Clear();
			_scriptRegistry?.Clear();
			_scenePreprocessor = null;
		}

		private void ActivateScripts()
		{
			var sw = Stopwatch.StartNew();

			var activatedScriptsCount = 0;
			var contexts = _contextRegistry.AllContexts;
			foreach (var context in contexts)
			{
				try
				{
					var scriptDef = _scriptRegistry.GetByID(context.ScriptID);
					if (scriptDef == null)
					{
						LunyLogger.LogWarning($"Script definition not found for {context.ScriptID}", this);
						continue;
					}

					// Create script instance and call OnStartup()
					var scriptInstance = (LunyScript)Activator.CreateInstance(scriptDef.Type);
					scriptInstance.OnStartup();
					activatedScriptsCount++;
					LunyLogger.LogInfo($"Initialized script: {scriptDef.Name} for object {context.Object}", this);
				}
				catch (Exception ex)
				{
					LunyLogger.LogException(ex);
				}
			}

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Activated {activatedScriptsCount} LunyScript(s) in {ms} ms.", this);
		}
	}
}
