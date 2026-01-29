using Luny;
using LunyScript.Blocks;
using LunyScript.Execution;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		public readonly struct VarApi
		{
			private readonly Table _table;

			internal VarApi(Table table) => _table = table;

			public ScriptVar Get(String name) => ScriptVar.From(_table.GetHandle(name), name, _table);

			public IScriptActionBlock Set(String name, Variable value) => Get(name).Set(value);
			public IScriptActionBlock Set(String name, IScriptVariable value) => Get(name).Set(value);

			public IScriptActionBlock Add(String name, Variable value) => Get(name).Add(value);
			public IScriptActionBlock Add(String name, IScriptVariable value) => Get(name).Add(value);

			public IScriptConditionBlock IsTrue(String name) => Get(name).IsTrue();
			public IScriptConditionBlock IsFalse(String name) => Get(name).IsFalse();

			// public Variable this[String variableName]
			// {
			// 	get => _table[variableName];
			// 	set => _table[variableName] = value;
			// }
		}

		public sealed class ScriptVar : IScriptVariable
		{
			private readonly Table.VarHandle _handle;
			private readonly String _name;
			private readonly Table _table;

			// Operators
			public static IScriptVariable operator +(ScriptVar left, Variable right) => ArithmeticVariableBlock.Create(left._handle, left._name,
				left._table, Constant.Create(right), VariableOperation.Add);

			public static IScriptVariable operator +(ScriptVar left, IScriptVariable right) =>
				ArithmeticVariableBlock.Create(left._handle, left._name, left._table, right, VariableOperation.Add);

			public static IScriptVariable operator -(ScriptVar left, Variable right) => ArithmeticVariableBlock.Create(left._handle, left._name,
				left._table, Constant.Create(right), VariableOperation.Sub);

			public static IScriptVariable operator -(ScriptVar left, IScriptVariable right) =>
				ArithmeticVariableBlock.Create(left._handle, left._name, left._table, right, VariableOperation.Sub);

			public static IScriptVariable operator *(ScriptVar left, Variable right) => ArithmeticVariableBlock.Create(left._handle, left._name,
				left._table, Constant.Create(right), VariableOperation.Mul);

			public static IScriptVariable operator *(ScriptVar left, IScriptVariable right) =>
				ArithmeticVariableBlock.Create(left._handle, left._name, left._table, right, VariableOperation.Mul);

			public static IScriptVariable operator /(ScriptVar left, Variable right) => ArithmeticVariableBlock.Create(left._handle, left._name,
				left._table, Constant.Create(right), VariableOperation.Div);

			public static IScriptVariable operator /(ScriptVar left, IScriptVariable right) =>
				ArithmeticVariableBlock.Create(left._handle, left._name, left._table, right, VariableOperation.Div);

			internal static ScriptVar From(Table.VarHandle handle, String name, Table table) => new(handle, name, table);

			private ScriptVar(Table.VarHandle handle, String name, Table table)
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

			public IScriptConditionBlock IsEqual(IScriptVariable value) =>
				VariableConditionBlock.Create(_handle, VariableComparison.Equal, value);

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

			public IScriptConditionBlock IsLess(IScriptVariable value) =>
				VariableConditionBlock.Create(_handle, VariableComparison.Less, value);

			public IScriptConditionBlock IsLessOrEqual(Variable value) =>
				VariableConditionBlock.Create(_handle, VariableComparison.LessOrEqual, Constant.Create(value));

			public IScriptConditionBlock IsLessOrEqual(IScriptVariable value) =>
				VariableConditionBlock.Create(_handle, VariableComparison.LessOrEqual, value);
		}
	}
}
