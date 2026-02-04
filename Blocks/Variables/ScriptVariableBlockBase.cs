using Luny;
using LunyScript.Execution;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	public sealed class ScriptVariable : ScriptVariableBlockBase
	{
		private readonly Table.VarHandle _handle;

		internal override ScriptVariable Variable => this;
		internal Table.VarHandle VarHandle => _handle;

		public String Name => _handle.Name;
		public Variable Value => _handle.Value;

		internal static ScriptVariable From(Table.VarHandle handle) => new(handle);

		private ScriptVariable(Table.VarHandle handle) => _handle = handle;

		public override String ToString() => _handle.ToString();

		// IScriptVariable
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _handle.Value;
	}

	public abstract class ScriptVariableBlockBase : IScriptVariable, IScriptConditionBlock
	{
		internal virtual ScriptVariable Variable => null;

		internal Table.VarHandle Handle => Variable?.VarHandle;

		// Arithmetic Operators
		public static ScriptVariableBlockBase operator +(ScriptVariableBlockBase left, Variable right) =>
			AddBlock.Create(left, Constant.Create(right));

		public static ScriptVariableBlockBase operator +(ScriptVariableBlockBase left, IScriptVariable right) => AddBlock.Create(left, right);

		public static ScriptVariableBlockBase operator +(Variable left, ScriptVariableBlockBase right) =>
			AddBlock.Create(Constant.Create(left), right);

		public static ScriptVariableBlockBase operator -(ScriptVariableBlockBase left, Variable right) =>
			SubBlock.Create(left, Constant.Create(right));

		public static ScriptVariableBlockBase operator -(ScriptVariableBlockBase left, IScriptVariable right) => SubBlock.Create(left, right);

		public static ScriptVariableBlockBase operator -(Variable left, ScriptVariableBlockBase right) =>
			SubBlock.Create(Constant.Create(left), right);

		public static ScriptVariableBlockBase operator *(ScriptVariableBlockBase left, Variable right) =>
			MulBlock.Create(left, Constant.Create(right));

		public static ScriptVariableBlockBase operator *(ScriptVariableBlockBase left, IScriptVariable right) => MulBlock.Create(left, right);

		public static ScriptVariableBlockBase operator *(Variable left, ScriptVariableBlockBase right) =>
			MulBlock.Create(Constant.Create(left), right);

		public static ScriptVariableBlockBase operator /(ScriptVariableBlockBase left, Variable right) =>
			DivBlock.Create(left, Constant.Create(right));

		public static ScriptVariableBlockBase operator /(ScriptVariableBlockBase left, IScriptVariable right) => DivBlock.Create(left, right);

		public static ScriptVariableBlockBase operator /(Variable left, ScriptVariableBlockBase right) =>
			DivBlock.Create(Constant.Create(left), right);

		public static ScriptVariableBlockBase operator ++(ScriptVariableBlockBase a) => a + 1;
		public static ScriptVariableBlockBase operator --(ScriptVariableBlockBase a) => a - 1;

		// Comparison Operators
		public static ScriptVariableBlockBase operator ==(ScriptVariableBlockBase left, Variable right) =>
			IsEqualToBlock.Create(left, Constant.Create(right));

		public static ScriptVariableBlockBase operator ==(ScriptVariableBlockBase left, IScriptVariable right) =>
			IsEqualToBlock.Create(left, right);

		public static ScriptVariableBlockBase operator !=(ScriptVariableBlockBase left, Variable right) =>
			IsNotEqualToBlock.Create(left, Constant.Create(right));

		public static ScriptVariableBlockBase operator !=(ScriptVariableBlockBase left, IScriptVariable right) =>
			IsNotEqualToBlock.Create(left, right);

		public static ScriptVariableBlockBase operator >(ScriptVariableBlockBase left, Variable right) =>
			IsGreaterThanBlock.Create(left, Constant.Create(right));

		public static ScriptVariableBlockBase operator >(ScriptVariableBlockBase left, IScriptVariable right) =>
			IsGreaterThanBlock.Create(left, right);

		public static ScriptVariableBlockBase operator >=(ScriptVariableBlockBase left, Variable right) =>
			IsAtLeastBlock.Create(left, Constant.Create(right));

		public static ScriptVariableBlockBase operator >=(ScriptVariableBlockBase left, IScriptVariable right) =>
			IsAtLeastBlock.Create(left, right);

		public static ScriptVariableBlockBase operator <(ScriptVariableBlockBase left, Variable right) =>
			IsLessThanBlock.Create(left, Constant.Create(right));

		public static ScriptVariableBlockBase operator <(ScriptVariableBlockBase left, IScriptVariable right) =>
			IsLessThanBlock.Create(left, right);

		public static ScriptVariableBlockBase operator <=(ScriptVariableBlockBase left, Variable right) =>
			IsAtMostBlock.Create(left, Constant.Create(right));

		public static ScriptVariableBlockBase operator <=(ScriptVariableBlockBase left, IScriptVariable right) =>
			IsAtMostBlock.Create(left, right);

		// Unary Operators
		public static ScriptVariableBlockBase operator !(ScriptVariableBlockBase operand) => NotBlock.Create(operand);

		// Actions
		public IScriptActionBlock Set(Variable value) => SetVariableBlock.Create(GetHandle(), Constant.Create(value));
		public IScriptActionBlock Set(IScriptVariable value) => SetVariableBlock.Create(GetHandle(), value);

		public IScriptActionBlock Inc() => Add(1);
		public IScriptActionBlock Dec() => Sub(1);

		public IScriptActionBlock Add(Variable value) => Set(this + value);
		public IScriptActionBlock Add(IScriptVariable value) => Set(this + value);

		public IScriptActionBlock Sub(Variable value) => Set(this - value);
		public IScriptActionBlock Sub(IScriptVariable value) => Set(this - value);

		public IScriptActionBlock Mul(Variable value) => Set(this * value);
		public IScriptActionBlock Mul(IScriptVariable value) => Set(this * value);

		public IScriptActionBlock Div(Variable value) => Set(this / value);
		public IScriptActionBlock Div(IScriptVariable value) => Set(this / value);

		public IScriptActionBlock Toggle() => Set(!this);

		// Conditions
		public IScriptConditionBlock IsTrue() => this;
		public IScriptConditionBlock IsFalse() => !this;

		public IScriptConditionBlock IsEqualTo(Variable value) => this == value;
		public IScriptConditionBlock IsEqualTo(IScriptVariable value) => this == value;

		public IScriptConditionBlock IsNotEqualTo(Variable value) => this != value;
		public IScriptConditionBlock IsNotEqualTo(IScriptVariable value) => this != value;

		public IScriptConditionBlock IsGreaterThan(Variable value) => this > value;
		public IScriptConditionBlock IsGreaterThan(IScriptVariable value) => this > value;

		public IScriptConditionBlock IsLessThan(Variable value) => this < value;
		public IScriptConditionBlock IsLessThan(IScriptVariable value) => this < value;

		public IScriptConditionBlock IsAtLeast(Variable value) => this >= value;
		public IScriptConditionBlock IsAtLeast(IScriptVariable value) => this >= value;

		public IScriptConditionBlock IsAtMost(Variable value) => this <= value;
		public IScriptConditionBlock IsAtMost(IScriptVariable value) => this <= value;

		private Table.VarHandle GetHandle() => Handle ?? throw new InvalidOperationException($"Cannot perform modification action on {GetType().Name}. Only variables can be modified.");

		public virtual Boolean Evaluate(ILunyScriptContext context) => GetValue(context).AsBoolean();
		public abstract Variable GetValue(ILunyScriptContext context);

		public override Boolean Equals(Object obj) => base.Equals(obj);
		public override Int32 GetHashCode() => base.GetHashCode();
	}
}
