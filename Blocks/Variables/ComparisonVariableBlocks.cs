using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class ComparisonVariableBlock : VariableBlock
	{
		protected readonly VariableBlock _left;
		protected readonly VariableBlock _right;

		internal override Table.VarHandle TargetHandle => (_left as VariableBlock)?.TargetHandle ?? (_right as VariableBlock)?.TargetHandle;

		protected ComparisonVariableBlock(VariableBlock left, VariableBlock right = null)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right;
		}
	}

	internal sealed class IsEqualToVariableBlock : ComparisonVariableBlock
	{
		public static IsEqualToVariableBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private IsEqualToVariableBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) == _right.GetValue(runtimeContext);

		public override String ToString() => $"{_left} == {_right}";
	}

	internal sealed class IsNotEqualToVariableBlock : ComparisonVariableBlock
	{
		public static IsNotEqualToVariableBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private IsNotEqualToVariableBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) != _right.GetValue(runtimeContext);
	}

	internal sealed class IsGreaterThanVariableBlock : ComparisonVariableBlock
	{
		public static IsGreaterThanVariableBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private IsGreaterThanVariableBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) > (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class IsAtLeastVariableBlock : ComparisonVariableBlock
	{
		public static IsAtLeastVariableBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private IsAtLeastVariableBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) >= (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class IsLessThanVariableBlock : ComparisonVariableBlock
	{
		public static IsLessThanVariableBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private IsLessThanVariableBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) < (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class IsAtMostVariableBlock : ComparisonVariableBlock
	{
		public static IsAtMostVariableBlock Create(VariableBlock left, VariableBlock right) => new(left, right);

		private IsAtMostVariableBlock(VariableBlock left, VariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) <= (Double)_right.GetValue(runtimeContext);
	}
}
