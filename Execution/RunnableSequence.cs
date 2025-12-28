using LunyScript.Interfaces;
using System;
using System.Collections.Generic;

namespace LunyScript.Execution
{
	/// <summary>
	/// Executes child blocks in sequential order.
	/// </summary>
	public sealed class RunnableSequence : IRunnable
	{
		public RunnableID ID { get; }
		public IReadOnlyList<IBlock> Children { get; }

		public RunnableSequence(IReadOnlyList<IBlock> blocks)
		{
			if (blocks == null || blocks.Count == 0)
				throw new ArgumentException("Sequence must contain at least one block", nameof(blocks));

			ID = RunnableID.Generate();
			Children = blocks;
		}

		public void Execute(ScriptContext context)
		{
			foreach (var block in Children)
				block.Execute(context);
		}
	}
}
