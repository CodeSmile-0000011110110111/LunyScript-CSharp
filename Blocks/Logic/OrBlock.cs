using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical OR condition block.
	/// </summary>
	internal sealed class OrBlock : VariableBlock
	{
		private readonly IScriptConditionBlock[] _conditions;

		public static OrBlock Create(params IScriptConditionBlock[] conditions) => new(conditions);

		private OrBlock(IScriptConditionBlock[] conditions) => _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext)
		{
			foreach (var condition in _conditions)
			{
				if (condition.Evaluate(runtimeContext))
					return true;
			}

			return false;
		}
	}
}
