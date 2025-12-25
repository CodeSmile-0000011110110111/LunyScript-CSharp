using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LunyScript
{
	/// <summary>
	/// Dictionary-based variable storage for LunyScript contexts.
	/// TODO: Consider restricting to number, bool and string values
	/// TODO: Replace with LuaTable when Lua integration is added.
	/// TODO: Optimize boxing/unboxing (consider variant type or generic storage).
	/// </summary>
	public sealed class Variables : IEnumerable<KeyValuePair<String, Object>>
	{
		/// <summary>
		/// Fired when a variable is changed. Only invoked in debug builds.
		/// </summary>
		public event EventHandler<VariableChangedEventArgs> OnVariableChanged;

		// TODO: replace with LuaTable
		private readonly Dictionary<String, Object> _vars = new();

		public IEnumerator<KeyValuePair<String, Object>> GetEnumerator() => _vars.GetEnumerator();

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Gets or sets a variable by name.
		/// </summary>
		public Object this[String key]
		{
			get => _vars.TryGetValue(key, out var value) ? value : null;
			set
			{
				var oldValue = _vars.TryGetValue(key, out var existing) ? existing : null;
				_vars[key] = value;
				NotifyVariableChanged(key, oldValue, value);
			}
		}

		/// <summary>
		/// Gets the number of variables.
		/// </summary>
		public Int32 Count => _vars.Count;

		/// <summary>
		/// Gets a variable with type casting.
		/// </summary>
		public T Get<T>(String key) => _vars.TryGetValue(key, out var value) && value is T t ? t : default;

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

		public override String ToString()
		{
			if (_vars.Count == 0)
				return "Variables: (empty)";

			var sb = new StringBuilder();
			sb.AppendLine($"Variables: ({_vars.Count})");
			foreach (var kvp in _vars)
				sb.AppendLine($"  {kvp.Key} = {kvp.Value ?? "null"}");
			return sb.ToString();
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void NotifyVariableChanged(String key, Object oldValue, Object newValue)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			OnVariableChanged?.Invoke(this, new VariableChangedEventArgs(key, oldValue, newValue));
#endif
		}
	}
}
