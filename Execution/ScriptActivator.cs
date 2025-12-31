using Luny;
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
		/// <param name="sceneObjects"></param>
		/// <param name="scriptRunner"></param>
		public static void RegisterObjects(IReadOnlyList<ILunyObject> sceneObjects, ScriptRunner scriptRunner)
		{
			var scriptRegistry = scriptRunner.Scripts;
			var contextRegistry = scriptRunner.Contexts;
			var lifecycleManager = scriptRunner.LifecycleManager;

			var matchedCount = 0;
			var processedNames = new HashSet<String>();

			foreach (var sceneObject in sceneObjects)
			{
				if (sceneObject == null || !sceneObject.IsValid || !sceneObject.IsEnabled)
					continue;

				var objectName = sceneObject.Name;

				// Check if we have a script matching this object's name
				var scriptDef = scriptRegistry.GetByName(objectName);
				if (scriptDef != null)
				{
					// Create run context for this object-script pair
					var context = contextRegistry.CreateContext(scriptDef, sceneObject);
					lifecycleManager.Register(context, sceneObject);
					matchedCount++;

					if (!processedNames.Contains(objectName))
					{
						processedNames.Add(objectName);
						LunyLogger.LogInfo($"Bound {scriptDef} to object '{objectName}'", scriptRunner);
					}
				}
			}

			LunyLogger.LogInfo($"{matchedCount} {nameof(ScriptContext)}s created from {sceneObjects.Count} {nameof(LunyObject)}s",
				scriptRunner);
		}

		/// <summary>
		/// Clears all contexts and reprocesses the scene.
		/// Useful for scene reloads or hot reload scenarios.
		/// </summary>
		public static void ReprocessScene(ScriptRunner scriptRunner)
		{
			// TODO: this should be in the runner itself
			scriptRunner.Contexts.Clear();
			var sceneObjects = LunyEngine.Instance.Scene.GetAllObjects();
			RegisterObjects(sceneObjects, scriptRunner);
		}

		public static void BuildScripts(IReadOnlyList<ScriptContext> scriptContexts, ScriptRunner scriptRunner)
		{
			var sw = Stopwatch.StartNew();

			var activatedCount = 0;
			foreach (var context in scriptContexts)
			{
				try
				{
					LunyLogger.LogInfo($"Building script {context.ScriptType.Name} for {context.LunyObject}", scriptRunner);

					// Create script instance, initialize with context, and call Build()
					var scriptInstance = (LunyScript)Activator.CreateInstance(context.ScriptType);
					scriptInstance.Initialize(context);
					scriptInstance.Build();
					scriptInstance.Shutdown();
					activatedCount++;
				}
				catch (Exception ex)
				{
					LunyLogger.LogError($"{context.ScriptType} failed to build: {ex}", scriptRunner);
					Debugger.Break();
				}
			}

			// sends initial OnCreate and (if enabled) OnEnable events
			foreach (var context in scriptContexts)
				context.Activate();

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Activated {activatedCount} script(s) in {ms} ms", scriptRunner);
		}
	}
}
