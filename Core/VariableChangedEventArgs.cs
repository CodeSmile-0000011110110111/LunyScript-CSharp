using System;

namespace LunyScript
{
	public sealed class VariableChangedEventArgs : EventArgs
	{
		public String Name { get; internal set; }
		public Variable OldValue { get; internal set; }
		public Variable NewValue { get; internal set; }

		public override String ToString() => $"Variable '{Name}' changed: {OldValue} -> {NewValue}";
	}
}
