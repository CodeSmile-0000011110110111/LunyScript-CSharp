using System;
using System.Collections.Generic;

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

		public void Execute(IScriptRuntimeContext runtimeContext) => (_cachedBlock ??= Build()).Execute(runtimeContext);

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

		private IScriptActionBlock Build() => IfBlock.Create(_branches, _elseBlocks);
	}

	/// <summary>
	/// Conditional execution block.
	/// </summary>
	internal sealed class IfBlock : IScriptActionBlock
	{
		private readonly List<(IScriptConditionBlock[] conditions, IScriptActionBlock[] blocks)> _branches;
		private readonly IScriptActionBlock[] _elseBlocks;

		public static IfBlock Create(List<(IScriptConditionBlock[] conditions, IScriptActionBlock[] blocks)> branches,
			IScriptActionBlock[] elseBlocks) => new(branches, elseBlocks);

		private IfBlock(List<(IScriptConditionBlock[] conditions, IScriptActionBlock[] blocks)> branches, IScriptActionBlock[] elseBlocks)
		{
			_branches = branches;
			_elseBlocks = elseBlocks;
		}

		public void Execute(IScriptRuntimeContext runtimeContext)
		{
			foreach (var (conditions, blocks) in _branches)
			{
				if (EvaluateAll(runtimeContext, conditions))
				{
					ExecuteAll(runtimeContext, blocks);
					return;
				}
			}

			if (_elseBlocks != null)
				ExecuteAll(runtimeContext, _elseBlocks);
		}

		private Boolean EvaluateAll(IScriptRuntimeContext runtimeContext, IScriptConditionBlock[] conditions)
		{
			if (conditions == null || conditions.Length == 0)
				return true;

			foreach (var condition in conditions)
			{
				if (condition == null || !condition.Evaluate(runtimeContext))
					return false;
			}

			return true;
		}

		private void ExecuteAll(IScriptRuntimeContext runtimeContext, IScriptActionBlock[] blocks)
		{
			if (blocks == null)
				return;

			foreach (var block in blocks)
				block.Execute(runtimeContext);
		}
	}
}
