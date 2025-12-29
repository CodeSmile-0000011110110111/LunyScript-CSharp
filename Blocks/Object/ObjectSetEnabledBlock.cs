namespace LunyScript.Blocks
{
	/// <summary>
	/// Changes enabled state of an object.
	/// Enabled means the object is visible, receives update events, and participates in physics simulation (collisions).
	/// Disable an object to take it out of the game loop temporarily without destroying it.
	/// </summary>
	internal sealed class ObjectSetEnabledBlock : IBlock
	{
		public void Execute(IScriptContext context) => context.SetObjectEnabled(true);
	}
}
