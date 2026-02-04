using Luny;
using LunyScript.Execution;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	public sealed class ReferenceVariableBlock : VariableBlock
	{
		private readonly Table.VarHandle _handle;

		internal override Table.VarHandle TargetHandle => _handle;
		internal Table.VarHandle VarHandle => _handle;

		public String Name => _handle.Name;
		public Variable Value => _handle.Value;

		internal static ReferenceVariableBlock From(Table.VarHandle handle) => new(handle);

		private ReferenceVariableBlock(Table.VarHandle handle) => _handle = handle;

		public override String ToString() => _handle.ToString();

		// IScriptVariableBlock
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _handle.Value;
	}

	/// <summary>
	/// Block that returns a constant variable value.
	/// </summary>
	public sealed class ConstantVariableBlock : VariableBlock
	{
		private readonly Variable _value;

		public static ConstantVariableBlock Create(Variable value) => new(value);

		private ConstantVariableBlock(Variable value) => _value = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _value;

		public override String ToString() => _value.ToString();
	}

	/// <summary>
	/// Block that returns the current loop counter from the context stack.
	/// </summary>
	internal sealed class LoopCounterVariableBlock : VariableBlock
	{
		public static readonly LoopCounterVariableBlock Instance = new();
		private LoopCounterVariableBlock() {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => context.LoopCount;
	}
}
