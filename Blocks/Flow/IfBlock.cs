using LunyScript.Execution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Builder for constructing 'If' blocks with 'ElseIf' and 'Else' branches.
	/// </summary>
	public sealed class IfBlockBuilder : IScriptActionBlock
	{
		private readonly List<(IScriptConditionBlock[] conditions, IScriptActionBlock[] blocks)> _branches = new();
		private IScriptActionBlock[] _elseBlocks;
		private IScriptActionBlock _cachedBlock;

		internal IfBlockBuilder(IScriptConditionBlock[] conditions) => _branches.Add((conditions, Array.Empty<IScriptActionBlock>()));

		public IfBlockBuilder Then(params IScriptActionBlock[] blocks)
		{
			var lastIndex = _branches.Count - 1;
			_branches[lastIndex] = (_branches[lastIndex].conditions, blocks);
			return this;
		}

		public IfBlockBuilder ElseIf(params IScriptConditionBlock[] conditions)
		{
			_branches.Add((conditions, Array.Empty<IScriptActionBlock>()));
			return this;
		}

		public IScriptActionBlock Else(params IScriptActionBlock[] blocks)
		{
			_elseBlocks = blocks;
			return Build();
		}

		public void Execute(ILunyScriptContext context) => (_cachedBlock ??= Build()).Execute(context);

		private IScriptActionBlock Build() => IfBlock.Create(_branches, _elseBlocks);
	}

	/// <summary>
	/// Conditional execution block.
	/// </summary>
	internal sealed class IfBlock : IScriptActionBlock
	{
		private readonly List<(IScriptConditionBlock[] conditions, IScriptActionBlock[] blocks)> _branches;
		private readonly IScriptActionBlock[] _elseBlocks;

		public static IfBlock Create(List<(IScriptConditionBlock[] conditions, IScriptActionBlock[] blocks)> branches, IScriptActionBlock[] elseBlocks) => 
			new(branches, elseBlocks);

		private IfBlock(List<(IScriptConditionBlock[] conditions, IScriptActionBlock[] blocks)> branches, IScriptActionBlock[] elseBlocks)
		{
			_branches = branches;
			_elseBlocks = elseBlocks;
		}

		public void Execute(ILunyScriptContext context)
		{
			foreach (var (conditions, blocks) in _branches)
			{
				if (EvaluateAll(context, conditions))
				{
					ExecuteAll(context, blocks);
					return;
				}
			}

			if (_elseBlocks != null)
				ExecuteAll(context, _elseBlocks);
		}

		private Boolean EvaluateAll(ILunyScriptContext context, IScriptConditionBlock[] conditions)
		{
			if (conditions == null || conditions.Length == 0)
				return true;

			foreach (var condition in conditions)
				if (condition == null || !condition.Evaluate(context))
					return false;

			return true;
		}

		private void ExecuteAll(ILunyScriptContext context, IScriptActionBlock[] blocks)
		{
			if (blocks == null)
				return;

			foreach (var block in blocks)
				block.Execute(context);
		}
	}
}
