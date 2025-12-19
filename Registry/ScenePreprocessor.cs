using System;
using System.Collections.Generic;
using Luny;
using Luny.Providers;
using Luny.Proxies;

namespace LunyScript
{
	/// <summary>
	/// Scans scenes at runtime to discover objects that should run LunyScripts.
	/// Binds scripts to objects based on name matching (exact, case-sensitive).
	/// </summary>
	public sealed class ScenePreprocessor
	{
		private readonly ScriptRegistry _scriptRegistry;
		private readonly ExecutionContextRegistry _contextRegistry;
		private readonly ISceneServiceProvider _sceneService;

		public ScenePreprocessor(
			ScriptRegistry scriptRegistry,
			ExecutionContextRegistry contextRegistry,
			ISceneServiceProvider sceneService)
		{
			_scriptRegistry = scriptRegistry ?? throw new ArgumentNullException(nameof(scriptRegistry));
			_contextRegistry = contextRegistry ?? throw new ArgumentNullException(nameof(contextRegistry));
			_sceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
		}

		/// <summary>
		/// Processes the current scene, finding objects and binding them to scripts.
		/// Creates execution contexts for matching object-script pairs.
		/// </summary>
		public void ProcessCurrentScene()
		{
			var allObjects = _sceneService.GetAllObjects();
			if (allObjects == null || allObjects.Count == 0)
			{
				LunyLogger.LogInfo("No objects found in current scene", this);
				return;
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
					// Create execution context for this object-script pair
					var context = new ExecutionContext(scriptDef.ScriptID, obj);
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
