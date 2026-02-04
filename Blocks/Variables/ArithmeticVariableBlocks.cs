using Luny;
using LunyScript.Execution;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class ArithmeticVariableBlock : VariableBlock
	{
		protected readonly IScriptVariableBlock _left;
		protected readonly IScriptVariableBlock _right;

		internal override Table.VarHandle TargetHandle => (_left as VariableBlock)?.TargetHandle ?? (_right as VariableBlock)?.TargetHandle;

		protected ArithmeticVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));
		}
	}

	internal sealed class AssignmentVariableBlock : IScriptActionBlock, IScriptVariableBlock
	{
		private readonly Table.VarHandle _handle;
		private readonly IScriptVariableBlock _value;

		public static AssignmentVariableBlock Create(Table.VarHandle handle, IScriptVariableBlock value) => new(handle, value);

		private AssignmentVariableBlock(Table.VarHandle handle, IScriptVariableBlock value)
		{
			_handle = handle ?? throw new ArgumentNullException(nameof(handle));
			_value = value ?? throw new ArgumentNullException(nameof(value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Variable GetValue(ILunyScriptContext context) => _value.GetValue(context);

		public override String ToString() => $"{_handle} = {_value}";
	}

	internal sealed class AddVariableBlock : ArithmeticVariableBlock
	{
		public static AddVariableBlock Create(IScriptVariableBlock left, IScriptVariableBlock right) => new(left, right);

		private AddVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _left.GetValue(context) + (Double)_right.GetValue(context);
	}

	internal sealed class SubVariableBlock : ArithmeticVariableBlock
	{
		public static SubVariableBlock Create(IScriptVariableBlock left, IScriptVariableBlock right) => new(left, right);

		private SubVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _left.GetValue(context) - (Double)_right.GetValue(context);
	}

	internal sealed class MulVariableBlock : ArithmeticVariableBlock
	{
		public static MulVariableBlock Create(IScriptVariableBlock left, IScriptVariableBlock right) => new(left, right);

		private MulVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _left.GetValue(context) * (Double)_right.GetValue(context);
	}

	internal sealed class DivVariableBlock : ArithmeticVariableBlock
	{
		public static DivVariableBlock Create(IScriptVariableBlock left, IScriptVariableBlock right) => new(left, right);

		private DivVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _left.GetValue(context) / (Double)_right.GetValue(context);
	}
}
