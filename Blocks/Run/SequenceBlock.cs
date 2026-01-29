using LunyScript.Execution;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Executes child blocks in sequential order.
	/// </summary>
	public sealed class SequenceBlock : IScriptSequenceBlock
	{
		public LunyScriptRunID ID { get; }
		public IReadOnlyList<IScriptActionBlock> Blocks { get; }
		public Boolean IsEmpty => Blocks.Count == 0;

		public SequenceBlock(IReadOnlyList<IScriptActionBlock> blocks)
		{
			if (blocks == null || blocks.Count == 0)
				throw new ArgumentException("Sequence must contain at least one block", nameof(blocks));

			ID = LunyScriptRunID.Generate();
			Blocks = blocks;
		}

		public void Execute(ILunyScriptContext context)
		{
			foreach (var block in Blocks)
				block?.Execute(context);
		}

		//~LunyScriptBlockSequence() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
