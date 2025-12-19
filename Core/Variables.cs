using System;
using System.Collections.Generic;
using System.Text;

namespace LunyScript
{
	/// <summary>
	/// Dictionary-based variable storage for LunyScript contexts.
	/// TODO: Replace with LuaTable when Lua integration is added.
	/// TODO: Optimize boxing/unboxing (consider variant type or generic storage).
	/// </summary>
	public sealed class Variables
	{
		private readonly Dictionary<String, Object> _vars = new Dictionary<String, Object>();

		/// <summary>
		/// Gets or sets a variable by name.
		/// </summary>
		public Object this[String key]
		{
			get => _vars.TryGetValue(key, out var value) ? value : null;
			set => _vars[key] = value;
		}

		/// <summary>
		/// Gets a variable with type casting.
		/// </summary>
		public T Get<T>(String key)
		{
			if (_vars.TryGetValue(key, out var value))
				return (T)value;
			return default;
		}

		/// <summary>
		/// Sets a variable.
		/// </summary>
		public void Set<T>(String key, T value) => _vars[key] = value;

		/// <summary>
		/// Checks if a variable exists.
		/// </summary>
		public Boolean Has(String key) => _vars.ContainsKey(key);

		/// <summary>
		/// Removes a variable.
		/// </summary>
		public Boolean Remove(String key) => _vars.Remove(key);

		/// <summary>
		/// Clears all variables.
		/// </summary>
		public void Clear() => _vars.Clear();

		/// <summary>
		/// Gets the number of variables.
		/// </summary>
		public Int32 Count => _vars.Count;

		public override String ToString()
		{
			if (_vars.Count == 0)
				return "Variables: (empty)";

			var sb = new StringBuilder();
			sb.AppendLine($"Variables: ({_vars.Count})");
			foreach (var kvp in _vars)
			{
				sb.AppendLine($"  {kvp.Key} = {kvp.Value ?? "null"}");
			}
			return sb.ToString();
		}
	}
}
