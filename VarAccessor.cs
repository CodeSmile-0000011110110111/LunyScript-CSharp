using Luny;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Provides indexed access to script variables.
	/// Getter returns a VariableBlock for use in script expressions and conditions.
	/// Setter performs immediate variable assignment during Build().
	/// </summary>
	public readonly struct VarAccessor
	{
		private readonly ITable _table;

		internal VarAccessor(ITable table) => _table = table;

		public VariableBlock this[String name]
		{
			get => TableVariableBlock.Create(_table.GetHandle(name));
			set => _table.GetHandle(name).Value = value.GetValue(null);
		}
	}
}
