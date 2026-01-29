using LunyScript.Execution;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical AND condition block.
	/// </summary>
	internal sealed class AndBlock : IScriptConditionBlock
	{
		private readonly IScriptConditionBlock[] _conditions;

		public static IScriptConditionBlock Create(params IScriptConditionBlock[] conditions) => new AndBlock(conditions);

		private AndBlock(IScriptConditionBlock[] conditions) => _conditions = conditions;

		public Boolean Evaluate(ILunyScriptContext context)
		{
			if (_conditions == null || _conditions.Length == 0)
				return true;

			foreach (var condition in _conditions)
				if (!condition.Evaluate(context))
					return false;

			return true;
		}
	}

	/// <summary>
	/// Logical OR condition block.
	/// </summary>
	internal sealed class OrBlock : IScriptConditionBlock
	{
		private readonly IScriptConditionBlock[] _conditions;

		public static IScriptConditionBlock Create(params IScriptConditionBlock[] conditions) => new OrBlock(conditions);

		private OrBlock(IScriptConditionBlock[] conditions) => _conditions = conditions;

		public Boolean Evaluate(ILunyScriptContext context)
		{
			if (_conditions == null || _conditions.Length == 0)
				return false;

			foreach (var condition in _conditions)
				if (condition.Evaluate(context))
					return true;

			return false;
		}
	}

	/// <summary>
	/// Logical NOT condition block.
	/// </summary>
	internal sealed class NotBlock : IScriptConditionBlock
	{
		private readonly IScriptConditionBlock _condition;

		public static IScriptConditionBlock Create(IScriptConditionBlock condition) => new NotBlock(condition);

		private NotBlock(IScriptConditionBlock condition) => _condition = condition;

		public Boolean Evaluate(ILunyScriptContext context) => _condition == null || !_condition.Evaluate(context);
	}
}
