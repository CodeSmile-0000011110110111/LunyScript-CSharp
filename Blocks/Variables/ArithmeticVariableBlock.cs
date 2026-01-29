using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal enum VariableOperation
	{
		Add,
		Sub,
		Mul,
		Div,
	}

	internal sealed class ArithmeticVariableBlock : IScriptActionBlock, IScriptVariable
	{
		private readonly Table.VarHandle _handle;
		private readonly String _name;
		private readonly Table _table;
		private readonly IScriptVariable _value;
		private readonly VariableOperation _operation;

		public static ArithmeticVariableBlock Create(Table.VarHandle handle, String name, Table table, IScriptVariable value,
			VariableOperation operation) => new(handle, name, table, value, operation);

		private ArithmeticVariableBlock(Table.VarHandle handle, String name, Table table, IScriptVariable value,
			VariableOperation operation)
		{
			_handle = handle;
			_name = name;
			_table = table;
			_value = value;
			_operation = operation;
		}

		public void Execute(ILunyScriptContext context)
		{
			var previous = _handle.Value;
			var operand = _value.GetValue(context);
			var current = Apply(previous, operand);
			_handle.Value = current;
			_table.NotifyVariableChanged(_name, current, previous);
		}

		public Variable GetValue(ILunyScriptContext context) => Apply(_handle.Value, _value.GetValue(context));

		private Variable Apply(Variable left, Variable right) => _operation switch
		{
			VariableOperation.Add => left + (Double)right,
			VariableOperation.Sub => left - (Double)right,
			VariableOperation.Mul => left * (Double)right,
			VariableOperation.Div => left / (Double)right,
			var _ => throw new NotImplementedException(),
		};
	}
}
