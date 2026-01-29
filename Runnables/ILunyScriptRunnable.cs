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
	public interface ILunyScriptRunnable : IScriptActionBlock
	{
		LunyScriptRunID ID { get; }
		IReadOnlyList<IScriptActionBlock> Blocks { get; }
		Boolean IsEmpty { get; }
	}
}
