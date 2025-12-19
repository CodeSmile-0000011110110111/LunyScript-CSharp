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
	/// Manages script discovery, object binding, and run context lifecycle.
	/// </summary>
	public sealed class LunyScriptRunner : IEngineLifecycleObserver
	{
		private ScriptRegistry _scriptRegistry;
		private RunContextRegistry _contextRegistry;
		private ScenePreprocessor _scenePreprocessor;
		private Variables _globalVariables;

		public void OnStartup()
		{
			LunyLogger.LogInfo("LunyScriptRunner starting up...", this);

			// Initialize global variables and registries
			_globalVariables = new Variables();
			_scriptRegistry = new ScriptRegistry();
			_contextRegistry = new RunContextRegistry();

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
			_scenePreprocessor = new ScenePreprocessor(_scriptRegistry, _contextRegistry, sceneService, _globalVariables);

			// Process current scene to bind scripts to objects
			_scenePreprocessor.ProcessCurrentScene();

			// Initialize all scripts by calling Build()
			ActivateScripts();

			LunyLogger.LogInfo("LunyScriptRunner initialized successfully", this);
		}

		public void OnUpdate(Double deltaTime)
		{
			// Remove any contexts for destroyed objects
			_contextRegistry.RemoveInvalidContexts();

			// Execute all Update runnables
			foreach (var context in _contextRegistry.AllContexts)
			{
				foreach (var runnable in context.UpdateRunnables)
				{
					// TODO Step 2: Debug hooks
					// context.DebugHooks.OnBlockExecute?.Invoke(runnable, context);

					runnable.Execute(context);

					// TODO Step 2: Debug hooks
					// context.DebugHooks.OnBlockComplete?.Invoke(runnable, context);
				}
			}
		}

		public void OnLateUpdate(Double deltaTime)
		{
			// Execute all LateUpdate runnables
			foreach (var context in _contextRegistry.AllContexts)
			{
				foreach (var runnable in context.LateUpdateRunnables)
				{
					// TODO Step 2: Debug hooks
					// context.DebugHooks.OnBlockExecute?.Invoke(runnable, context);

					runnable.Execute(context);

					// TODO Step 2: Debug hooks
					// context.DebugHooks.OnBlockComplete?.Invoke(runnable, context);
				}
			}
		}

		public void OnFixedStep(Double fixedDeltaTime)
		{
			// Execute all FixedStep runnables
			foreach (var context in _contextRegistry.AllContexts)
			{
				foreach (var runnable in context.FixedStepRunnables)
				{
					// TODO Step 2: Debug hooks
					// context.DebugHooks.OnBlockExecute?.Invoke(runnable, context);

					runnable.Execute(context);

					// TODO Step 2: Debug hooks
					// context.DebugHooks.OnBlockComplete?.Invoke(runnable, context);
				}
			}
		}

		public void OnShutdown()
		{
			LunyLogger.LogInfo("LunyScriptRunner shutting down...", this);

			// Cleanup
			_contextRegistry?.Clear();
			_scriptRegistry?.Clear();
			_globalVariables?.Clear();
			_scenePreprocessor = null;
		}

		private void ActivateScripts()
		{
			var sw = Stopwatch.StartNew();

			var activatedCount = 0;
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

					// Create script instance, initialize with context, and call Build()
					var scriptInstance = (LunyScript)Activator.CreateInstance(scriptDef.Type);
					scriptInstance.Initialize(context);
					scriptInstance.Build();
					activatedCount++;

					LunyLogger.LogInfo($"Built script: {scriptDef.Name} for {context.Object}", this);
				}
				catch (Exception ex)
				{
					LunyLogger.LogException(ex);
				}
			}

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Activated {activatedCount} script(s) in {ms} ms", this);
		}
	}
}
