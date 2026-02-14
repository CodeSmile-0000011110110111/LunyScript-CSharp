using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for sequence blocks that contain child action blocks.
	/// </summary>
	public abstract class ScriptSequenceBlock : ScriptActionBlock
	{
		public abstract ScriptBlockID ID { get; }
		public abstract IReadOnlyList<ScriptActionBlock> Blocks { get; }
		public abstract Boolean IsEmpty { get; }
	}
}
