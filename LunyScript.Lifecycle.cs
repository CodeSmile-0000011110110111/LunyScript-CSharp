using LunyScript.Execution;
using LunyScript.Interfaces;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		// User-facing API: Runnable registration
		protected void OnFixedStep(params IBlock[] blocks)
		{
			if (blocks?.Length == 0)
				return;

			var runnable = new RunnableSequence(blocks);
			_context.RunnablesScheduledInFixedStep.Add(runnable);
		}

		protected void OnUpdate(params IBlock[] blocks)
		{
			if (blocks?.Length == 0)
				return;

			var runnable = new RunnableSequence(blocks);
			_context.RunnablesScheduledInUpdate.Add(runnable);
		}

		protected void OnLateUpdate(params IBlock[] blocks)
		{
			if (blocks?.Length == 0)
				return;

			var runnable = new RunnableSequence(blocks);
			_context.RunnablesScheduledInLateUpdate.Add(runnable);
		}
	}
}
