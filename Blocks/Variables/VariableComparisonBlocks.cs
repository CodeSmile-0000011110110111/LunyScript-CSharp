using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal abstract class VariableComparisonBlockBase
	{
		protected readonly Table.VarHandle _handle;
		protected readonly IScriptVariable _right;

		protected VariableComparisonBlockBase(Table.VarHandle handle, IScriptVariable right = null)
		{
			_handle = handle;
			_right = right;
		}
	}

	internal sealed class IsVariableEqualToBlock : VariableComparisonBlockBase, IScriptConditionBlock
	{
		public static IsVariableEqualToBlock Create(Table.VarHandle handle, IScriptVariable right) => new(handle, right);

		private IsVariableEqualToBlock(Table.VarHandle handle, IScriptVariable right)
			: base(handle, right) {}

		public Boolean Evaluate(ILunyScriptContext context) => _handle.Value == _right.GetValue(context);
	}

	internal sealed class IsVariableNotEqualToBlock : VariableComparisonBlockBase, IScriptConditionBlock
	{
		public static IsVariableNotEqualToBlock Create(Table.VarHandle handle, IScriptVariable right) => new(handle, right);

		private IsVariableNotEqualToBlock(Table.VarHandle handle, IScriptVariable right)
			: base(handle, right) {}

		public Boolean Evaluate(ILunyScriptContext context) => _handle.Value != _right.GetValue(context);
	}

	internal sealed class IsVariableGreaterThanBlock : VariableComparisonBlockBase, IScriptConditionBlock
	{
		public static IsVariableGreaterThanBlock Create(Table.VarHandle handle, IScriptVariable right) => new(handle, right);

		private IsVariableGreaterThanBlock(Table.VarHandle handle, IScriptVariable right)
			: base(handle, right) {}

		public Boolean Evaluate(ILunyScriptContext context) => _handle.Value > (Double)_right.GetValue(context);
	}

	internal sealed class IsVariableGreaterOrEqualThanBlock : VariableComparisonBlockBase, IScriptConditionBlock
	{
		public static IsVariableGreaterOrEqualThanBlock Create(Table.VarHandle handle, IScriptVariable right) => new(handle, right);

		private IsVariableGreaterOrEqualThanBlock(Table.VarHandle handle, IScriptVariable right)
			: base(handle, right) {}

		public Boolean Evaluate(ILunyScriptContext context) => _handle.Value >= (Double)_right.GetValue(context);
	}

	internal sealed class IsVariableLessThanBlock : VariableComparisonBlockBase, IScriptConditionBlock
	{
		public static IsVariableLessThanBlock Create(Table.VarHandle handle, IScriptVariable right) => new(handle, right);

		private IsVariableLessThanBlock(Table.VarHandle handle, IScriptVariable right)
			: base(handle, right) {}

		public Boolean Evaluate(ILunyScriptContext context) => _handle.Value < (Double)_right.GetValue(context);
	}

	internal sealed class IsVariableLessOrEqualThanBlock : VariableComparisonBlockBase, IScriptConditionBlock
	{
		public static IsVariableLessOrEqualThanBlock Create(Table.VarHandle handle, IScriptVariable right) => new(handle, right);

		private IsVariableLessOrEqualThanBlock(Table.VarHandle handle, IScriptVariable right)
			: base(handle, right) {}

		public Boolean Evaluate(ILunyScriptContext context) => _handle.Value <= (Double)_right.GetValue(context);
	}

	internal sealed class IsVariableTrueBlock : VariableComparisonBlockBase, IScriptConditionBlock
	{
		public static IsVariableTrueBlock Create(Table.VarHandle handle) => new(handle);

		private IsVariableTrueBlock(Table.VarHandle handle)
			: base(handle) {}

		public Boolean Evaluate(ILunyScriptContext context) => _handle.Value.AsBoolean();
	}

	internal sealed class IsVariableFalseBlock : VariableComparisonBlockBase, IScriptConditionBlock
	{
		public static IsVariableFalseBlock Create(Table.VarHandle handle) => new(handle);

		private IsVariableFalseBlock(Table.VarHandle handle)
			: base(handle) {}

		public Boolean Evaluate(ILunyScriptContext context) => !_handle.Value.AsBoolean();
	}
}
