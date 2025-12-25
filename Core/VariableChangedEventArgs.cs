using System;

namespace LunyScript
{
	public sealed class VariableChangedEventArgs : EventArgs
	{
		public String Name { get; }
		public Object OldValue { get; }
		public Object NewValue { get; }

		public VariableChangedEventArgs(String name, Object oldValue, Object newValue)
		{
			Name = name;
			OldValue = oldValue;
			NewValue = newValue;
		}

		public override String ToString() => $"Variable '{Name}' changed: {OldValue} -> {NewValue}";
	}
}
