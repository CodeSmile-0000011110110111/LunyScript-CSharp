namespace LunyScript.Coroutines
{
	/// <summary>
	/// Represents the execution state of a coroutine or timer.
	/// </summary>
	internal enum CoroutineState
	{
		/// <summary>
		/// Coroutine has not run before.
		/// </summary>
		New,

		/// <summary>
		/// Coroutine is not running and has no accumulated time.
		/// </summary>
		Stopped,

		/// <summary>
		/// Coroutine is actively running and accumulating time.
		/// </summary>
		Running,

		/// <summary>
		/// Coroutine is frozen at current time, will resume when unpaused.
		/// </summary>
		Paused,
	}

	/// <summary>
	/// For Counter coroutines: Whether it counts frames or heartbeats.
	/// </summary>
	internal enum CoroutineCountMode
	{
		Frames,
		Heartbeats,
	}

	/// <summary>
	/// Coroutine behaviour after it ran to completion.
	/// </summary>
	internal enum CoroutineContinuationMode
	{
		Finite,
		Repeating,
	}
}
