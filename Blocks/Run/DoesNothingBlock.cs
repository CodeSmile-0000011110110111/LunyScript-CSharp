using LunyScript.Execution;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Does nothing. Used as placeholder in case user leaves a runnable empty.
	/// </summary>
	internal sealed class DoesNothingBlock : IBlock
	{
		public void Execute(ILunyScriptContext context) {}
	}
}
