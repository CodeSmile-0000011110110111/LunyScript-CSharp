using Luny;
using Luny.Engine.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LunyScript.Execution
{
	/// <summary>
	/// Discovers and manages LunyScript definitions.
	/// Supports both reflection-based discovery and manual registration.
	/// </summary>
	internal sealed class LunyScriptDefinitionRegistry
	{
		private readonly Dictionary<LunyScriptID, LunyScriptDefinition> _scriptsById = new();
		private readonly Dictionary<String, LunyScriptDefinition> _scriptsByName = new();

		public LunyScriptDefinitionRegistry() => DiscoverScripts();
		~LunyScriptDefinitionRegistry() => LunyTraceLogger.LogInfoFinalized(this);

		/// <summary>
		/// Discovers all LunyScript subclasses via reflection and registers them.
		/// </summary>
		public void DiscoverScripts()
		{
			var sw = Stopwatch.StartNew();

			var scriptTypes = LunyTypeDiscovery.FindAll<LunyScript>();

			foreach (var type in scriptTypes)
				RegisterScript(type);

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Discovered {_scriptsById.Count} LunyScript(s) in {ms} ms.", this);
		}

		/// <summary>
		/// Manually registers a LunyScript type.
		/// </summary>
		private void RegisterScript(Type scriptType)
		{
			if (scriptType == null)
				throw new ArgumentNullException(nameof(scriptType));

			if (_scriptsByName.ContainsKey(scriptType.Name))
			{
				LunyLogger.LogError($"{scriptType.Name} already registered", this);
				return;
			}

			var definition = new LunyScriptDefinition(scriptType);
			_scriptsById[definition.ScriptID] = definition;
			_scriptsByName[definition.Name] = definition;

			LunyLogger.LogInfo($"{definition} registered", this);
		}

		/// <summary>
		/// Gets a script definition by ID.
		/// </summary>
		public LunyScriptDefinition GetByID(LunyScriptID id)
		{
			_scriptsById.TryGetValue(id, out var definition);
			return definition;
		}

		/// <summary>
		/// Gets a script definition by name (for object binding).
		/// </summary>
		public LunyScriptDefinition GetByName(String name)
		{
			_scriptsByName.TryGetValue(name, out var definition);
			return definition;
		}

		/// <summary>
		/// Checks if a script with the given name exists.
		/// </summary>
		public Boolean HasScript(String name) => _scriptsByName.ContainsKey(name);

		internal void Shutdown()
		{
			_scriptsById.Clear();
			_scriptsByName.Clear();
		}

		public IReadOnlyCollection<String> GetNames() => _scriptsByName.Keys;
	}
}
