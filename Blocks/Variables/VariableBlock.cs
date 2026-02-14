using Luny;
using LunyScript.Exceptions;
using System;

namespace LunyScript.Blocks
{
	public abstract class VariableBlock : ScriptVariableBlock
	{
		internal virtual Table.VarHandle TargetHandle => null;

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

		public static VariableBlock operator +(VariableBlock left, IScriptVariableBlock right) => AddVariableBlock.Create(left, right);

		public static VariableBlock operator +(Variable left, VariableBlock right) =>
			AddVariableBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator -(VariableBlock left, Variable right) =>
			SubVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator -(VariableBlock left, IScriptVariableBlock right) => SubVariableBlock.Create(left, right);

		public static VariableBlock operator -(Variable left, VariableBlock right) =>
			SubVariableBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator *(VariableBlock left, Variable right) =>
			MulVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator *(VariableBlock left, IScriptVariableBlock right) => MulVariableBlock.Create(left, right);

		public static VariableBlock operator *(Variable left, VariableBlock right) =>
			MulVariableBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator /(VariableBlock left, Variable right) =>
			DivVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator /(VariableBlock left, IScriptVariableBlock right) => DivVariableBlock.Create(left, right);

		public static VariableBlock operator /(Variable left, VariableBlock right) =>
			DivVariableBlock.Create(ConstantVariableBlock.Create(left), right);

		public static VariableBlock operator ++(VariableBlock a) => a + 1;
		public static VariableBlock operator --(VariableBlock a) => a - 1;

		// Comparison Operators
		public static VariableBlock operator ==(VariableBlock left, Variable right) =>
			IsEqualToVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator ==(VariableBlock left, IScriptVariableBlock right) => IsEqualToVariableBlock.Create(left, right);

		public static VariableBlock operator !=(VariableBlock left, Variable right) =>
			IsNotEqualToVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator !=(VariableBlock left, IScriptVariableBlock right) =>
			IsNotEqualToVariableBlock.Create(left, right);

		public static VariableBlock operator >(VariableBlock left, Variable right) =>
			IsGreaterThanVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator >(VariableBlock left, IScriptVariableBlock right) =>
			IsGreaterThanVariableBlock.Create(left, right);

		public static VariableBlock operator >=(VariableBlock left, Variable right) =>
			IsAtLeastVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator >=(VariableBlock left, IScriptVariableBlock right) => IsAtLeastVariableBlock.Create(left, right);

		public static VariableBlock operator <(VariableBlock left, Variable right) =>
			IsLessThanVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator <(VariableBlock left, IScriptVariableBlock right) => IsLessThanVariableBlock.Create(left, right);

		public static VariableBlock operator <=(VariableBlock left, Variable right) =>
			IsAtMostVariableBlock.Create(left, ConstantVariableBlock.Create(right));

		public static VariableBlock operator <=(VariableBlock left, IScriptVariableBlock right) => IsAtMostVariableBlock.Create(left, right);

		// Unary Operators
		public static VariableBlock operator !(VariableBlock operand) => NotBlock.Create(operand);

		public override abstract Variable GetValue(IScriptRuntimeContext runtimeContext);
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

		public ScriptActionBlock Set(IScriptVariableBlock value) => AssignmentVariableBlock.Create(GetHandleOrThrow(), value);

		public ScriptActionBlock Inc() => Add(1);
		public ScriptActionBlock Dec() => Sub(1);

		public ScriptActionBlock Add(Variable value) => Set(this + value);
		public ScriptActionBlock Add(IScriptVariableBlock value) => Set(this + value);

		public ScriptActionBlock Sub(Variable value) => Set(this - value);
		public ScriptActionBlock Sub(IScriptVariableBlock value) => Set(this - value);

		public ScriptActionBlock Mul(Variable value) => Set(this * value);
		public ScriptActionBlock Mul(IScriptVariableBlock value) => Set(this * value);

		public ScriptActionBlock Div(Variable value) => Set(this / value);
		public ScriptActionBlock Div(IScriptVariableBlock value) => Set(this / value);

		public ScriptActionBlock Toggle() => Set(!this);

	}
}
