using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Represents a coroutine block that runs perpetually (indefinitely).
	/// Coroutines can be started, stopped, paused, resumed.
	/// </summary>
	public interface ICoroutineBlock : IScriptBlock
	{
		/// <summary>
		/// Starts or restarts the coroutine.
		/// </summary>
		ScriptActionBlock Start();

		/// <summary>
		/// Stops the coroutine and resets its state.
		/// </summary>
		ScriptActionBlock Stop();

		/// <summary>
		/// Pauses the coroutine, preserving current state.
		/// </summary>
		ScriptActionBlock Pause();

		/// <summary>
		/// Resumes a paused coroutine.
		/// </summary>
		ScriptActionBlock Resume();
	}

	/// <summary>
	/// Represents a coroutine timer block. Timers fire after a duration elapses.
	/// </summary>
	public interface ITimerCoroutineBlock : ICoroutineBlock
	{
		/// <summary>
		/// Sets the time scale. Values >= 0; negative values are clamped to 0.
		/// </summary>
		ScriptActionBlock TimeScale(Double scale);
	}

	/// <summary>
	/// Represents a coroutine counter block. Counters elapse after a specific number of frames/heartbeats have passed.
	/// </summary>
	public interface ICounterCoroutineBlock : ICoroutineBlock {}

	/// <summary>
	/// Abstract base for coroutine blocks.
	/// </summary>
	public abstract class CoroutineBlock : ScriptActionBlock, ICoroutineBlock
	{
		public abstract ScriptActionBlock Start();
		public abstract ScriptActionBlock Stop();
		public abstract ScriptActionBlock Pause();
		public abstract ScriptActionBlock Resume();
	}
}
