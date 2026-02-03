using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal interface IVariableOperation
	{
		Variable Execute(Variable left, Variable right);
	}

	internal struct AddOp : IVariableOperation
	{
		public Variable Execute(Variable left, Variable right) => left + (Double)right;
	}

	internal struct SubOp : IVariableOperation
	{
		public Variable Execute(Variable left, Variable right) => left - (Double)right;
	}

	internal struct MulOp : IVariableOperation
	{
		public Variable Execute(Variable left, Variable right) => left * (Double)right;
	}

	internal struct DivOp : IVariableOperation
	{
		public Variable Execute(Variable left, Variable right) => left / (Double)right;
	}

	internal abstract class ArithmeticVariableBlockBase
	{
		protected readonly Table.VarHandle _handle;
		protected readonly String _varName;
		protected readonly Table _table;
		protected readonly IScriptVariable _value;

		protected static void Execute<TVarOp>(Table.VarHandle handle, String varName, Table table, IScriptVariable value,
			ILunyScriptContext context, TVarOp op) where TVarOp : struct, IVariableOperation
		{
			var previous = handle.Value;
			var operand = value.GetValue(context);
			var current = op.Execute(previous, operand);
			handle.Value = current;
			table.NotifyVariableChanged(varName, current, previous);
		}

		protected ArithmeticVariableBlockBase(Table.VarHandle handle, String varName, Table table, IScriptVariable value)
		{
			_handle = handle;
			_varName = varName;
			_table = table;
			_value = value;
		}
	}

	internal sealed class AddVariableBlock : ArithmeticVariableBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static AddVariableBlock Create(Table.VarHandle handle, String varName, Table table, IScriptVariable value) =>
			new(handle, varName, table, value);

		private AddVariableBlock(Table.VarHandle handle, String varName, Table table, IScriptVariable value)
			: base(handle, varName,
				table, value) {}

		public void Execute(ILunyScriptContext context) => Execute(_handle, _varName, _table, _value, context, new AddOp());

		public Variable GetValue(ILunyScriptContext context) => _handle.Value + (Double)_value.GetValue(context);
	}

	internal sealed class SubtractVariableBlock : ArithmeticVariableBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static SubtractVariableBlock Create(Table.VarHandle handle, String varName, Table table, IScriptVariable value) =>
			new(handle, varName, table, value);

		private SubtractVariableBlock(Table.VarHandle handle, String varName, Table table, IScriptVariable value)
			: base(handle,
				varName, table, value) {}

		public void Execute(ILunyScriptContext context) => Execute(_handle, _varName, _table, _value, context, new SubOp());

		public Variable GetValue(ILunyScriptContext context) => _handle.Value - (Double)_value.GetValue(context);
	}

	internal sealed class MultiplyVariableBlock : ArithmeticVariableBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static MultiplyVariableBlock Create(Table.VarHandle handle, String varName, Table table, IScriptVariable value) =>
			new(handle, varName, table, value);

		private MultiplyVariableBlock(Table.VarHandle handle, String varName, Table table, IScriptVariable value)
			: base(handle,
				varName, table, value) {}

		public void Execute(ILunyScriptContext context) => Execute(_handle, _varName, _table, _value, context, new MulOp());

		public Variable GetValue(ILunyScriptContext context) => _handle.Value * (Double)_value.GetValue(context);
	}

	internal sealed class DivideVariableBlock : ArithmeticVariableBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static DivideVariableBlock Create(Table.VarHandle handle, String varName, Table table, IScriptVariable value) =>
			new(handle, varName, table, value);

		private DivideVariableBlock(Table.VarHandle handle, String varName, Table table, IScriptVariable value)
			: base(handle,
				varName, table, value) {}

		public void Execute(ILunyScriptContext context) => Execute(_handle, _varName, _table, _value, context, new DivOp());

		public Variable GetValue(ILunyScriptContext context) => _handle.Value / (Double)_value.GetValue(context);
	}
}
