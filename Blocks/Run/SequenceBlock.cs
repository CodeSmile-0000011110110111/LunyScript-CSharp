using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Executes child blocks in sequential order.
	/// </summary>
	internal sealed class SequenceBlock : ScriptSequenceBlock
	{
		public override ScriptBlockID ID { get; }
		public override IReadOnlyList<ScriptActionBlock> Blocks { get; }
		public override Boolean IsEmpty => Blocks.Count == 0;

		public static ScriptSequenceBlock TryCreate(IReadOnlyList<ScriptActionBlock> blocks) =>
			blocks?.Count > 0 ? new SequenceBlock(blocks) : null;

		public SequenceBlock(IReadOnlyList<ScriptActionBlock> blocks)
		{
			if (blocks == null || blocks.Count == 0)
				throw new ArgumentException("Sequence must contain at least one block", nameof(blocks));

			ID = ScriptBlockID.Generate();
			Blocks = blocks;
		}

		public override void Execute(IScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext == null)
				return;

			foreach (var block in Blocks)
				block?.Execute(runtimeContext);
		}

		//~LunyScriptBlockSequence() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
