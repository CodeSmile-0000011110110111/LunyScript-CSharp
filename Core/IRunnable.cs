using Luny.Core;
using System;
using System.Collections.Generic;

namespace LunyScript
{
	/// <summary>
	/// Container blocks that can be executed by LunyScriptRunner.
	/// Runnables have IDs and can contain child blocks.
	/// Examples: RunnableSequence, RunnableStateMachine, RunnableBehaviorTree.
	/// </summary>
	public interface IRunnable : IBlock
	{
		SequenceID ID { get; }
		IReadOnlyList<IBlock> Children { get; }
	}
}
