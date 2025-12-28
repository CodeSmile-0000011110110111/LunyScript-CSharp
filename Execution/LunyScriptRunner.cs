using Luny;
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
		private ILunyScriptEngine _scriptEngine;
		private ScriptRegistry _scriptRegistry;
		private ScriptContextRegistry _contextRegistry;
		private ScenePreprocessor _scenePreprocessor;

		internal ScriptRegistry Scripts => _scriptRegistry;
		internal ScriptContextRegistry Contexts => _contextRegistry;

		public LunyScriptRunner()
		{
			LunyLogger.LogInfo($"{nameof(LunyScriptRunner)} ctor runs", this);

			// Instantiate public API singleton
			_scriptEngine = new LunyScriptEngine(this);

			// Initialize registries
			_scriptRegistry = new ScriptRegistry();
			_contextRegistry = new ScriptContextRegistry();
			_scenePreprocessor = new ScenePreprocessor(this);
		}

		public void OnStartup()
		{
			LunyLogger.LogInfo($"{nameof(LunyScriptRunner)} {nameof(OnStartup)}", this);

			// Process current scene to bind scripts to objects
			_scriptRegistry.DiscoverScripts();
			_scenePreprocessor.ProcessSceneObjects();
			ActivateScripts();

			LunyLogger.LogInfo($"{nameof(LunyScriptRunner)} initialization complete", this);
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
			(_scriptEngine as LunyScriptEngine).Shutdown();
			_scriptEngine = null;
		}

		private void ExecuteRunnable(IRunnable runnable, ScriptContext context)
		{
			// TODO: avoid profiling overhead when not enabled
			var timeService = LunyEngine.Instance.Time;
			var blockType = runnable.GetType();
			var trace = new ExecutionTrace
			{
				FrameCount = timeService?.FrameCount ?? -1,
				ElapsedSeconds = timeService?.ElapsedSeconds ?? -1.0,
				RunnableID = runnable.ID,
				BlockType = blockType,
				BlockDescription = runnable.ToString(),
			};

			context.DebugHooks.NotifyBlockExecute(trace);
			context.BlockProfiler.BeginBlock(runnable.ID);

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
				var scriptDef = context.ScriptDef;
				try
				{
					// Create script instance, initialize with context, and call Build()
					var scriptInstance = (LunyScript)Activator.CreateInstance(scriptDef.Type);
					scriptInstance.Initialize(context);
					scriptInstance.Build();
					// script instance goes out of scope, they build runnables
					activatedCount++;

					LunyLogger.LogInfo($"Built: {scriptDef} for {context.EngineObject}", this);
				}
				catch (Exception ex)
				{
					LunyLogger.LogError($"{scriptDef} failed to build: {ex.Message}\n{ex.StackTrace}", this);
				}
			}

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Activated {activatedCount} script(s) in {ms} ms", this);
		}
	}
}
