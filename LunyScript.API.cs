using LunyScript.Blocks;
using LunyScript.Runnables;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		// User-facing API: Variables
		protected Variables Variables => _context.Variables;
		protected Variables GlobalVariables => _context.GlobalVariables;
		protected Variables InspectorVariables => _context.InspectorVariables;

		// User-facing API: Block factory methods
		protected static LogMessageBlock Log(String message) => new(message);
		protected ActionBlock Do(Action action) => new(_ => action());

		// User-facing API: Runnable registration
		protected void OnUpdate(params IBlock[] blocks)
		{
			var runnable = new RunnableSequence(blocks);
			_context.UpdateRunnables.Add(runnable);
		}

		protected void OnFixedStep(params IBlock[] blocks)
		{
			var runnable = new RunnableSequence(blocks);
			_context.FixedStepRunnables.Add(runnable);
		}

		protected void OnLateUpdate(params IBlock[] blocks)
		{
			var runnable = new RunnableSequence(blocks);
			_context.LateUpdateRunnables.Add(runnable);
		}
	}
}
