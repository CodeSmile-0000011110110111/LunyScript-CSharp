using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Destroys an instance of an engine object.
	/// </summary>
	internal sealed class ObjectDestroyBlock : IBlock
	{
		private String _objectName;

		public ObjectDestroyBlock(String name) => _objectName = name;

		public void Execute(IScriptContext context) => throw new NotImplementedException(nameof(ObjectDestroyBlock));
	}
}
