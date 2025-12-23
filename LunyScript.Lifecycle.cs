using LunyScript.Execution;
using LunyScript.Interfaces;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		// User-facing API: Runnable registration
		protected void OnUpdate(params IBlock[] blocks)
		{
			var runnable = new RunnableSequence(blocks);
			_context.RunnablesScheduledInUpdate.Add(runnable);
		}

		protected void OnFixedStep(params IBlock[] blocks)
		{
			var runnable = new RunnableSequence(blocks);
			_context.RunnablesScheduledInFixedStep.Add(runnable);
		}

		protected void OnLateUpdate(params IBlock[] blocks)
		{
			var runnable = new RunnableSequence(blocks);
			_context.RunnablesScheduledInLateUpdate.Add(runnable);
		}
	}
}
