namespace LunyScript.Blocks
{
	/// <summary>
	/// Represents a coroutine block that extends timer functionality with per-frame/heartbeat execution.
	/// Coroutines can run blocks on each update/heartbeat while running.
	/// </summary>
	public interface IScriptCoroutineBlock : IScriptTimerBlock
	{
		// Inherits all timer control methods.
		// Coroutines differ from timers in that they can run blocks per frame/heartbeat,
		// not just when elapsed.
	}
}
