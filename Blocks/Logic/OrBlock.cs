using Luny;
using LunyScript.Execution;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical OR condition block.
	/// </summary>
	internal sealed class OrBlock : ScriptVariableBlockBase
	{
		private readonly IScriptConditionBlock[] _conditions;

		public static OrBlock Create(params IScriptConditionBlock[] conditions) => new(conditions);

		private OrBlock(IScriptConditionBlock[] conditions) => _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => Evaluate(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(ILunyScriptContext context)
		{
			foreach (var condition in _conditions)
			{
				if (condition.Evaluate(context))
					return true;
			}

			return false;
		}
	}
}
