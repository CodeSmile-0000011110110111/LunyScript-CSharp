using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Changes enabled state of an object.
	/// Enabled means the object is visible, receives update events, and participates in physics simulation (collisions).
	/// Disable an object to take it out of the game loop temporarily without destroying it.
	/// </summary>
	internal sealed class ObjectSetEnabledBlock : IBlock
	{
		private readonly String _name;

		internal ObjectSetEnabledBlock(String name) => _name = name;

		public void Execute(IScriptContext context)
		{
			if (String.IsNullOrEmpty(_name))
				context.LunyObject.IsEnabled = true;
			else
				throw new NotImplementedException($"{nameof(ObjectSetEnabledBlock)} with name '{_name}' not implemented");
		}
	}
}
