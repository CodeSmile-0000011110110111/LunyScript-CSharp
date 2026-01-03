using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Destroys an instance of an engine object.
	/// </summary>
	internal sealed class ObjectDestroyBlock : IBlock
	{
		private String _objectName;

		internal ObjectDestroyBlock(String name) => _objectName = name;

		public void Execute(IScriptContext context)
		{
			if (String.IsNullOrEmpty(_objectName))
				context.LunyObject.Destroy();
			else
				throw new NotImplementedException($"{nameof(ObjectDestroyBlock)} for {_objectName} is not implemented.");
		}
	}
}
