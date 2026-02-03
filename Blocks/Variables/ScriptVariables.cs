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
		public static IScriptVariable operator +(ScriptVariable left, Variable right) => AddVariableBlock.Create(left._handle,
			left._name,
			left._table, Constant.Create(right));

		public static IScriptVariable operator +(ScriptVariable left, IScriptVariable right) =>
			AddVariableBlock.Create(left._handle, left._name, left._table, right);

		public static IScriptVariable operator -(ScriptVariable left, Variable right) => SubtractVariableBlock.Create(left._handle,
			left._name,
			left._table, Constant.Create(right));

		public static IScriptVariable operator -(ScriptVariable left, IScriptVariable right) =>
			SubtractVariableBlock.Create(left._handle, left._name, left._table, right);

		public static IScriptVariable operator *(ScriptVariable left, Variable right) => MultiplyVariableBlock.Create(left._handle,
			left._name,
			left._table, Constant.Create(right));

		public static IScriptVariable operator *(ScriptVariable left, IScriptVariable right) =>
			MultiplyVariableBlock.Create(left._handle, left._name, left._table, right);

		public static IScriptVariable operator /(ScriptVariable left, Variable right) => DivideVariableBlock.Create(left._handle,
			left._name,
			left._table, Constant.Create(right));

		public static IScriptVariable operator /(ScriptVariable left, IScriptVariable right) =>
			DivideVariableBlock.Create(left._handle, left._name, left._table, right);

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

		public IScriptActionBlock Add(Variable value) => AddVariableBlock.Create(_handle, _name, _table,
			Constant.Create(value));

		public IScriptActionBlock Add(IScriptVariable value) =>
			AddVariableBlock.Create(_handle, _name, _table, value);

		public IScriptActionBlock Sub(Variable value) => SubtractVariableBlock.Create(_handle, _name, _table,
			Constant.Create(value));

		public IScriptActionBlock Sub(IScriptVariable value) =>
			SubtractVariableBlock.Create(_handle, _name, _table, value);

		public IScriptActionBlock Mul(Variable value) => MultiplyVariableBlock.Create(_handle, _name, _table,
			Constant.Create(value));

		public IScriptActionBlock Mul(IScriptVariable value) =>
			MultiplyVariableBlock.Create(_handle, _name, _table, value);

		public IScriptActionBlock Div(Variable value) => DivideVariableBlock.Create(_handle, _name, _table,
			Constant.Create(value));

		public IScriptActionBlock Div(IScriptVariable value) =>
			DivideVariableBlock.Create(_handle, _name, _table, value);

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
