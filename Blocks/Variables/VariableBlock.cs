using Luny;
using LunyScript.Exceptions;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for variable blocks that evaluate to a runtime Variable.
	/// Extends ScriptConditionBlock because variables are implicitly usable as conditions
	/// (via AsBoolean conversion).
	/// </summary>
	public abstract class VariableBlock : ScriptConditionBlock
	{
		internal virtual Table.VarHandle TargetHandle => null;

		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			GetValue(runtimeContext).AsBoolean();

		public abstract Variable GetValue(IScriptRuntimeContext runtimeContext);

		public static implicit operator VariableBlock(Variable value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Int32 value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Int64 value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Single value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Double value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(Boolean value) => ConstantVariableBlock.Create(value);
		public static implicit operator VariableBlock(String value) => ConstantVariableBlock.Create(value);

		// Arithmetic Operators
		public static VariableBlock operator +(VariableBlock left, Variable right) =>
			AddVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator +(VariableBlock left, VariableBlock right) => AddVariableBlock.Create(left, right);

		public static VariableBlock operator +(Variable left, VariableBlock right) =>
			AddVariableBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator -(VariableBlock left, Variable right) =>
			SubVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator -(VariableBlock left, VariableBlock right) => SubVariableBlock.Create(left, right);

		public static VariableBlock operator -(Variable left, VariableBlock right) =>
			SubVariableBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator *(VariableBlock left, Variable right) =>
			MulVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator *(VariableBlock left, VariableBlock right) => MulVariableBlock.Create(left, right);

		public static VariableBlock operator *(Variable left, VariableBlock right) =>
			MulVariableBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator /(VariableBlock left, Variable right) =>
			DivVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator /(VariableBlock left, VariableBlock right) => DivVariableBlock.Create(left, right);

		public static VariableBlock operator /(Variable left, VariableBlock right) =>
			DivVariableBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator ++(VariableBlock a) => a + 1;
		public static VariableBlock operator --(VariableBlock a) => a - 1;

		// Comparison Operators
		public static VariableBlock operator ==(VariableBlock left, Variable right) =>
			IsEqualToVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator ==(VariableBlock left, VariableBlock right) => IsEqualToVariableBlock.Create(left, right);

		public static VariableBlock operator !=(VariableBlock left, Variable right) =>
			IsNotEqualToVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator !=(VariableBlock left, VariableBlock right) =>
			IsNotEqualToVariableBlock.Create(left, right);

		public static VariableBlock operator >(VariableBlock left, Variable right) =>
			IsGreaterThanVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator >(VariableBlock left, VariableBlock right) =>
			IsGreaterThanVariableBlock.Create(left, right);

		public static VariableBlock operator >=(VariableBlock left, Variable right) =>
			IsAtLeastVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator >=(VariableBlock left, VariableBlock right) => IsAtLeastVariableBlock.Create(left, right);

		public static VariableBlock operator <(VariableBlock left, Variable right) =>
			IsLessThanVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator <(VariableBlock left, VariableBlock right) => IsLessThanVariableBlock.Create(left, right);

		public static VariableBlock operator <=(VariableBlock left, Variable right) =>
			IsAtMostVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator <=(VariableBlock left, VariableBlock right) => IsAtMostVariableBlock.Create(left, right);

		// Unary Operators
		public static VariableBlock operator !(VariableBlock operand) => NotBlock.Create(operand);

		private Boolean Equals(VariableBlock other) => throw new NotImplementedException($"{nameof(VariableBlock)}.{nameof(Equals)}()");

		public override Boolean Equals(Object obj)
		{
			if (obj is null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;

			return Equals((VariableBlock)obj);
		}

		public override Int32 GetHashCode() => throw new NotImplementedException($"{nameof(VariableBlock)}.{nameof(GetHashCode)}()");

		// Actions
		private Table.VarHandle GetHandleOrThrow()
		{
			var handle = TargetHandle;
			if (handle == null)
				throw new LunyScriptVariableException($"Cannot modify read-only variable: {GetType().Name}");
			if (handle.IsConstant)
				throw new LunyScriptVariableException($"Cannot modify constant variable: {handle.Name}");

			return handle;
		}

		public ScriptActionBlock Set(Variable value) =>
			AssignmentVariableBlock.Create(GetHandleOrThrow(), ConstantVariableBlock.Create(value));

		public ScriptActionBlock Set(VariableBlock value) => AssignmentVariableBlock.Create(GetHandleOrThrow(), value);

		public ScriptActionBlock Inc() => Add(1);
		public ScriptActionBlock Dec() => Sub(1);

		public ScriptActionBlock Add(Variable value) => Set(this + value);
		public ScriptActionBlock Add(VariableBlock value) => Set(this + value);

		public ScriptActionBlock Sub(Variable value) => Set(this - value);
		public ScriptActionBlock Sub(VariableBlock value) => Set(this - value);

		public ScriptActionBlock Mul(Variable value) => Set(this * value);
		public ScriptActionBlock Mul(VariableBlock value) => Set(this * value);

		public ScriptActionBlock Div(Variable value) => Set(this / value);
		public ScriptActionBlock Div(VariableBlock value) => Set(this / value);

		public ScriptActionBlock Toggle() => Set(!this);

	}
}
