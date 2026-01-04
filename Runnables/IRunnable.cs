using LunyScript.Blocks;
using System;
using System.Collections.Generic;

namespace LunyScript.Runnables
{
	/// <summary>
	/// Container blocks that can be executed by LunyScriptRunner.
	/// Runnables have IDs and can contain child blocks.
	/// Examples: RunnableSequence, RunnableStateMachine, RunnableBehaviorTree.
	/// </summary>
	public interface IRunnable : ILunyScriptBlock
	{
		RunnableID ID { get; }
		IReadOnlyList<ILunyScriptBlock> Children { get; }
		Boolean IsEmpty { get; }
	}
}
