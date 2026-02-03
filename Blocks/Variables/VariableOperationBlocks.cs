using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal abstract class VariableOperationBlockBase
	{
		protected readonly Table.VarHandle _handle;
		protected readonly IScriptVariable _value;

		protected VariableOperationBlockBase(Table.VarHandle handle, IScriptVariable value)
		{
			_handle = handle;
			_value = value;
		}
	}

	internal sealed class AddVariableBlock : VariableOperationBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static AddVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private AddVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		public Variable GetValue(ILunyScriptContext context) => _handle.Value + (Double)_value.GetValue(context);
	}

	internal sealed class SubtractVariableBlock : VariableOperationBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static SubtractVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private SubtractVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		public Variable GetValue(ILunyScriptContext context) => _handle.Value - (Double)_value.GetValue(context);
	}

	internal sealed class MultiplyVariableBlock : VariableOperationBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static MultiplyVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private MultiplyVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		public Variable GetValue(ILunyScriptContext context) => _handle.Value * (Double)_value.GetValue(context);
	}

	internal sealed class DivideVariableBlock : VariableOperationBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static DivideVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private DivideVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		public Variable GetValue(ILunyScriptContext context) => _handle.Value / (Double)_value.GetValue(context);
	}
}
