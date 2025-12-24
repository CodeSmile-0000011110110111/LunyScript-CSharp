using Luny.Diagnostics;
using Luny.Interfaces;
using LunyScript.Diagnostics;
using LunyScript.Interfaces;
using LunyScript.Registries;
using System;
using System.Diagnostics;

namespace LunyScript.Execution
{
	/// <summary>
	/// Engine-agnostic script execution runner.
	/// Implements the lifecycle contract and will be discovered/reflected by
	/// the <see cref="Luny.LunyEngine"/> at startup.
	/// Manages script discovery, object binding, and run context lifecycle.
	/// </summary>
	internal sealed class LunyScriptRunner : IEngineLifecycleObserver
	{
		private ILunyEngine _engine;
		private ScriptRegistry _scriptRegistry;
		private ScriptContextRegistry _contextRegistry;
		private ScenePreprocessor _scenePreprocessor;

		internal ILunyEngine Engine => _engine;
		internal ScriptRegistry Scripts => _scriptRegistry;
		internal ScriptContextRegistry Contexts => _contextRegistry;

		public void OnStartup(ILunyEngine engine)
		{
			LunyLogger.LogInfo("LunyScriptRunner starting up...", this);
			_engine = engine;

			// Initialize registries
			_scriptRegistry = new ScriptRegistry();
			_contextRegistry = new ScriptContextRegistry();
			_scenePreprocessor = new ScenePreprocessor(this);

			// Process current scene to bind scripts to objects
			_scriptRegistry.DiscoverScripts();
			_scenePreprocessor.ProcessSceneObjects();
			ActivateScripts();

			LunyLogger.LogInfo("LunyScriptRunner initialized successfully", this);
		}

		public void OnFixedStep(Double fixedDeltaTime)
		{
			// Execute all FixedStep runnables
			foreach (var context in _contextRegistry.AllContexts)
			{
				foreach (var runnable in context.RunnablesScheduledInFixedStep)
					ExecuteRunnable(runnable, context);
			}
		}

		public void OnUpdate(Double deltaTime)
		{
			// Execute all Update runnables
			foreach (var context in _contextRegistry.AllContexts)
			{
				foreach (var runnable in context.RunnablesScheduledInUpdate)
					ExecuteRunnable(runnable, context);
			}
		}

		public void OnLateUpdate(Double deltaTime)
		{
			// Execute all LateUpdate runnables
			foreach (var context in _contextRegistry.AllContexts)
			{
				foreach (var runnable in context.RunnablesScheduledInLateUpdate)
					ExecuteRunnable(runnable, context);
			}

			// Remove any contexts for destroyed objects
			_contextRegistry.RemoveInvalidContexts();
		}

		public void OnShutdown()
		{
			LunyLogger.LogInfo("LunyScriptRunner shutting down...", this);

			// Cleanup
			_contextRegistry?.Clear();
			_scriptRegistry?.Clear();
			_scenePreprocessor = null;
			_engine = null;
		}

		private void ExecuteRunnable(IRunnable runnable, ScriptContext context)
		{
			// TODO: avoid profiling overhead when not enabled
			var timeService = _engine.Time;
			var blockType = runnable.GetType().Name;
			var trace = new ExecutionTrace
			{
				FrameCount = timeService?.FrameCount ?? -1,
				ElapsedSeconds = timeService?.ElapsedSeconds ?? -1.0,
				RunnableID = runnable.ID,
				BlockType = blockType,
				BlockDescription = runnable.ToString(),
			};
			context.DebugHooks.NotifyBlockExecute(trace);
			context.BlockProfiler.BeginBlock(runnable.ID, blockType);

			try
			{
				runnable.Execute(context);
			}
			catch (Exception ex)
			{
				context.BlockProfiler.RecordError(runnable.ID, ex);
				trace.Error = ex;
				context.DebugHooks.NotifyBlockError(trace);
				LunyLogger.LogException(ex, context.EngineObject);
			}
			finally
			{
				context.BlockProfiler.EndBlock(runnable.ID, blockType);
				context.DebugHooks.NotifyBlockComplete(trace);
			}
		}

		private void ActivateScripts()
		{
			var sw = Stopwatch.StartNew();

			var activatedCount = 0;
			foreach (var context in _contextRegistry.AllContexts)
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

					LunyLogger.LogInfo($"Built script: {scriptDef.Name} for {context.EngineObject}", this);
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
