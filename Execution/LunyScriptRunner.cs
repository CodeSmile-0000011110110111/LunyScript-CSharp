using Luny;
using Luny.Engine.Registries;
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

		internal static void Run(IEnumerable<IRunnable> runnables, LunyScriptContext context)
		{
			foreach (var runnable in runnables)
				Run(runnable, context);
		}

		private static void Run(IRunnable runnable, LunyScriptContext context)
		{
			// TODO: avoid profiling overhead when not enabled
			var timeService = LunyEngine.Instance.Time;
			var blockType = runnable.GetType();
			var trace = new LunyScriptExecutionTrace
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

			LunyScriptID.Reset();
			RunnableID.Reset();
			_scriptEngine = new LunyScriptEngine(this); // public API interface (split to ensure users don't call OnStartup etc)
			_scripts = new LunyScriptDefinitionRegistry(); // performs LunyScript type discovery
			_contexts = new LunyScriptContextRegistry();
			_lifecycle = new LunyScriptLifecycle(_contexts);

			LunyLogger.LogInfo("Initialization complete.", this);
		}

		public void OnStartup()
		{
			LunyLogger.LogInfo($"{nameof(OnStartup)} running...", this);

			// Process current scene to bind scripts to objects
			LunyEngine.Instance.Scene.GetAllObjects(); // triggers registration in LunyObjectRegistry
			LunyScriptActivator.BuildAndActivateLunyScripts(this);

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
		}

		public void OnShutdown()
		{
			LunyLogger.LogInfo($"{nameof(OnShutdown)}...", this);

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
			LunyLogger.LogInfo($"{nameof(OnShutdown)} complete.", this);
		}

		//~LunyScriptRunner() => LunyLogger.LogInfo($"finalized {GetHashCode()}", this);
	}
}
