using LunyScript.Exceptions;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Builder for 'For' loops.
	/// </summary>
	public sealed class ForBlockBuilder : IScriptActionBlock
	{
		private readonly Int32 _limit;
		private readonly Int32 _step;
		private IScriptActionBlock[] _blocks;
		private IScriptActionBlock _cachedBlock;

		internal ForBlockBuilder(Int32 limit, Int32 step = 1)
		{
			_limit = limit;
			_step = step;
		}

		public void Execute(IScriptRuntimeContext runtimeContext) => (_cachedBlock ??= Build()).Execute(runtimeContext);

		public IScriptActionBlock Do(params IScriptActionBlock[] blocks)
		{
			_blocks = blocks;
			return Build();
		}

		private IScriptActionBlock Build() => ForBlock.Create(_limit, _step, _blocks);
	}

	/// <summary>
	/// For loop execution block with 1-based indexing and safety limits.
	/// </summary>
	internal sealed class ForBlock : IScriptActionBlock
	{
		private readonly Int32 _limit;
		private readonly Int32 _step;
		private readonly IScriptActionBlock[] _blocks;

		public static ForBlock Create(Int32 limit, Int32 step, IScriptActionBlock[] blocks) => new(limit, step, blocks);

		private ForBlock(Int32 limit, Int32 step, IScriptActionBlock[] blocks)
		{
			_limit = limit;
			_step = step == 0 ? 1 : step; // Prevent division by zero/infinite loop if step is 0
			_blocks = blocks;
		}

		public void Execute(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || UNITY_EDITOR
			var iterations = 0;
#endif

			var loopStack = runtimeContext.LoopStack;
			var maxLimit = LunyScriptEngine.MaxLoopIterations;

			if (_step > 0)
			{
				for (var i = 1; i <= _limit; i += _step)
				{
#if DEBUG || UNITY_EDITOR
					if (++iterations > maxLimit)
						throw new LunyScriptMaxIterationException(nameof(ForBlock), maxLimit);
#endif
					loopStack.Push(i);
					try
					{
						ExecuteAll(runtimeContext);
					}
					finally
					{
						loopStack.Pop();
					}
				}
			}
			else
			{
				for (var i = _limit; i >= 1; i += _step)
				{
#if DEBUG || UNITY_EDITOR
					if (++iterations > maxLimit)
						throw new LunyScriptMaxIterationException(nameof(ForBlock), maxLimit);
#endif
					loopStack.Push(i);
					try
					{
						ExecuteAll(runtimeContext);
					}
					finally
					{
						loopStack.Pop();
					}
				}
			}
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
