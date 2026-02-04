using Luny;
using LunyScript.Execution;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical NOT condition block.
	/// </summary>
	internal sealed class NotBlock : VariableBlock
	{
		private readonly IScriptConditionBlock _condition;

		internal override Table.VarHandle TargetHandle => (_condition as VariableBlock)?.TargetHandle;

		public static NotBlock Create(IScriptConditionBlock condition) => new(condition);

		private NotBlock(IScriptConditionBlock condition) => _condition = condition;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => Evaluate(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(ILunyScriptContext context) => _condition == null || !_condition.Evaluate(context);
	}
}
