using Luny;
using Luny.Diagnostics;
using LunyScript.Diagnostics;
using System;
using System.Collections.Generic;

namespace LunyScript.Execution
{
	/// <summary>
	/// Engine-agnostic script execution runner.
	/// Implements the lifecycle contract and will be discovered/reflected by
	/// the <see cref="Luny.LunyEngine"/> at startup.
	/// Manages script discovery, object binding, and run context lifecycle.
	/// </summary>
	internal sealed class ScriptRunner : IEngineLifecycleObserver
	{
		private LunyScriptEngine _scriptEngine;
		private ScriptDefinitionRegistry _scripts;
		private ScriptContextRegistry _contexts;
		private ObjectLifecycleManager _lifecycleManager;

		internal ScriptDefinitionRegistry Scripts => _scripts;
		internal ScriptContextRegistry Contexts => _contexts;
		internal ObjectLifecycleManager LifecycleManager => _lifecycleManager;

		internal static void RunAllOnCreateRunners(ScriptContext context) =>
			RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnCreate), context);

		internal static void RunAllOnDestroyRunners(ScriptContext context)
		{
			RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnDestroy), context);
			context.DidRunOnDestroy = true;
		}

		internal static void RunAllOnReadyRunners(ScriptContext context)
		{
			RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnReady), context);
			context.DidRunOnReady = true;
		}

		internal static void RunAllOnEnableRunners(ScriptContext context) =>
			RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnEnable), context);

		internal static void RunAllOnDisableRunners(ScriptContext context) =>
			RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnDisable), context);

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

		public ScriptRunner()
		{
			LunyLogger.LogInfo($"{nameof(ScriptRunner)} ctor runs", this);

			// Instantiate public API singleton
			_scriptEngine = new LunyScriptEngine(this);

			// Initialize registries and lifecycle manager
			_scripts = new ScriptDefinitionRegistry();
			_contexts = new ScriptContextRegistry();
			_lifecycleManager = new ObjectLifecycleManager(_contexts);
		}

		public void OnStartup()
		{
			LunyLogger.LogInfo($"{nameof(ScriptRunner)} {nameof(OnStartup)}", this);

			// Process current scene to bind scripts to objects
			_scripts.DiscoverScripts();
			ScriptActivator.RegisterObjects(LunyEngine.Instance.Scene.GetAllObjects(), this);
			ScriptActivator.BuildScripts(_contexts.AllContexts, this);

			LunyLogger.LogInfo($"{nameof(ScriptRunner)} initialization complete", this);
		}

		public void OnFixedStep(Double fixedDeltaTime)
		{
			// Run all FixedStep runnables
			foreach (var context in _contexts.AllContexts)
			{
				if (!context.DidRunOnReady)
					RunAllOnReadyRunners(context);

				RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnFixedStep), context);
			}
		}

		public void OnUpdate(Double deltaTime)
		{
			// Run all Update runnables
			foreach (var context in _contexts.AllContexts)
			{
				if (!context.DidRunOnReady)
					RunAllOnReadyRunners(context);

				RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnUpdate), context);
			}
		}

		public void OnLateUpdate(Double deltaTime)
		{
			// Run all LateUpdate runnables
			foreach (var context in _contexts.AllContexts)
				RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnLateUpdate), context);

			// Structural changes pass: destroy queued objects
			_lifecycleManager.OnEndOfFrame();
		}

		public void OnShutdown()
		{
			foreach (var context in _contexts.AllContexts)
			{
				if (!context.DidRunOnDestroy)
					RunAllOnDestroyRunners(context);

				_lifecycleManager.Unregister(context);
			}

			LunyLogger.LogInfo("LunyScriptRunner shutting down...", this);
			_contexts?.Clear();
			_scripts?.Clear();
			_lifecycleManager = null;
			_scriptEngine.Shutdown();
			_scriptEngine = null;
		}
	}
}
