using System;
using System.Collections.Generic;
using Luny;
using Luny.Providers;

namespace LunyScript
{
	/// <summary>
	/// Scans scenes at runtime to discover objects that should run LunyScripts.
	/// Binds scripts to objects based on name matching (exact, case-sensitive).
	/// </summary>
	public sealed class ScenePreprocessor
	{
		private readonly ScriptRegistry _scriptRegistry;
		private readonly RunContextRegistry _contextRegistry;
		private readonly ISceneServiceProvider _sceneService;
		private readonly Variables _globalVariables;

		public ScenePreprocessor(
			ScriptRegistry scriptRegistry,
			RunContextRegistry contextRegistry,
			ISceneServiceProvider sceneService,
			Variables globalVariables)
		{
			_scriptRegistry = scriptRegistry ?? throw new ArgumentNullException(nameof(scriptRegistry));
			_contextRegistry = contextRegistry ?? throw new ArgumentNullException(nameof(contextRegistry));
			_sceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
			_globalVariables = globalVariables ?? throw new ArgumentNullException(nameof(globalVariables));
		}

		/// <summary>
		/// Processes the current scene, finding objects and binding them to scripts.
		/// Creates run contexts for matching object-script pairs.
		/// </summary>
		public void ProcessCurrentScene()
		{
			var allObjects = _sceneService.GetAllObjects();
			if (allObjects == null || allObjects.Count == 0)
			{
				throw new Exception($"No objects found in scene: {_sceneService.CurrentSceneName}");
			}

			var matchedCount = 0;
			var processedNames = new HashSet<String>();

			foreach (var obj in allObjects)
			{
				if (obj == null || !obj.IsValid)
					continue;

				var objectName = obj.Name;

				// Check if we have a script matching this object's name
				var scriptDef = _scriptRegistry.GetByName(objectName);
				if (scriptDef != null)
				{
					// Create run context for this object-script pair
					var context = new RunContext(scriptDef.ScriptID, scriptDef.Type, obj, _globalVariables);
					_contextRegistry.Register(context);
					matchedCount++;

					if (!processedNames.Contains(objectName))
					{
						processedNames.Add(objectName);
						LunyLogger.LogInfo($"Bound script '{scriptDef.Name}' to object '{objectName}'", this);
					}
				}
			}

			LunyLogger.LogInfo($"Scene preprocessing complete: {matchedCount} context(s) created from {allObjects.Count} object(s)", this);
		}

		/// <summary>
		/// Clears all contexts and reprocesses the scene.
		/// Useful for scene reloads or hot reload scenarios.
		/// </summary>
		public void ReprocessScene()
		{
			_contextRegistry.Clear();
			ProcessCurrentScene();
		}
	}
}
