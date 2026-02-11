using Luny;
using Luny.Engine;
using Luny.Engine.Bridge;
using Luny.Engine.Services;
using LunyScript.Activation;
using LunyScript.Blocks;
using LunyScript.Diagnostics;
using LunyScript.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LunyScript
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
		[NotNull] private ScriptDefinitionRegistry _scripts;
		[NotNull] private ScriptRuntimeContextRegistry _contexts;
		[NotNull] private ScriptLifecycle _scriptLifecycle;
		[NotNull] private ScriptSceneEventHandler _sceneEventHandler;

		private ILunyTimeService _engineTime;
		private Table.VarHandle gvar_Time_HeartbeatCount;
		private Table.VarHandle gvar_Time_FrameCount;
		private Table.VarHandle gvar_Time_ElapsedSeconds;

		internal ScriptDefinitionRegistry Scripts => _scripts;
		internal ScriptRuntimeContextRegistry Contexts => _contexts;
		internal ScriptLifecycle ScriptLifecycle => _scriptLifecycle;
		internal ScriptSceneEventHandler SceneEventHandler => _sceneEventHandler;

		internal static void Run(IEnumerable<IScriptSequenceBlock> sequences, ScriptRuntimeContext runtimeContext)
		{
			if (sequences == null)
				return;

			foreach (var sequence in sequences)
				Run(sequence, runtimeContext);
		}

		internal static void Run(IScriptSequenceBlock sequence, ScriptRuntimeContext runtimeContext)
		{
			if (sequence == null)
				return;

			// TODO: avoid profiling overhead when not enabled
			var timeService = LunyEngine.Instance.Time;
			var blockType = sequence.GetType();
			var trace = new ScriptExecutionTrace
			{
				FrameCount = timeService?.FrameCount ?? -1,
				ElapsedSeconds = timeService?.ElapsedSeconds ?? -1.0,
				ScriptBlockId = sequence.ID,
				BlockType = blockType,
				BlockDescription = sequence.ToString(),
			};

			runtimeContext.DebugHooks.NotifyBlockExecute(trace);
			runtimeContext.BlockProfiler.BeginBlock(sequence.ID);

			try
			{
				sequence.Execute(runtimeContext);
			}
			catch (Exception ex)
			{
				runtimeContext.BlockProfiler.RecordError(sequence.ID, ex);
				trace.Error = ex;
				runtimeContext.DebugHooks.NotifyBlockError(trace);
				//LunyLogger.LogError(ex.ToString(), context);
				throw;
			}
			finally
			{
				runtimeContext.BlockProfiler.EndBlock(sequence.ID, blockType);
				runtimeContext.DebugHooks.NotifyBlockComplete(trace);
			}
		}

		public LunyScriptRunner()
		{
			LunyTraceLogger.LogInfoInitializing(this);

			ScriptDefID.Reset();
			ScriptBlockID.Reset();
			_scriptEngine = new LunyScriptEngine(this); // public API interface (split to ensure users don't call OnStartup etc)
			_scripts = new ScriptDefinitionRegistry(); // performs LunyScript type discovery
			_contexts = new ScriptRuntimeContextRegistry();
			_scriptLifecycle = new ScriptLifecycle(_contexts);
			_sceneEventHandler = new ScriptSceneEventHandler(_contexts);
			_engineTime = LunyEngine.Instance.Time;

			LunyTraceLogger.LogInfoInitialized(this);
		}

		public void OnEngineStartup()
		{
			try
			{
				LunyTraceLogger.LogInfoStartingUp(this);

				var gvars = ScriptRuntimeContext.GetGlobalVariables();
				gvar_Time_HeartbeatCount = gvars.GetHandle("Time.HeartbeatCount");
				gvar_Time_FrameCount = gvars.GetHandle("Time.FrameCount");
				gvar_Time_ElapsedSeconds = gvars.GetHandle("Time.ElapsedSeconds");
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

			ScriptBuilder.BuildAndActivateLunyScripts(this, newScriptedObjects);
		}

		public void OnObjectRegistered(ILunyObject lunyObject)
		{
			LunyLogger.LogInfo($"{nameof(OnObjectRegistered)}: {lunyObject}");

			// Check if object is already scripted to avoid double activation
			if (_contexts.GetByNativeObjectID(lunyObject.NativeObjectID) != null)
				return;

			// Activate script on dynamically created object
			ScriptBuilder.BuildAndActivateLunyScripts(this, new[] { lunyObject });
		}

		public void OnObjectUnregistered(ILunyObject lunyObject)
		{
			LunyLogger.LogInfo($"{nameof(OnObjectUnregistered)}: {lunyObject}");

			// Cleanup context for destroyed object
			var context = _contexts.GetByNativeObjectID(lunyObject.NativeObjectID);
			if (context != null)
			{
				// _sceneEventHandler.Unregister(context);
				_contexts.Unregister(context);
			}
		}

		public void OnSceneUnloaded(ILunyScene unloadedScene) => _sceneEventHandler.OnSceneUnloaded(unloadedScene);

		//public void OnEngineFrameBegins() {}
		//public void OnEngineFrameEnds() {}

		public void OnEngineHeartbeat()
		{
			gvar_Time_ElapsedSeconds.Value = _engineTime.ElapsedSeconds;
			gvar_Time_HeartbeatCount.Value = _engineTime.HeartbeatCount;

			foreach (var context in _contexts.AllContexts)
				_scriptLifecycle.OnHeartbeat(context);
		}

		public void OnEngineFrameUpdate()
		{
			gvar_Time_ElapsedSeconds.Value = _engineTime.ElapsedSeconds;
			gvar_Time_FrameCount.Value = _engineTime.FrameCount;

			foreach (var context in _contexts.AllContexts)
				_scriptLifecycle.OnFrameUpdate(context);
		}

		public void OnEngineFrameLateUpdate()
		{
			// Run all LateUpdate runnables
			foreach (var context in _contexts.AllContexts)
				_scriptLifecycle.OnFrameLateUpdate(context);
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
				_scriptLifecycle.Shutdown();
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
				_scriptLifecycle = null;

				LunyTraceLogger.LogInfoShutdownComplete(this);
			}
		}

		~LunyScriptRunner() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
