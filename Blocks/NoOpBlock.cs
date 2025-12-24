using LunyScript.Interfaces;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Does nothing. Used as placeholder in case user leaves a runnable empty.
	/// </summary>
	internal sealed class NoOpBlock : IBlock
	{
		public void Execute(ScriptContext context) {}
	}
}
