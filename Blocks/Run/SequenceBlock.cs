using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Executes child blocks in sequential order.
	/// </summary>
	internal sealed class SequenceBlock : IScriptSequenceBlock
	{
		public ScriptBlockID ID { get; }
		public IReadOnlyList<IScriptActionBlock> Blocks { get; }
		public Boolean IsEmpty => Blocks.Count == 0;

		public static IScriptSequenceBlock TryCreate(IReadOnlyList<IScriptActionBlock> blocks) =>
			blocks?.Count > 0 ? new SequenceBlock(blocks) : null;

		public SequenceBlock(IReadOnlyList<IScriptActionBlock> blocks)
		{
			if (blocks == null || blocks.Count == 0)
				throw new ArgumentException("Sequence must contain at least one block", nameof(blocks));

			ID = ScriptBlockID.Generate();
			Blocks = blocks;
		}

		public void Execute(IScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext == null)
				return;

			foreach (var block in Blocks)
				block?.Execute(runtimeContext);
		}

		//~LunyScriptBlockSequence() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
