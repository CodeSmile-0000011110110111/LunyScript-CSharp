using Luny;
using LunyScript.Execution;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class ArithmeticBlockBase : ScriptVariableBlockBase
	{
		protected readonly IScriptVariable _left;
		protected readonly IScriptVariable _right;

		internal override ScriptVariable Variable => (_left as ScriptVariableBlockBase)?.Variable;

		protected ArithmeticBlockBase(IScriptVariable left, IScriptVariable right)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));
		}
	}

	internal sealed class SetVariableBlock : IScriptActionBlock, IScriptVariable
	{
		private readonly Table.VarHandle _handle;
		private readonly IScriptVariable _value;

		public static SetVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private SetVariableBlock(Table.VarHandle handle, IScriptVariable value)
		{
			_handle = handle ?? throw new ArgumentNullException(nameof(handle));
			_value = value ?? throw new ArgumentNullException(nameof(value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Variable GetValue(ILunyScriptContext context) => _value.GetValue(context);

		override  public String ToString() => $"{_handle} = {_value}";
	}

	internal sealed class AddBlock : ArithmeticBlockBase
	{
		public static AddBlock Create(IScriptVariable left, IScriptVariable right) => new(left, right);
		private AddBlock(IScriptVariable left, IScriptVariable right) : base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _left.GetValue(context) + (Double)_right.GetValue(context);
	}

	internal sealed class SubBlock : ArithmeticBlockBase
	{
		public static SubBlock Create(IScriptVariable left, IScriptVariable right) => new(left, right);
		private SubBlock(IScriptVariable left, IScriptVariable right) : base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _left.GetValue(context) - (Double)_right.GetValue(context);
	}

	internal sealed class MulBlock : ArithmeticBlockBase
	{
		public static MulBlock Create(IScriptVariable left, IScriptVariable right) => new(left, right);
		private MulBlock(IScriptVariable left, IScriptVariable right) : base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _left.GetValue(context) * (Double)_right.GetValue(context);
	}

	internal sealed class DivBlock : ArithmeticBlockBase
	{
		public static DivBlock Create(IScriptVariable left, IScriptVariable right) => new(left, right);
		private DivBlock(IScriptVariable left, IScriptVariable right) : base(left, right) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _left.GetValue(context) / (Double)_right.GetValue(context);
	}
}
