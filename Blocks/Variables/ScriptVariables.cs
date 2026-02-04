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

	public sealed class ScriptVariable : IScriptVariable, IScriptConditionBlock
	{
		private readonly Table.VarHandle _handle;

		// Operators
		public static IScriptVariable operator +(ScriptVariable left, Variable right) => AddVariableBlock.Create(left._handle,
			Constant.Create(right));

		public static IScriptVariable operator +(ScriptVariable left, IScriptVariable right) => AddVariableBlock.Create(left._handle, right);

		public static IScriptVariable operator -(ScriptVariable left, Variable right) => SubtractVariableBlock.Create(left._handle,
			Constant.Create(right));

		public static IScriptVariable operator -(ScriptVariable left, IScriptVariable right) =>
			SubtractVariableBlock.Create(left._handle, right);

		public static IScriptVariable operator *(ScriptVariable left, Variable right) => MultiplyVariableBlock.Create(left._handle,
			Constant.Create(right));

		public static IScriptVariable operator *(ScriptVariable left, IScriptVariable right) =>
			MultiplyVariableBlock.Create(left._handle, right);

		public static IScriptVariable operator /(ScriptVariable left, Variable right) => DivideVariableBlock.Create(left._handle,
			Constant.Create(right));

		public static IScriptVariable operator /(ScriptVariable left, IScriptVariable right) => DivideVariableBlock.Create(left._handle, right);

		// Comparison Operators
		public static IScriptConditionBlock operator ==(ScriptVariable left, Variable right) => left.IsEqualTo(right);
		public static IScriptConditionBlock operator ==(ScriptVariable left, IScriptVariable right) => left.IsEqualTo(right);
		public static IScriptConditionBlock operator !=(ScriptVariable left, Variable right) => left.IsNotEqualTo(right);
		public static IScriptConditionBlock operator !=(ScriptVariable left, IScriptVariable right) => left.IsNotEqualTo(right);

		public static IScriptConditionBlock operator >(ScriptVariable left, Variable right) => left.IsGreaterThan(right);
		public static IScriptConditionBlock operator >(ScriptVariable left, IScriptVariable right) => left.IsGreaterThan(right);
		public static IScriptConditionBlock operator >=(ScriptVariable left, Variable right) => left.IsAtLeast(right);
		public static IScriptConditionBlock operator >=(ScriptVariable left, IScriptVariable right) => left.IsAtLeast(right);

		public static IScriptConditionBlock operator <(ScriptVariable left, Variable right) => left.IsLessThan(right);
		public static IScriptConditionBlock operator <(ScriptVariable left, IScriptVariable right) => left.IsLessThan(right);
		public static IScriptConditionBlock operator <=(ScriptVariable left, Variable right) => left.IsAtMost(right);
		public static IScriptConditionBlock operator <=(ScriptVariable left, IScriptVariable right) => left.IsAtMost(right);

		internal static ScriptVariable From(Table.VarHandle handle) => new(handle);

		private ScriptVariable(Table.VarHandle handle) => _handle = handle;

		// IScriptConditionBlock
		public Boolean Evaluate(ILunyScriptContext context) => GetValue(context).AsBoolean();

		// IScriptVariable
		public Variable GetValue(ILunyScriptContext context) => _handle.Value;

		public override Boolean Equals(Object obj) => base.Equals(obj);
		public override Int32 GetHashCode() => base.GetHashCode();

		// Actions
		public IScriptActionBlock Set(Variable value) => SetVariableBlock.Create(_handle, Constant.Create(value));
		public IScriptActionBlock Set(IScriptVariable value) => SetVariableBlock.Create(_handle, value);

		public IScriptActionBlock Inc() => AddVariableBlock.Create(_handle, Constant.Create(1));
		public IScriptActionBlock Dec() => SubtractVariableBlock.Create(_handle, Constant.Create(1));

		public IScriptActionBlock Add(Variable value) => AddVariableBlock.Create(_handle, Constant.Create(value));
		public IScriptActionBlock Add(IScriptVariable value) => AddVariableBlock.Create(_handle, value);

		public IScriptActionBlock Sub(Variable value) => SubtractVariableBlock.Create(_handle, Constant.Create(value));
		public IScriptActionBlock Sub(IScriptVariable value) => SubtractVariableBlock.Create(_handle, value);

		public IScriptActionBlock Mul(Variable value) => MultiplyVariableBlock.Create(_handle, Constant.Create(value));
		public IScriptActionBlock Mul(IScriptVariable value) => MultiplyVariableBlock.Create(_handle, value);

		public IScriptActionBlock Div(Variable value) => DivideVariableBlock.Create(_handle, Constant.Create(value));
		public IScriptActionBlock Div(IScriptVariable value) => DivideVariableBlock.Create(_handle, value);

		public IScriptActionBlock Toggle() => ToggleBooleanVariableBlock.Create(_handle);

		// Conditions
		public IScriptConditionBlock IsTrue() => IsVariableTrueBlock.Create(_handle);
		public IScriptConditionBlock IsFalse() => IsVariableFalseBlock.Create(_handle);

		public IScriptConditionBlock IsEqualTo(Variable value) => IsVariableEqualToBlock.Create(_handle, Constant.Create(value));
		public IScriptConditionBlock IsEqualTo(IScriptVariable value) => IsVariableEqualToBlock.Create(_handle, value);
		public IScriptConditionBlock IsNotEqualTo(Variable value) => IsVariableNotEqualToBlock.Create(_handle, Constant.Create(value));
		public IScriptConditionBlock IsNotEqualTo(IScriptVariable value) => IsVariableNotEqualToBlock.Create(_handle, value);
		public IScriptConditionBlock IsGreaterThan(Variable value) => IsVariableGreaterThanBlock.Create(_handle, Constant.Create(value));
		public IScriptConditionBlock IsGreaterThan(IScriptVariable value) => IsVariableGreaterThanBlock.Create(_handle, value);
		public IScriptConditionBlock IsLessThan(Variable value) => IsVariableLessThanBlock.Create(_handle, Constant.Create(value));
		public IScriptConditionBlock IsLessThan(IScriptVariable value) => IsVariableLessThanBlock.Create(_handle, value);
		public IScriptConditionBlock IsAtLeast(Variable value) => IsVariableAtLeastBlock.Create(_handle, Constant.Create(value));
		public IScriptConditionBlock IsAtLeast(IScriptVariable value) => IsVariableAtLeastBlock.Create(_handle, value);
		public IScriptConditionBlock IsAtMost(Variable value) => IsVariableAtMostBlock.Create(_handle, Constant.Create(value));
		public IScriptConditionBlock IsAtMost(IScriptVariable value) => IsVariableAtMostBlock.Create(_handle, value);
	}
}
