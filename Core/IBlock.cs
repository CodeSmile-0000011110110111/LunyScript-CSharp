namespace LunyScript
{
	/// <summary>
	/// Base interface for all executable blocks (leaf and container).
	/// Leaf blocks contain logic but no child blocks.
	/// </summary>
	public interface IBlock
	{
		void Execute(IScriptContext context);
	}
}
