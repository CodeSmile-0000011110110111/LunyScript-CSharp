using Luny;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace LunyScript
{
	public interface IVariables : IEnumerable<KeyValuePair<String, Variable>>
	{
		event EventHandler<VariableChangedEventArgs> OnVariableChanged;
		Variable this[String key] { get; set; }
		T Get<T>(String key);
		Boolean Has(String key);
		Boolean Remove(String key);
		void Clear();
	}

	/// <summary>
	/// Dictionary-based variable storage for LunyScript contexts.
	/// TODO: Consider restricting to number, bool and string values
	/// TODO: Replace with LuaTable when Lua integration is added.
	/// TODO: Optimize boxing/unboxing (consider variant type or generic storage).
	/// </summary>
	public sealed class Variables : IVariables
	{
		/// <summary>
		/// Fired when a variable is changed. Only invoked in debug builds.
		/// </summary>
		public event EventHandler<VariableChangedEventArgs> OnVariableChanged;

		// TODO: replace with LuaTable
		private readonly Dictionary<String, Variable> _vars = new();

		/// <summary>
		/// Gets or sets a variable by name.
		/// </summary>
		public Variable this[String key]
		{
			get => _vars.TryGetValue(key, out var value) ? value : new Variable(key, null);
			set
			{
				var oldValue = _vars.TryGetValue(key, out var existing) ? existing : new Variable(key, null);
				var newValue = new Variable(key, value.Value);
				_vars[key] = newValue;
				NotifyVariableChanged(key, oldValue, newValue);
			}
		}

		/// <summary>
		/// Gets the number of variables.
		/// </summary>
		public Int32 Count => _vars.Count;

		public IEnumerator<KeyValuePair<String, Variable>> GetEnumerator() => _vars.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Gets a variable with type casting.
		/// </summary>
		public T Get<T>(String key)
		{
			if (!_vars.TryGetValue(key, out var variable))
				return default;

			var value = variable.Value;
			if (value == null)
				return default;
			if (value is T tValue)
				return tValue;

			// Handle common conversions
			if (typeof(T) == typeof(Double))
				return (T)(Object)Convert.ToDouble(value, CultureInfo.InvariantCulture);
			if (typeof(T) == typeof(Single))
				return (T)(Object)Convert.ToSingle(value, CultureInfo.InvariantCulture);
			if (typeof(T) == typeof(Int32))
				return (T)(Object)Convert.ToInt32(value, CultureInfo.InvariantCulture);
			if (typeof(T) == typeof(Boolean))
				return (T)(Object)Convert.ToBoolean(value, CultureInfo.InvariantCulture);
			if (typeof(T) == typeof(String))
				return (T)(Object)value.ToString();
			if (typeof(T) == typeof(Number))
				return (T)(Object)new Number(Convert.ToDouble(value, CultureInfo.InvariantCulture));

			return default;
		}

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
				sb.AppendLine($"  {kvp.Key} = {kvp.Value}");
			return sb.ToString();
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void NotifyVariableChanged(String key, Variable oldValue, Variable newValue)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			OnVariableChanged?.Invoke(this, new VariableChangedEventArgs(key, oldValue, newValue));
#endif
		}
	}
}
