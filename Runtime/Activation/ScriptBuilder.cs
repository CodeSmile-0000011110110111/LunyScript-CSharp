using Luny;
using Luny.Engine.Bridge;
using LunyScript.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace LunyScript.Activation
{
	/// <summary>
	/// Scans scenes at runtime to discover objects that should run LunyScripts.
	/// Binds scripts to objects based on name matching (exact, case-sensitive).
	/// </summary>
	internal static class ScriptBuilder
	{
		/// <summary>
		/// Processes the current scene, finding objects and binding them to scripts.
		/// Creates run contexts for matching object-script pairs.
		/// </summary>
		/// <param name="lunyObjects"></param>
		/// <param name="scriptRegistry"></param>
		/// <param name="runtimeContextRegistry"></param>
		public static IReadOnlyList<ScriptRuntimeContext> CreateRuntimeContexts(IEnumerable<ILunyObject> lunyObjects,
			ScriptDefinitionRegistry scriptRegistry, ScriptRuntimeContextRegistry runtimeContextRegistry)
		{
			var createdContexts = new List<ScriptRuntimeContext>();

			foreach (var lunyObject in lunyObjects)
			{
				if (lunyObject == null || !lunyObject.IsValid)
					continue;

				var objectName = lunyObject.Name;

				// Check if we have a script matching this object's name
				var scriptDef = scriptRegistry.GetByName(objectName);
				if (scriptDef != null)
				{
					// Create ScriptContext for this object-script pair
					var context = runtimeContextRegistry.CreateContext(scriptDef, lunyObject);
					createdContexts.Add(context);
				}
			}

			LunyLogger.LogInfo(
				$"{createdContexts.Count} {nameof(ScriptRuntimeContext)}s created from {lunyObjects.Count()} {nameof(LunyObject)}s.",
				nameof(ScriptBuilder));

			return createdContexts;
		}

		public static void BuildAndActivateLunyScripts(LunyScriptRunner scriptRunner, IEnumerable<ILunyObject> lunyObjects)
		{
			var sw = Stopwatch.StartNew();

			var objectEventHandler = scriptRunner.ObjectLifecycle;
			var sceneEventHandler = scriptRunner.SceneEventHandler;

			var activatedCount = 0;
			var buildContext = new ScriptBuildContext();
			var runtimeContexts = CreateRuntimeContexts(lunyObjects, scriptRunner.Scripts, scriptRunner.Contexts);
			foreach (var runtimeContext in runtimeContexts)
			{
				BuildAndActivateLunyScript(buildContext, runtimeContext, objectEventHandler, sceneEventHandler);
				activatedCount++;
			}

			ActivateScripts(runtimeContexts);

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Built {activatedCount} script(s) in {ms} ms", nameof(ScriptBuilder));
		}

		public static void BuildAndActivateLunyScript(ScriptBuildContext buildContext, ScriptRuntimeContext runtimeContext,
			ScriptObjectLifecycle objectLifecycle, ScriptSceneEventHandler sceneEventHandler)
		{
			try
			{
				LunyLogger.LogInfo($"Building {runtimeContext} ...", nameof(ScriptBuilder));

				// Create script instance, initialize with context, and call Build()
				var scriptInstance = (LunyScript)Activator.CreateInstance(runtimeContext.ScriptType);
				scriptInstance.Initialize(runtimeContext);
				scriptInstance.Build(buildContext);
				scriptInstance.Destroy();

				// hook up events
				objectLifecycle.Register(runtimeContext);
				sceneEventHandler.Register(runtimeContext);
			}
			catch (Exception ex)
			{
				LunyLogger.LogException(ex, nameof(ScriptBuilder));
				throw;
			}
		}

		public static void ActivateScripts(IEnumerable<ScriptRuntimeContext> contexts)
		{
			// sends initial OnCreate and (if enabled) OnEnable events
			foreach (var context in contexts)
				context.Activate();
		}
	}
}
