using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class ArithmeticVariableBlock : VariableBlock
	{
		protected readonly VariableBlock _left;
		protected readonly VariableBlock _right;

		internal override Table.VarHandle TargetHandle => (_left as VariableBlock)?.TargetHandle ?? (_right as VariableBlock)?.TargetHandle;

		protected ArithmeticVariableBlock(VariableBlock left, VariableBlock right)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));
		}
	}

	internal sealed class AddVariableBlock : ArithmeticVariableBlock
	{
		public static AddVariableBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private AddVariableBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) + (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class SubVariableBlock : ArithmeticVariableBlock
	{
		public static SubVariableBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private SubVariableBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) - (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class MulVariableBlock : ArithmeticVariableBlock
	{
		public static MulVariableBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private MulVariableBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) * (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class DivVariableBlock : ArithmeticVariableBlock
	{
		public static DivVariableBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private DivVariableBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) / (Double)_right.GetValue(runtimeContext);
	}
}
