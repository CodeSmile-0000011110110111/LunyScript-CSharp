using Luny;
using LunyScript.Execution;
using LunyScript.Interfaces;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		private static Boolean HasBlocks(IBlock[] blocks) => blocks?.Length > 0;
		private static RunnableSequence CreateSequence(IBlock[] blocks) => HasBlocks(blocks) ? new RunnableSequence(blocks) : null;

		private void Schedule(RunnableSequence sequence, ObjectLifecycleEvents eventType) =>
			(_context as ScriptContext).ScheduleRunnable(sequence, eventType);
	}
}
