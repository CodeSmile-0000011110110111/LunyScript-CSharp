using Luny;
using LunyScript.Execution;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical AND condition block.
	/// </summary>
	internal sealed class AndBlock : VariableBlock
	{
		private readonly IScriptConditionBlock[] _conditions;

		public static AndBlock Create(params IScriptConditionBlock[] conditions) => new(conditions);

		private AndBlock(IScriptConditionBlock[] conditions) => _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => Evaluate(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(ILunyScriptContext context)
		{
			foreach (var condition in _conditions)
			{
				if (!condition.Evaluate(context))
					return false;
			}

			return true;
		}
	}
}
