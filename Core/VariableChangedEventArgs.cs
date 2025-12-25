using System;

namespace LunyScript
{
	public sealed class VariableChangedEventArgs : EventArgs
	{
		public String Name { get; }
		public Variable OldValue { get; }
		public Variable NewValue { get; }

		public VariableChangedEventArgs(String name, Variable oldValue, Variable newValue)
		{
			Name = name;
			OldValue = oldValue;
			NewValue = newValue;
		}

		public override String ToString() => $"Variable '{Name}' changed: {OldValue} -> {NewValue}";
	}
}
