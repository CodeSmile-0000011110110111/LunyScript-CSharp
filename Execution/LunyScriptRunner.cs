using Luny;
using Luny.Diagnostics;
using LunyScript.Diagnostics;
using LunyScript.Registries;
using System;
using System.Collections.Generic;
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
		private LunyScriptEngine _scriptEngine;
		private ScriptDefinitionRegistry _scriptRegistry;
		private ScriptContextRegistry _contextRegistry;
		private ObjectLifecycleManager _lifecycleManager;
		private ScenePreprocessor _scenePreprocessor;

		internal ScriptDefinitionRegistry Scripts => _scriptRegistry;
		internal ScriptContextRegistry Contexts => _contextRegistry;
		internal ObjectLifecycleManager LifecycleManager => _lifecycleManager;

		internal static void RunObjectEnabled(ScriptContext context)
		{
			if (context.LunyObject.Enabled)
				RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnEnable), context);
		}

		internal static void RunObjectDisabled(ScriptContext context)
		{
			if (!context.LunyObject.Enabled)
				RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnDisable), context);
		}

		internal static void RunObjectDestroyed(ScriptContext context)
		{
			RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnDestroy), context);
			context.DidRunOnDestroy = true;
		}

		private static void RunAll(IEnumerable<IRunnable> runnables, ScriptContext context)
		{
			if (runnables == null || context == null)
				return;

			foreach (var runnable in runnables)
				Run(runnable, context);
		}

		private static void Run(IRunnable runnable, ScriptContext context)
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
				LunyLogger.LogError(ex.ToString(), context);
			}
			finally
			{
				context.BlockProfiler.EndBlock(runnable.ID, blockType);
				context.DebugHooks.NotifyBlockComplete(trace);
			}
		}

		private static void RunObjectCreated(ScriptContext context)
		{
			RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnCreate), context);
			RunObjectEnabled(context);
		}

		private static void RunObjectReady(ScriptContext context)
		{
			RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnReady), context);
			context.DidRunOnReady = true;
		}

		public LunyScriptRunner()
		{
			LunyLogger.LogInfo($"{nameof(LunyScriptRunner)} ctor runs", this);

			// Instantiate public API singleton
			_scriptEngine = new LunyScriptEngine(this);

			// Initialize registries and lifecycle manager
			_scriptRegistry = new ScriptDefinitionRegistry();
			_contextRegistry = new ScriptContextRegistry();
			_lifecycleManager = new ObjectLifecycleManager(_contextRegistry);
			_scenePreprocessor = new ScenePreprocessor(this);
		}

		public void OnStartup()
		{
			LunyLogger.LogInfo($"{nameof(LunyScriptRunner)} {nameof(OnStartup)}", this);

			// Process current scene to bind scripts to objects
			_scriptRegistry.DiscoverScripts();
			_scenePreprocessor.ProcessSceneObjects();
			ActivateScripts();

			// TODO: replace this and route it through the lifecycle manager
			// Run OnCreate and OnEnable
			foreach (var context in _contextRegistry.AllContexts)
				RunObjectCreated(context);

			LunyLogger.LogInfo($"{nameof(LunyScriptRunner)} initialization complete", this);
		}

		public void OnFixedStep(Double fixedDeltaTime)
		{
			// Run all FixedStep runnables
			foreach (var context in _contextRegistry.AllContexts)
			{
				if (!context.DidRunOnReady)
					RunObjectReady(context);

				RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnFixedStep), context);
			}
		}

		public void OnUpdate(Double deltaTime)
		{
			// Run all Update runnables
			foreach (var context in _contextRegistry.AllContexts)
			{
				if (!context.DidRunOnReady)
					RunObjectReady(context);

				RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnUpdate), context);
			}
		}

		public void OnLateUpdate(Double deltaTime)
		{
			// Run all LateUpdate runnables
			foreach (var context in _contextRegistry.AllContexts)
				RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnLateUpdate), context);

			// Structural changes pass: destroy queued objects
			_lifecycleManager.OnEndOfFrame();
		}

		public void OnShutdown()
		{
			foreach (var context in _contextRegistry.AllContexts)
			{
				if (!context.DidRunOnDestroy)
					RunObjectDestroyed(context);

				_lifecycleManager.UnregisterObject(context.LunyObject);
			}

			LunyLogger.LogInfo("LunyScriptRunner shutting down...", this);
			_contextRegistry?.Clear();
			_scriptRegistry?.Clear();
			_lifecycleManager = null;
			_scenePreprocessor = null;
			_scriptEngine.Shutdown();
			_scriptEngine = null;
		}

		private void ActivateScripts()
		{
			var sw = Stopwatch.StartNew();

			var activatedCount = 0;
			foreach (var context in _contextRegistry.AllContexts)
			{
				try
				{
					LunyLogger.LogInfo($"Building script {context.ScriptType.Name} for {context.LunyObject}", this);

					// Create script instance, initialize with context, and call Build()
					var scriptInstance = (LunyScript)Activator.CreateInstance(context.ScriptType);
					scriptInstance.Initialize(context);
					scriptInstance.Build();
					scriptInstance.Shutdown();
					activatedCount++;
				}
				catch (Exception ex)
				{
					LunyLogger.LogError($"{context.ScriptType} failed to build: {ex}", this);
					Debugger.Break();
				}
			}

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Activated {activatedCount} script(s) in {ms} ms", this);
		}
	}
}
