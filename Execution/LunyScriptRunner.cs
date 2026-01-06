using Luny;
using Luny.Engine;
using LunyScript.Diagnostics;
using LunyScript.Runnables;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript.Execution
{
	/// <summary>
	/// Engine-agnostic script execution runner.
	/// Implements the lifecycle contract and will be discovered/reflected by
	/// the <see cref="Luny.LunyEngine"/> at startup.
	/// Manages script discovery, object binding, and run context lifecycle.
	/// </summary>
	internal sealed class LunyScriptRunner : ILunyEngineObserver
	{
		[NotNull] private LunyScriptEngine _scriptEngine;
		[NotNull] private LunyScriptDefinitionRegistry _scripts;
		[NotNull] private LunyScriptContextRegistry _contexts;
		[NotNull] private LunyScriptLifecycle _lifecycle;

		internal LunyScriptDefinitionRegistry Scripts => _scripts;
		internal LunyScriptContextRegistry Contexts => _contexts;
		public LunyScriptLifecycle Lifecycle => _lifecycle;

		internal static void Run(IEnumerable<ILunyScriptRunnable> runnables, LunyScriptContext context)
		{
			foreach (var runnable in runnables)
				Run(runnable, context);
		}

		private static void Run(ILunyScriptRunnable lunyScriptRunnable, LunyScriptContext context)
		{
			// TODO: avoid profiling overhead when not enabled
			var timeService = LunyEngine.Instance.Time;
			var blockType = lunyScriptRunnable.GetType();
			var trace = new LunyScriptExecutionTrace
			{
				FrameCount = timeService?.FrameCount ?? -1,
				ElapsedSeconds = timeService?.ElapsedSeconds ?? -1.0,
				LunyScriptRunID = lunyScriptRunnable.ID,
				BlockType = blockType,
				BlockDescription = lunyScriptRunnable.ToString(),
			};

			context.DebugHooks.NotifyBlockExecute(trace);
			context.BlockProfiler.BeginBlock(lunyScriptRunnable.ID);

			try
			{
				lunyScriptRunnable.Execute(context);
			}
			catch (Exception ex)
			{
				context.BlockProfiler.RecordError(lunyScriptRunnable.ID, ex);
				trace.Error = ex;
				context.DebugHooks.NotifyBlockError(trace);
				LunyLogger.LogError(ex.ToString(), context);
			}
			finally
			{
				context.BlockProfiler.EndBlock(lunyScriptRunnable.ID, blockType);
				context.DebugHooks.NotifyBlockComplete(trace);
			}
		}

		public LunyScriptRunner()
		{
			LunyTraceLogger.LogInfoInitializing(this);

			LunyScriptID.Reset();
			LunyScriptRunID.Reset();
			_scriptEngine = new LunyScriptEngine(this); // public API interface (split to ensure users don't call OnStartup etc)
			_scripts = new LunyScriptDefinitionRegistry(); // performs LunyScript type discovery
			_contexts = new LunyScriptContextRegistry();
			_lifecycle = new LunyScriptLifecycle(_contexts);

			LunyTraceLogger.LogInfoInitialized(this);
		}

		public void OnEngineStartup()
		{
			LunyTraceLogger.LogInfoStartingUp(this);

			// Process current scene to bind scripts to objects
			LunyEngine.Instance.Scene.GetAllObjects(); // triggers registration in LunyObjectRegistry
			LunyScriptActivator.BuildAndActivateLunyScripts(this);

			LunyTraceLogger.LogInfoStartupComplete(this);
		}

		public void OnEngineFixedStep(Double fixedDeltaTime)
		{
			foreach (var context in _contexts.AllContexts)
				_lifecycle.OnFixedStep(fixedDeltaTime, context);
		}

		public void OnEngineUpdate(Double deltaTime)
		{
			foreach (var context in _contexts.AllContexts)
				_lifecycle.OnUpdate(deltaTime, context);
		}

		public void OnEngineLateUpdate(Double deltaTime)
		{
			// Run all LateUpdate runnables
			foreach (var context in _contexts.AllContexts)
				_lifecycle.OnLateUpdate(deltaTime, context);
		}

		public void OnEngineShutdown()
		{
			LunyTraceLogger.LogInfoShuttingDown(this);

			// ensure all objects run their OnDestroy
			foreach (var context in _contexts.AllContexts)
				context.LunyObject.Destroy();

			// final cleanup of pending object destroy
			_lifecycle.Shutdown();
			_contexts.Shutdown();
			_scripts.Shutdown();
			_scriptEngine.Shutdown();

			_scriptEngine = null;
			_lifecycle = null;

			LunyTraceLogger.LogInfoShutdownComplete(this);
		}

		~LunyScriptRunner() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
