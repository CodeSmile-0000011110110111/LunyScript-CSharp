using Luny.Diagnostics;
using Luny.Proxies;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LunyScript.Execution
{
	/// <summary>
	/// Scans scenes at runtime to discover objects that should run LunyScripts.
	/// Binds scripts to objects based on name matching (exact, case-sensitive).
	/// </summary>
	internal static class ScriptActivator
	{
		/// <summary>
		/// Processes the current scene, finding objects and binding them to scripts.
		/// Creates run contexts for matching object-script pairs.
		/// </summary>
		/// <param name="lunyObjects"></param>
		/// <param name="scriptRegistry"></param>
		/// <param name="contextRegistry"></param>
		public static IReadOnlyList<ScriptContext> CreateContexts(IReadOnlyList<ILunyObject> lunyObjects,
			ScriptDefinitionRegistry scriptRegistry, ScriptContextRegistry contextRegistry)
		{
			var createdContexts = new List<ScriptContext>();

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

					//LunyLogger.LogInfo($"{scriptDef} => {lunyObject}", contextRegistry);
				}
			}

			LunyLogger.LogInfo($"{createdContexts.Count} {nameof(ScriptContext)}s created from {lunyObjects.Count} {nameof(LunyObject)}s.",
				nameof(ScriptActivator));

			return createdContexts;
		}

		public static void BuildAndActivateLunyScripts(IReadOnlyList<ILunyObject> sceneObjects, LunyScriptRunner scriptRunner)
		{
			var sw = Stopwatch.StartNew();

			var scriptContexts = CreateContexts(sceneObjects, scriptRunner.Scripts, scriptRunner.Contexts);

			var lifecycle = scriptRunner.Lifecycle;
			var activatedCount = 0;
			foreach (var context in scriptContexts)
			{
				try
				{
					LunyLogger.LogInfo($"Building {context} ...", nameof(ScriptActivator));

					// Create script instance, initialize with context, and call Build()
					var scriptInstance = (LunyScript)Activator.CreateInstance(context.ScriptType);
					scriptInstance.Initialize(context);
					scriptInstance.Build();
					scriptInstance.Shutdown();
					lifecycle.RegisterCallbacks(context); // hooks up lifecycle events
					activatedCount++;
				}
				catch (Exception ex)
				{
					LunyLogger.LogError($"{context.ScriptType} failed to build: {ex}", nameof(ScriptActivator));
					Debugger.Break();
				}
			}

			ActivateScripts(scriptContexts);

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Built {activatedCount} script(s) in {ms} ms", nameof(ScriptActivator));
		}

		public static void ActivateScripts(IReadOnlyList<ScriptContext> contexts)
		{
			// sends initial OnCreate and (if enabled) OnEnable events
			foreach (var context in contexts)
				context.Activate();
		}
	}
}
