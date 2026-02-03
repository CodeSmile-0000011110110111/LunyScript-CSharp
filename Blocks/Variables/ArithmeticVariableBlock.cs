using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal abstract class ArithmeticVariableBlockBase
	{
		protected readonly Table.VarHandle _handle;
		protected readonly IScriptVariable _value;

		protected ArithmeticVariableBlockBase(Table.VarHandle handle, IScriptVariable value)
		{
			_handle = handle;
			_value = value;
		}
	}

	internal sealed class AddVariableBlock : ArithmeticVariableBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static AddVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private AddVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		public Variable GetValue(ILunyScriptContext context) => _handle.Value + (Double)_value.GetValue(context);
	}

	internal sealed class SubtractVariableBlock : ArithmeticVariableBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static SubtractVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private SubtractVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		public Variable GetValue(ILunyScriptContext context) => _handle.Value - (Double)_value.GetValue(context);
	}

	internal sealed class MultiplyVariableBlock : ArithmeticVariableBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static MultiplyVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private MultiplyVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		public Variable GetValue(ILunyScriptContext context) => _handle.Value * (Double)_value.GetValue(context);
	}

	internal sealed class DivideVariableBlock : ArithmeticVariableBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static DivideVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private DivideVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		public Variable GetValue(ILunyScriptContext context) => _handle.Value / (Double)_value.GetValue(context);
	}
}
