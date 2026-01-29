using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal sealed class SetVariableBlock : IScriptActionBlock
	{
		private readonly Table.VarHandle _handle;
		private readonly String _name;
		private readonly Table _table;
		private readonly IScriptVariable _value;

		public static IScriptActionBlock Create(Table.VarHandle handle, String name, Table table, IScriptVariable value) =>
			new SetVariableBlock(handle, name, table, value);

		private SetVariableBlock(Table.VarHandle handle, String name, Table table, IScriptVariable value)
		{
			_handle = handle ?? throw new ArgumentNullException(nameof(handle));
			_name = name ?? throw new ArgumentNullException(nameof(name));
			_table = table ?? throw new ArgumentNullException(nameof(table));
			_value = value ?? throw new ArgumentNullException(nameof(value));
		}

		public void Execute(ILunyScriptContext context)
		{
			var previous = _handle.Value;
			var current = _value.GetValue(context);
			_handle.Value = current;
			_table.NotifyVariableChanged(_name, current, previous);
		}
	}
}
