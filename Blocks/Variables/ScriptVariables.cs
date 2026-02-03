using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that returns the current loop counter from the context stack.
	/// </summary>
	internal sealed class LoopCounter : IScriptVariable
	{
		public static readonly LoopCounter Instance = new();
		public static Variable Get(ILunyScriptContext context) => context.LoopCount;

		private LoopCounter() {}

		public Variable GetValue(ILunyScriptContext context) => context.LoopCount;
	}

	/// <summary>
	/// Block that returns a constant variable value.
	/// </summary>
	public sealed class Constant : IScriptVariable
	{
		private readonly Variable _value;

		public static IScriptVariable Create(Variable value) => new Constant(value);

		private Constant(Variable value) => _value = value;

		public Variable GetValue(ILunyScriptContext context) => _value;
	}

	public sealed class ScriptVariable : IScriptVariable
	{
		private readonly Table.VarHandle _handle;
		private readonly String _name;
		private readonly Table _table;

		// Operators
		public static IScriptVariable operator +(ScriptVariable left, Variable right) => ArithmeticVariableBlock.Create(left._handle,
			left._name,
			left._table, Constant.Create(right), VariableOperation.Add);

		public static IScriptVariable operator +(ScriptVariable left, IScriptVariable right) =>
			ArithmeticVariableBlock.Create(left._handle, left._name, left._table, right, VariableOperation.Add);

		public static IScriptVariable operator -(ScriptVariable left, Variable right) => ArithmeticVariableBlock.Create(left._handle,
			left._name,
			left._table, Constant.Create(right), VariableOperation.Sub);

		public static IScriptVariable operator -(ScriptVariable left, IScriptVariable right) =>
			ArithmeticVariableBlock.Create(left._handle, left._name, left._table, right, VariableOperation.Sub);

		public static IScriptVariable operator *(ScriptVariable left, Variable right) => ArithmeticVariableBlock.Create(left._handle,
			left._name,
			left._table, Constant.Create(right), VariableOperation.Mul);

		public static IScriptVariable operator *(ScriptVariable left, IScriptVariable right) =>
			ArithmeticVariableBlock.Create(left._handle, left._name, left._table, right, VariableOperation.Mul);

		public static IScriptVariable operator /(ScriptVariable left, Variable right) => ArithmeticVariableBlock.Create(left._handle,
			left._name,
			left._table, Constant.Create(right), VariableOperation.Div);

		public static IScriptVariable operator /(ScriptVariable left, IScriptVariable right) =>
			ArithmeticVariableBlock.Create(left._handle, left._name, left._table, right, VariableOperation.Div);

		internal static ScriptVariable From(Table.VarHandle handle, String name, Table table) => new(handle, name, table);

		private ScriptVariable(Table.VarHandle handle, String name, Table table)
		{
			_handle = handle;
			_name = name;
			_table = table;
		}

		// IScriptVariable
		public Variable GetValue(ILunyScriptContext context) => _handle.Value;

		// Actions
		public IScriptActionBlock Set(Variable value) => SetVariableBlock.Create(_handle, _name, _table, Constant.Create(value));
		public IScriptActionBlock Set(IScriptVariable value) => SetVariableBlock.Create(_handle, _name, _table, value);

		public IScriptActionBlock Add(Variable value) => ArithmeticVariableBlock.Create(_handle, _name, _table,
			Constant.Create(value), VariableOperation.Add);

		public IScriptActionBlock Add(IScriptVariable value) =>
			ArithmeticVariableBlock.Create(_handle, _name, _table, value, VariableOperation.Add);

		public IScriptActionBlock Sub(Variable value) => ArithmeticVariableBlock.Create(_handle, _name, _table,
			Constant.Create(value), VariableOperation.Sub);

		public IScriptActionBlock Sub(IScriptVariable value) =>
			ArithmeticVariableBlock.Create(_handle, _name, _table, value, VariableOperation.Sub);

		public IScriptActionBlock Mul(Variable value) => ArithmeticVariableBlock.Create(_handle, _name, _table,
			Constant.Create(value), VariableOperation.Mul);

		public IScriptActionBlock Mul(IScriptVariable value) =>
			ArithmeticVariableBlock.Create(_handle, _name, _table, value, VariableOperation.Mul);

		public IScriptActionBlock Div(Variable value) => ArithmeticVariableBlock.Create(_handle, _name, _table,
			Constant.Create(value), VariableOperation.Div);

		public IScriptActionBlock Div(IScriptVariable value) =>
			ArithmeticVariableBlock.Create(_handle, _name, _table, value, VariableOperation.Div);

		public IScriptActionBlock Toggle() => ToggleVariableBlock.Create(_handle, _name, _table);

		// Conditions
		public IScriptConditionBlock IsTrue() => VariableConditionBlock.Create(_handle, VariableComparison.IsTrue);
		public IScriptConditionBlock IsFalse() => VariableConditionBlock.Create(_handle, VariableComparison.IsFalse);

		public IScriptConditionBlock IsEqual(Variable value) =>
			VariableConditionBlock.Create(_handle, VariableComparison.Equal, Constant.Create(value));

		public IScriptConditionBlock IsEqual(IScriptVariable value) => VariableConditionBlock.Create(_handle, VariableComparison.Equal, value);

		public IScriptConditionBlock IsNotEqual(Variable value) =>
			VariableConditionBlock.Create(_handle, VariableComparison.NotEqual, Constant.Create(value));

		public IScriptConditionBlock IsNotEqual(IScriptVariable value) =>
			VariableConditionBlock.Create(_handle, VariableComparison.NotEqual, value);

		public IScriptConditionBlock IsGreater(Variable value) =>
			VariableConditionBlock.Create(_handle, VariableComparison.Greater, Constant.Create(value));

		public IScriptConditionBlock IsGreater(IScriptVariable value) =>
			VariableConditionBlock.Create(_handle, VariableComparison.Greater, value);

		public IScriptConditionBlock IsGreaterOrEqual(Variable value) =>
			VariableConditionBlock.Create(_handle, VariableComparison.GreaterOrEqual, Constant.Create(value));

		public IScriptConditionBlock IsGreaterOrEqual(IScriptVariable value) =>
			VariableConditionBlock.Create(_handle, VariableComparison.GreaterOrEqual, value);

		public IScriptConditionBlock IsLess(Variable value) =>
			VariableConditionBlock.Create(_handle, VariableComparison.Less, Constant.Create(value));

		public IScriptConditionBlock IsLess(IScriptVariable value) => VariableConditionBlock.Create(_handle, VariableComparison.Less, value);

		public IScriptConditionBlock IsLessOrEqual(Variable value) =>
			VariableConditionBlock.Create(_handle, VariableComparison.LessOrEqual, Constant.Create(value));

		public IScriptConditionBlock IsLessOrEqual(IScriptVariable value) =>
			VariableConditionBlock.Create(_handle, VariableComparison.LessOrEqual, value);
	}
}
