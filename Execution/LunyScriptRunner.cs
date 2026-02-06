using Luny;
using Luny.Engine;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Diagnostics;
using LunyScript.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

		internal static void Run(IEnumerable<IScriptSequenceBlock> sequences, LunyScriptContext context)
		{
			foreach (var sequence in sequences)
				Run(sequence, context);
		}

		private static void Run(IScriptSequenceBlock sequence, LunyScriptContext context)
		{
			// TODO: avoid profiling overhead when not enabled
			var timeService = LunyEngine.Instance.Time;
			var blockType = sequence.GetType();
			var trace = new LunyScriptExecutionTrace
			{
				FrameCount = timeService?.FrameCount ?? -1,
				ElapsedSeconds = timeService?.ElapsedSeconds ?? -1.0,
				LunyScriptRunID = sequence.ID,
				BlockType = blockType,
				BlockDescription = sequence.ToString(),
			};

			context.DebugHooks.NotifyBlockExecute(trace);
			context.BlockProfiler.BeginBlock(sequence.ID);

			try
			{
				sequence.Execute(context);
			}
			catch (Exception ex)
			{
				context.BlockProfiler.RecordError(sequence.ID, ex);
				trace.Error = ex;
				context.DebugHooks.NotifyBlockError(trace);
				//LunyLogger.LogError(ex.ToString(), context);
				throw;
			}
			finally
			{
				context.BlockProfiler.EndBlock(sequence.ID, blockType);
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

		public void OnEngineShutdown() => Shutdown();

		public void OnSceneLoaded(ILunyScene loadedScene)
		{
			_sceneEventHandler.OnSceneLoaded(loadedScene);

			// Process current scene to bind scripts to objects
			var lunyEngine = LunyEngine.Instance;
			var scriptNames = _scripts.GetNames();
			var scriptedObjects = lunyEngine.Scene.GetObjects(scriptNames);

			// Filter out objects that already have a context to avoid double activation
			var newScriptedObjects = scriptedObjects.Where(obj => _contexts.GetByNativeObjectID(obj.NativeObjectID) == null).ToList();

			LunyScriptActivator.BuildAndActivateLunyScripts(this, newScriptedObjects);
		}

		public void OnObjectCreated(ILunyObject lunyObject)
		{
			// Check if object is already scripted to avoid double activation
			if (_contexts.GetByNativeObjectID(lunyObject.NativeObjectID) != null)
				return;

			// Activate script on dynamically created object
			LunyScriptActivator.BuildAndActivateLunyScripts(this, new[] { lunyObject });
		}

		public void OnObjectDestroyed(ILunyObject lunyObject)
		{
			// Cleanup context for destroyed object
			var context = _contexts.GetByNativeObjectID(lunyObject.NativeObjectID);
			if (context != null)
			{
				// _objectEventHandler.Unregister(context);
				// _sceneEventHandler.Unregister(context);
				_contexts.Unregister(context);
			}
		}

		public void OnSceneUnloaded(ILunyScene unloadedScene) => _sceneEventHandler.OnSceneUnloaded(unloadedScene);

		public void OnEngineHeartbeat(Double fixedDeltaTime)
		{
			foreach (var context in _contexts.AllContexts)
				_objectEventHandler.OnFixedStep(fixedDeltaTime, context);
		}

		public void OnEngineFrameUpdate(Double deltaTime)
		{
			foreach (var context in _contexts.AllContexts)
				_objectEventHandler.OnUpdate(deltaTime, context);
		}

		public void OnEngineFrameLateUpdate(Double deltaTime)
		{
			// Run all LateUpdate runnables
			foreach (var context in _contexts.AllContexts)
				_objectEventHandler.OnLateUpdate(deltaTime, context);
		}

		internal void Shutdown()
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
			catch (Exception)
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

		~LunyScriptRunner() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
