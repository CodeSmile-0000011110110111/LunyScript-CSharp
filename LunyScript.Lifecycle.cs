using Luny.Diagnostics;
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
			(_context as ScriptContext).ScheduleRunnable(runnable, ObjectLifecycleEvents.OnFixedStep);
		}

		protected void OnUpdate(params IBlock[] blocks)
		{
			if (blocks?.Length == 0)
				return;

			var runnable = new RunnableSequence(blocks);
			(_context as ScriptContext).ScheduleRunnable(runnable, ObjectLifecycleEvents.OnUpdate);
		}

		protected void OnLateUpdate(params IBlock[] blocks)
		{
			if (blocks?.Length == 0)
				return;

			var runnable = new RunnableSequence(blocks);
			(_context as ScriptContext).ScheduleRunnable(runnable, ObjectLifecycleEvents.OnLateUpdate);
		}
	}
}
