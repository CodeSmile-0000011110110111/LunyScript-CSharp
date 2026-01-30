using Luny;
using Luny.Engine;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ToggleVariableBlock : IScriptActionBlock
	{
		private readonly Table.VarHandle _handle;
		private readonly String _name;
		private readonly Table _table;

		public static IScriptActionBlock Create(Table.VarHandle handle, String name, Table table) =>
			new ToggleVariableBlock(handle, name, table);

		private ToggleVariableBlock(Table.VarHandle handle, String name, Table table)
		{
			_handle = handle;
			_name = name;
			_table = table;
		}

		public void Execute(ILunyScriptContext context)
		{
			var previous = _handle.Value;
			var current = (Variable)!previous.AsBoolean();
			_handle.Value = current;
			_table.NotifyVariableChanged(_name, current, previous);
		}
	}
}
