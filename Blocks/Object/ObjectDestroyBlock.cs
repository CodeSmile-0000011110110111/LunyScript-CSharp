using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Destroys an instance of an engine object.
	/// </summary>
	internal sealed class ObjectDestroyBlock : ILunyScriptBlock
	{
		private String _objectName;

		public static ILunyScriptBlock Create(String name) => new ObjectDestroyBlock(name);

		private ObjectDestroyBlock() {}
		private ObjectDestroyBlock(String name) => _objectName = name;

		public void Execute(ILunyScriptContext context)
		{
			if (String.IsNullOrEmpty(_objectName))
				context.LunyObject.Destroy();
			else
			{
				var target = LunyEngine.Instance.Objects.GetByName(_objectName);
				target?.Destroy();
			}
		}
	}
}
