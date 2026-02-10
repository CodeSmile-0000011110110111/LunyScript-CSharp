using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class ComparisonVariableBlock : VariableBlock
	{
		protected readonly IScriptVariableBlock _left;
		protected readonly IScriptVariableBlock _right;

		internal override Table.VarHandle TargetHandle => (_left as VariableBlock)?.TargetHandle ?? (_right as VariableBlock)?.TargetHandle;

		protected ComparisonVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right = null)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right;
		}
	}

	internal sealed class IsEqualToVariableBlock : ComparisonVariableBlock
	{
		public static IsEqualToVariableBlock Create(IScriptVariableBlock left, IScriptVariableBlock right) => new(left, right);

		private IsEqualToVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
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
		public static IsNotEqualToVariableBlock Create(IScriptVariableBlock left, IScriptVariableBlock right) => new(left, right);

		private IsNotEqualToVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) != _right.GetValue(runtimeContext);
	}

	internal sealed class IsGreaterThanVariableBlock : ComparisonVariableBlock
	{
		public static IsGreaterThanVariableBlock Create(IScriptVariableBlock left, IScriptVariableBlock right) => new(left, right);

		private IsGreaterThanVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) > (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class IsAtLeastVariableBlock : ComparisonVariableBlock
	{
		public static IsAtLeastVariableBlock Create(IScriptVariableBlock left, IScriptVariableBlock right) => new(left, right);

		private IsAtLeastVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) >= (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class IsLessThanVariableBlock : ComparisonVariableBlock
	{
		public static IsLessThanVariableBlock Create(IScriptVariableBlock left, IScriptVariableBlock right) => new(left, right);

		private IsLessThanVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) < (Double)_right.GetValue(runtimeContext);
	}

	internal sealed class IsAtMostVariableBlock : ComparisonVariableBlock
	{
		public static IsAtMostVariableBlock Create(IScriptVariableBlock left, IScriptVariableBlock right) => new(left, right);

		private IsAtMostVariableBlock(IScriptVariableBlock left, IScriptVariableBlock right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_left.GetValue(runtimeContext) <= (Double)_right.GetValue(runtimeContext);
	}
}
