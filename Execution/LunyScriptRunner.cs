using Luny;
using Luny.Engine;
using Luny.Engine.Bridge;
using LunyScript.Diagnostics;
using LunyScript.Events;
using LunyScript.Runnables;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript.Execution
{
	/// <summary>
	/// Engine-agnostic script execution runner.
	/// Implements the lifecycle contract and will be discovered/reflected by
	/// the <see cref="LunyEngine"/> at startup.
	/// Manages script discovery, object binding, and run context lifecycle.
	/// </summary>
	internal sealed class LunyScriptRunner : ILunyEngineObserver
	{
		[NotNull] private LunyScriptEngine _scriptEngine;
		[NotNull] private LunyScriptDefinitionRegistry _scripts;
		[NotNull] private LunyScriptContextRegistry _contexts;
		[NotNull] private LunyScriptObjectEventHandler _objectEventHandler;
		[NotNull] private LunyScriptSceneEventHandler _sceneEventHandler;

		internal LunyScriptDefinitionRegistry Scripts => _scripts;
		internal LunyScriptContextRegistry Contexts => _contexts;
		internal LunyScriptObjectEventHandler ObjectEventHandler => _objectEventHandler;
		internal LunyScriptSceneEventHandler SceneEventHandler => _sceneEventHandler;

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
			_objectEventHandler = new LunyScriptObjectEventHandler(_contexts);
			_sceneEventHandler = new LunyScriptSceneEventHandler(_contexts);

			LunyTraceLogger.LogInfoInitialized(this);
		}

		public void OnEngineStartup()
		{
			try
			{
				LunyTraceLogger.LogInfoStartingUp(this);
			}
			catch (Exception)
			{
				LunyLogger.LogError($"Error during {nameof(LunyEngine)} {nameof(OnEngineStartup)}!", this);
				throw;
			}
			finally
			{
				LunyTraceLogger.LogInfoStartupComplete(this);
			}
		}

		public void OnEngineShutdown()
		{
			try
			{
				LunyTraceLogger.LogInfoShuttingDown(this);

				// ensure all objects run their OnDestroy
				foreach (var context in _contexts.AllContexts)
					context.LunyObject.Destroy();

				// final cleanup of pending object destroy
				_objectEventHandler.Shutdown();
				_sceneEventHandler.Shutdown();
				_contexts.Shutdown();
				_scripts.Shutdown();
				_scriptEngine.Shutdown();
			}
			catch (Exception e)
			{
				LunyLogger.LogError($"Error during {nameof(LunyScriptRunner)} {nameof(OnEngineShutdown)}!", this);
				throw;
			}
			finally
			{
				_scriptEngine = null;
				_objectEventHandler = null;

				LunyTraceLogger.LogInfoShutdownComplete(this);
			}
		}

		public void OnSceneLoaded(ILunyScene loadedScene)
		{
			_sceneEventHandler.OnSceneLoaded(loadedScene);

			// Process current scene to bind scripts to objects
			LunyEngine.Instance.Scene.GetAllObjects(); // triggers registration in LunyObjectRegistry
			LunyScriptActivator.BuildAndActivateLunyScripts(this);
		}

		public void OnSceneUnloaded(ILunyScene unloadedScene) => _sceneEventHandler.OnSceneUnloaded(unloadedScene);

		public void OnEngineFixedStep(Double fixedDeltaTime)
		{
			foreach (var context in _contexts.AllContexts)
				_objectEventHandler.OnFixedStep(fixedDeltaTime, context);
		}

		public void OnEngineUpdate(Double deltaTime)
		{
			foreach (var context in _contexts.AllContexts)
				_objectEventHandler.OnUpdate(deltaTime, context);
		}

		public void OnEngineLateUpdate(Double deltaTime)
		{
			// Run all LateUpdate runnables
			foreach (var context in _contexts.AllContexts)
				_objectEventHandler.OnLateUpdate(deltaTime, context);
		}

		~LunyScriptRunner() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
