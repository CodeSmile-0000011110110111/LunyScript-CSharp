using Luny;
using LunyScript.Execution;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class ComparisonBlockBase : ScriptVariableBlockBase
	{
		protected readonly IScriptVariable _left;
		protected readonly IScriptVariable _right;

		internal override ScriptVariable Variable => (_left as ScriptVariableBlockBase)?.Variable;

		protected ComparisonBlockBase(IScriptVariable left, IScriptVariable right = null)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right;
		}
	}

	internal sealed class IsEqualToBlock : ComparisonBlockBase
	{
		public static IsEqualToBlock Create(IScriptVariable left, IScriptVariable right) => new(left, right);

		private IsEqualToBlock(IScriptVariable left, IScriptVariable right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => Evaluate(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(ILunyScriptContext context) => _left.GetValue(context) == _right.GetValue(context);

		public override String ToString() => $"{_left} == {_right}";
	}

	internal sealed class IsNotEqualToBlock : ComparisonBlockBase
	{
		public static IsNotEqualToBlock Create(IScriptVariable left, IScriptVariable right) => new(left, right);

		private IsNotEqualToBlock(IScriptVariable left, IScriptVariable right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => Evaluate(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(ILunyScriptContext context) => _left.GetValue(context) != _right.GetValue(context);
	}

	internal sealed class IsGreaterThanBlock : ComparisonBlockBase
	{
		public static IsGreaterThanBlock Create(IScriptVariable left, IScriptVariable right) => new(left, right);

		private IsGreaterThanBlock(IScriptVariable left, IScriptVariable right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => Evaluate(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(ILunyScriptContext context) => _left.GetValue(context) > (Double)_right.GetValue(context);
	}

	internal sealed class IsAtLeastBlock : ComparisonBlockBase
	{
		public static IsAtLeastBlock Create(IScriptVariable left, IScriptVariable right) => new(left, right);

		private IsAtLeastBlock(IScriptVariable left, IScriptVariable right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => Evaluate(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(ILunyScriptContext context) => _left.GetValue(context) >= (Double)_right.GetValue(context);
	}

	internal sealed class IsLessThanBlock : ComparisonBlockBase
	{
		public static IsLessThanBlock Create(IScriptVariable left, IScriptVariable right) => new(left, right);

		private IsLessThanBlock(IScriptVariable left, IScriptVariable right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => Evaluate(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(ILunyScriptContext context) => _left.GetValue(context) < (Double)_right.GetValue(context);
	}

	internal sealed class IsAtMostBlock : ComparisonBlockBase
	{
		public static IsAtMostBlock Create(IScriptVariable left, IScriptVariable right) => new(left, right);

		private IsAtMostBlock(IScriptVariable left, IScriptVariable right)
			: base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => Evaluate(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(ILunyScriptContext context) => _left.GetValue(context) <= (Double)_right.GetValue(context);
	}
}
