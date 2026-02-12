using LunyScript.Exceptions;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Builder for 'While' loops.
	/// </summary>
	public sealed class WhileBlockBuilder : IScriptActionBlock
	{
		private readonly IScriptConditionBlock[] _conditions;
		private IScriptActionBlock[] _blocks;
		private IScriptActionBlock _cachedBlock;

		internal WhileBlockBuilder(IScriptConditionBlock[] conditions) => _conditions = conditions;

		public void Execute(IScriptRuntimeContext runtimeContext) => (_cachedBlock ??= Build()).Execute(runtimeContext);

		public IScriptActionBlock Do(params IScriptActionBlock[] blocks)
		{
			_blocks = blocks;
			return Build();
		}

		private IScriptActionBlock Build() => WhileBlock.Create(_conditions, _blocks);
	}

	/// <summary>
	/// While loop execution block with safety limits.
	/// </summary>
	internal sealed class WhileBlock : IScriptActionBlock
	{
		private readonly IScriptConditionBlock[] _conditions;
		private readonly IScriptActionBlock[] _blocks;

		public static WhileBlock Create(IScriptConditionBlock[] conditions, IScriptActionBlock[] blocks) => new(conditions, blocks);

		private WhileBlock(IScriptConditionBlock[] conditions, IScriptActionBlock[] blocks)
		{
			_conditions = conditions;
			_blocks = blocks;
		}

		public void Execute(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || UNITY_EDITOR
			var iterations = 0;
#endif

			var limit = ScriptEngine.MaxLoopIterations;

			while (EvaluateAll(runtimeContext))
			{
#if DEBUG || UNITY_EDITOR
				if (++iterations > limit)
					throw new LunyScriptMaxIterationException(nameof(WhileBlock), limit);
#endif
				ExecuteAll(runtimeContext);
			}
		}

		private Boolean EvaluateAll(IScriptRuntimeContext runtimeContext)
		{
			if (_conditions == null || _conditions.Length == 0)
				return false; // Infinite loop prevention if no conditions

			foreach (var condition in _conditions)
			{
				if (!condition.Evaluate(runtimeContext))
					return false;
			}

			return true;
		}

		private void ExecuteAll(IScriptRuntimeContext runtimeContext)
		{
			if (_blocks == null)
				return;

			foreach (var block in _blocks)
				block.Execute(runtimeContext);
		}
	}
}
