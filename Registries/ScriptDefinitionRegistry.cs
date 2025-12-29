using Luny;
using Luny.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LunyScript.Registries
{
	/// <summary>
	/// Discovers and manages LunyScript definitions.
	/// Supports both reflection-based discovery and manual registration.
	/// </summary>
	internal sealed class ScriptDefinitionRegistry
	{
		private readonly Dictionary<ScriptID, ScriptDefinition> _scriptsById = new();
		private readonly Dictionary<String, ScriptDefinition> _scriptsByName = new();

		/// <summary>
		/// Gets all registered script definitions.
		/// </summary>
		public IEnumerable<ScriptDefinition> AllScripts => _scriptsById.Values;

		/// <summary>
		/// Discovers all LunyScript subclasses via reflection and registers them.
		/// </summary>
		public void DiscoverScripts()
		{
			var sw = Stopwatch.StartNew();

			var scriptTypes = TypeDiscovery.FindAll<LunyScript>();

			foreach (var type in scriptTypes)
				RegisterScript(type);

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Discovered {_scriptsById.Count} LunyScript(s) in {ms} ms.", this);
		}

		/// <summary>
		/// Manually registers a LunyScript type.
		/// </summary>
		public ScriptDefinition RegisterScript(Type scriptType)
		{
			if (scriptType == null)
				throw new ArgumentNullException(nameof(scriptType));

			// Check if already registered by name
			if (_scriptsByName.ContainsKey(scriptType.Name))
			{
				LunyLogger.LogWarning($"Script {scriptType.Name} already registered, skipping duplicate", this);
				return _scriptsByName[scriptType.Name];
			}

			var definition = new ScriptDefinition(scriptType);
			_scriptsById[definition.ScriptID] = definition;
			_scriptsByName[definition.Name] = definition;

			LunyLogger.LogInfo($"Registered {definition}", this);
			return definition;
		}

		/// <summary>
		/// Gets a script definition by ID.
		/// </summary>
		public ScriptDefinition GetByID(ScriptID id)
		{
			_scriptsById.TryGetValue(id, out var definition);
			return definition;
		}

		/// <summary>
		/// Gets a script definition by name (for object binding).
		/// </summary>
		public ScriptDefinition GetByName(String name)
		{
			_scriptsByName.TryGetValue(name, out var definition);
			return definition;
		}

		/// <summary>
		/// Checks if a script with the given name exists.
		/// </summary>
		public Boolean HasScript(String name) => _scriptsByName.ContainsKey(name);

		/// <summary>
		/// Clears all registered scripts.
		/// </summary>
		public void Clear()
		{
			_scriptsById.Clear();
			_scriptsByName.Clear();
		}
	}
}
