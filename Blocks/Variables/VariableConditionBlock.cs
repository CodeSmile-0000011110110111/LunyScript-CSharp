using Luny;
using Luny.Engine;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal enum VariableComparison
	{
		Equal,
		NotEqual,
		Greater,
		GreaterOrEqual,
		Less,
		LessOrEqual,
		IsTrue,
		IsFalse,
	}

	internal sealed class VariableConditionBlock : IScriptConditionBlock
	{
		private readonly Table.VarHandle _handle;
		private readonly IScriptVariable _right;
		private readonly VariableComparison _comparison;

		public static IScriptConditionBlock Create(Table.VarHandle handle, VariableComparison comparison, IScriptVariable right = null) =>
			new VariableConditionBlock(handle, comparison, right);

		private VariableConditionBlock(Table.VarHandle handle, VariableComparison comparison, IScriptVariable right)
		{
			_handle = handle;
			_comparison = comparison;
			_right = right;
		}

		public Boolean Evaluate(ILunyScriptContext context)
		{
			var left = _handle.Value;

			if (_comparison == VariableComparison.IsTrue)
				return left.AsBoolean();
			if (_comparison == VariableComparison.IsFalse)
				return !left.AsBoolean();

			var right = _right.GetValue(context);

			return _comparison switch
			{
				VariableComparison.Equal => left == right,
				VariableComparison.NotEqual => left != right,
				VariableComparison.Greater => left > (Double)right,
				VariableComparison.GreaterOrEqual => left >= (Double)right,
				VariableComparison.Less => left < (Double)right,
				VariableComparison.LessOrEqual => left <= (Double)right,
				var _ => throw new NotImplementedException(),
			};
		}
	}
}
