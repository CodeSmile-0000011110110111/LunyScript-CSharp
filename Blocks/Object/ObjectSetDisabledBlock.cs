using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Changes enabled state of an object.
	/// Enabled means the object is visible, receives update events, and participates in physics simulation (collisions).
	/// Disable an object to take it out of the game loop temporarily without destroying it.
	/// </summary>
	internal sealed class ObjectSetDisabledBlock : ILunyScriptBlock
	{
		private readonly String _name;

		internal ObjectSetDisabledBlock(String name) => _name = name;

		public void Execute(ILunyScriptContext context)
		{
			if (String.IsNullOrEmpty(_name))
				context.LunyObject.IsEnabled = false;
			else
				throw new NotImplementedException($"{nameof(ObjectSetDisabledBlock)} with name '{_name}' not implemented");
		}
	}
}
