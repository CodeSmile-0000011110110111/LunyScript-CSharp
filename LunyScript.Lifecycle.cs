using LunyScript.Interfaces;
using LunyScript.Runnables;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
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
