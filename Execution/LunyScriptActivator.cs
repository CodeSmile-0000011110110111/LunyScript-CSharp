using Luny;
using Luny.Engine.Bridge;
using LunyScript.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace LunyScript.Execution
{
	/// <summary>
	/// Scans scenes at runtime to discover objects that should run LunyScripts.
	/// Binds scripts to objects based on name matching (exact, case-sensitive).
	/// </summary>
	internal static class LunyScriptActivator
	{
		/// <summary>
		/// Processes the current scene, finding objects and binding them to scripts.
		/// Creates run contexts for matching object-script pairs.
		/// </summary>
		/// <param name="lunyObjects"></param>
		/// <param name="scriptRegistry"></param>
		/// <param name="contextRegistry"></param>
		public static IReadOnlyList<LunyScriptContext> CreateContexts(IEnumerable<ILunyObject> lunyObjects,
			LunyScriptDefinitionRegistry scriptRegistry, LunyScriptContextRegistry contextRegistry)
		{
			var createdContexts = new List<LunyScriptContext>();

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
					var context = contextRegistry.CreateContext(scriptDef, lunyObject);
					createdContexts.Add(context);
				}
			}

			LunyLogger.LogInfo(
				$"{createdContexts.Count} {nameof(LunyScriptContext)}s created from {lunyObjects.Count()} {nameof(LunyObject)}s.",
				nameof(LunyScriptActivator));

			return createdContexts;
		}

		public static void BuildAndActivateLunyScripts(LunyScriptRunner scriptRunner, IEnumerable<ILunyObject> lunyObjects)
		{
			var sw = Stopwatch.StartNew();

			var objectEventHandler = scriptRunner.ObjectLifecycle;
			var sceneEventHandler = scriptRunner.SceneEventHandler;

			var activatedCount = 0;
			var scriptContexts = CreateContexts(lunyObjects, scriptRunner.Scripts, scriptRunner.Contexts);
			foreach (var context in scriptContexts)
			{
				BuildAndActivateLunyScript(context, objectEventHandler, sceneEventHandler);
				activatedCount++;
			}

			ActivateScripts(scriptContexts);

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Built {activatedCount} script(s) in {ms} ms", nameof(LunyScriptActivator));
		}

		public static void BuildAndActivateLunyScript(LunyScriptContext context, LunyScriptObjectLifecycle objectLifecycle,
			LunyScriptSceneEventHandler sceneEventHandler)
		{
			try
			{
				LunyLogger.LogInfo($"Building {context} ...", nameof(LunyScriptActivator));

				// Create script instance, initialize with context, and call Build()
				var scriptInstance = (LunyScript)Activator.CreateInstance(context.ScriptType);
				scriptInstance.Initialize(context);
				scriptInstance.Build();
				scriptInstance.Destroy();

				// hook up events
				objectLifecycle.Register(context);
				sceneEventHandler.Register(context);
			}
			catch (Exception ex)
			{
				LunyLogger.LogException(ex, nameof(LunyScriptActivator));
				throw;
			}
		}

		public static void ActivateScripts(IEnumerable<LunyScriptContext> contexts)
		{
			// sends initial OnCreate and (if enabled) OnEnable events
			foreach (var context in contexts)
				context.Activate();
		}
	}
}
