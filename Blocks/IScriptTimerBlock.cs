using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Represents a timer block that can be controlled (started, stopped, paused, resumed).
	/// Timers fire after a duration elapses.
	/// </summary>
	public interface IScriptTimerBlock : IScriptActionBlock
	{
		/// <summary>
		/// Starts or restarts the timer.
		/// </summary>
		IScriptActionBlock Start();

		/// <summary>
		/// Stops the timer and resets its state.
		/// </summary>
		IScriptActionBlock Stop();

		/// <summary>
		/// Pauses the timer, preserving current elapsed time.
		/// </summary>
		IScriptActionBlock Pause();

		/// <summary>
		/// Resumes a paused timer.
		/// </summary>
		IScriptActionBlock Resume();

		/// <summary>
		/// Sets the time scale. Values >= 0; negative values are clamped to 0.
		/// </summary>
		void TimeScale(Double scale);
	}
}
