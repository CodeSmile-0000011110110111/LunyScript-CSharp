using Luny;
using Luny.Diagnostics;
using Luny.Proxies;
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
	internal sealed class LunyScriptRunner : IEngineLifecycleObserver
	{
		private LunyScriptEngine _scriptEngine;
		private ScriptDefinitionRegistry _scripts;
		private ScriptContextRegistry _contexts;
		private ScriptLifecycle _lifecycle;

		internal ScriptDefinitionRegistry Scripts => _scripts;
		internal ScriptContextRegistry Contexts => _contexts;

		internal static void RunAllOnReadyRunners(ScriptContext context)
		{
			RunAll(context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnReady), context);
			context.DidRunOnReady = true;
		}

		internal static void RunAll(IEnumerable<IRunnable> runnables, ScriptContext context)
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

		public LunyScriptRunner()
		{
			LunyLogger.LogInfo("Initializing...", this);

			// Instantiate public API singleton
			_scriptEngine = new LunyScriptEngine(this);

			_scripts = new ScriptDefinitionRegistry(); // performs LunyScript type discovery
			_contexts = new ScriptContextRegistry();
			_lifecycle = new ScriptLifecycle(_contexts);

			LunyLogger.LogInfo("Initialization complete.", this);
		}

		public void OnStartup()
		{
			LunyLogger.LogInfo($"{nameof(OnStartup)} running...", this);

			// Process current scene to bind scripts to objects
			var sceneObjects = LunyEngine.Instance.Scene.GetAllObjects();
			BuildAndActivateLunyScriptObjects(sceneObjects);

			LunyLogger.LogInfo($"{nameof(OnStartup)} complete.", this);
		}

		public void OnFixedStep(Double fixedDeltaTime)
		{
			foreach (var context in _contexts.AllContexts)
				_lifecycle.OnFixedStep(fixedDeltaTime, context);
		}

		public void OnUpdate(Double deltaTime)
		{
			foreach (var context in _contexts.AllContexts)
				_lifecycle.OnUpdate(deltaTime, context);
		}

		public void OnLateUpdate(Double deltaTime)
		{
			// Run all LateUpdate runnables
			foreach (var context in _contexts.AllContexts)
				_lifecycle.OnLateUpdate(deltaTime, context);

			// Structural changes pass: destroy queued objects
			_lifecycle.ProcessPendingDestroy();
		}

		public void OnShutdown()
		{
			LunyLogger.LogInfo($"{nameof(OnShutdown)}...", this);

			// ensure all objects run their OnDestroy
			foreach (var context in _contexts.AllContexts)
				context.LunyObject.Destroy();

			// final cleanup of pending object destroy
			foreach (var _ in _contexts.AllContexts)
				_lifecycle.ProcessPendingDestroy();

			_contexts?.Clear();
			_scripts?.Clear();
			_lifecycle = null;
			_scriptEngine.Shutdown();
			_scriptEngine = null;
			LunyLogger.LogInfo($"{nameof(OnShutdown)} complete.", this);
		}

		~LunyScriptRunner() => LunyLogger.LogInfo($"finalized {GetHashCode()}", this);

		private void BuildAndActivateLunyScriptObjects(IReadOnlyList<ILunyObject> sceneObjects)
		{
			var createdContexts = ScriptActivator.CreateContexts(sceneObjects, _scripts, _contexts);
			ScriptActivator.BuildScripts(createdContexts, _lifecycle);
			ScriptActivator.ActivateScripts(createdContexts);
		}
	}
}
