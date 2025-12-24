using Luny.Diagnostics;
using Luny.Interfaces.Providers;
using LunyScript.Execution;
using System;
using System.Collections.Generic;

namespace LunyScript.Registries
{
	/// <summary>
	/// Scans scenes at runtime to discover objects that should run LunyScripts.
	/// Binds scripts to objects based on name matching (exact, case-sensitive).
	/// </summary>
	internal sealed class ScenePreprocessor
	{
		private readonly LunyScriptRunner _scriptRunner;

		public ScenePreprocessor(LunyScriptRunner runner)
		{
			_scriptRunner = runner ?? throw new ArgumentNullException(nameof(runner));
		}

		/// <summary>
		/// Processes the current scene, finding objects and binding them to scripts.
		/// Creates run contexts for matching object-script pairs.
		/// </summary>
		public void ProcessSceneObjects()
		{
			var scene = _scriptRunner.Engine.Scene;
			var allSceneObjects = scene.GetAllObjects();
			if (allSceneObjects == null || allSceneObjects.Count == 0)
				throw new Exception($"No objects found in scene: {scene.CurrentSceneName}");

			var matchedCount = 0;
			var processedNames = new HashSet<String>();
			var scripts = _scriptRunner.Scripts;
			var contexts = _scriptRunner.Contexts;
			var engine = _scriptRunner.Engine;

			foreach (var sceneObject in allSceneObjects)
			{
				if (sceneObject == null || !sceneObject.IsValid)
					continue;

				var objectName = sceneObject.Name;

				// Check if we have a script matching this object's name
				var scriptDef = scripts.GetByName(objectName);
				if (scriptDef != null)
				{
					// Create run context for this object-script pair
					var context = new ScriptContext(scriptDef, sceneObject, engine);
					contexts.Register(context);
					matchedCount++;

					if (!processedNames.Contains(objectName))
					{
						processedNames.Add(objectName);
						LunyLogger.LogInfo($"Bound {scriptDef} to object '{objectName}'", this);
					}
				}
			}

			LunyLogger.LogInfo($"Scene preprocessing complete: {matchedCount} context(s) created from {allSceneObjects.Count} object(s)", this);
		}

		/// <summary>
		/// Clears all contexts and reprocesses the scene.
		/// Useful for scene reloads or hot reload scenarios.
		/// </summary>
		public void ReprocessScene()
		{
			_scriptRunner.Contexts.Clear();
			ProcessSceneObjects();
		}
	}
}
