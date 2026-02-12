using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines.ApiBuilders
{
	/// <summary>
	/// Entry point for the Coroutine fluent builder chain.
	/// Usage: Coroutine("name").For(3).Seconds().OnFrameUpdate(blocks).Elapsed(blocks);
	/// </summary>
	public readonly struct CoroutineBuilder
	{
		private readonly IScript _script;
		private readonly String _name;

		internal CoroutineBuilder(IScript script, String name)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Coroutine name is null or empty", nameof(name));
		}

		/// <summary>
		/// Sets the coroutine duration.
		/// </summary>
		public CoroutineDurationBuilder For(Double duration) => new(_script, _name, duration);

		/// <summary>
		/// Creates a coroutine without duration (runs until stopped) which runs the blocks after all scripts ran On.FrameUpdate() event, but before On.FrameLateUpdate().
		/// </summary>
		public CoroutineFinalBuilder OnFrameUpdate(params IScriptActionBlock[] blocks) =>
			new CoroutineFinalBuilder(_script, Coroutine.Options.ForOpenEnded(_name, Coroutine.Process.FrameUpdate)).OnFrameUpdate(blocks);

		/// <summary>
		/// Creates a coroutine without duration (runs until stopped) which runs the blocks after all scripts ran On.Hearbeat() event.
		/// </summary>
		public CoroutineFinalBuilder OnHeartbeat(params IScriptActionBlock[] blocks) =>
			new CoroutineFinalBuilder(_script, Coroutine.Options.ForOpenEnded(_name, Coroutine.Process.Heartbeat)).OnHeartbeat(blocks);
	}

	/// <summary>
	/// Builder step after duration amount is set. Next: specify time unit.
	/// </summary>
	public readonly struct CoroutineDurationBuilder
	{
		private readonly IScript _script;
		private readonly String _name;
		private readonly Double _duration;

		internal CoroutineDurationBuilder(IScript script, String name, Double duration)
		{
			_script = script;
			_name = name;
			_duration = Math.Max(0, duration);

			if (duration < 0)
				throw new ArgumentException($"Coroutine duration must be 0 or greater, got: {duration}");
		}

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public CoroutineFinalBuilder Seconds() => new(_script,
			Coroutine.Options.ForTimer(_name, _duration, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public CoroutineFinalBuilder Milliseconds() => new(_script, Coroutine.Options.ForTimer(_name, _duration / 1000.0,
			Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public CoroutineFinalBuilder Minutes() => new(_script,
			Coroutine.Options.ForTimer(_name, _duration * 60.0, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in heartbeats (count-based, counts fixed steps).
		/// </summary>
		public CoroutineFinalBuilder Heartbeats() => new(_script,
			Coroutine.Options.ForCounter(_name, (Int32)_duration, Coroutine.Continuation.Finite, Coroutine.Process.Heartbeat));

		/// <summary>
		/// Duration in frames (count-based, counts frames).
		/// </summary>
		public CoroutineFinalBuilder Frames() => new(_script,
			Coroutine.Options.ForCounter(_name, (Int32)_duration, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));
	}

	/// <summary>
	/// Final builder step for coroutines. Provides terminal methods to add handlers and complete.
	/// </summary>
	public readonly struct CoroutineFinalBuilder
	{
		private readonly IScript _script;
		private readonly Coroutine.Options _options;

		internal CoroutineFinalBuilder(IScript script, in Coroutine.Options options)
		{
			_script = script;
			_options = options;
		}

		/// <summary>
		/// Adds blocks to run every frame update while coroutine is running.
		/// </summary>
		public CoroutineFinalBuilder OnFrameUpdate(params IScriptActionBlock[] blocks)
		{
			if (_options.ProcessMode == Coroutine.Process.Heartbeat)
				throw new NotSupportedException($"{nameof(OnFrameUpdate)} cannot be combined with {nameof(OnHeartbeat)}");

			return new CoroutineFinalBuilder(_script, _options with { OnFrameUpdate = blocks });
		}

		/// <summary>
		/// Adds blocks to run every heartbeat (fixed step) while coroutine is running.
		/// </summary>
		public CoroutineFinalBuilder OnHeartbeat(params IScriptActionBlock[] blocks)
		{
			if (_options.ProcessMode == Coroutine.Process.FrameUpdate)
				throw new NotSupportedException($"{nameof(OnHeartbeat)} cannot be combined with {nameof(OnFrameUpdate)}");

			return new CoroutineFinalBuilder(_script, _options with { OnHeartbeat = blocks });
		}

		/// <summary>
		/// Adds blocks to run when the coroutine is started (not when restarted).
		/// </summary>
		public CoroutineFinalBuilder Started(params IScriptActionBlock[] blocks) => new(_script, _options with { OnStarted = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is stopped.
		/// </summary>
		public CoroutineFinalBuilder Stopped(params IScriptActionBlock[] blocks) => new(_script, _options with { OnStopped = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is paused.
		/// </summary>
		public CoroutineFinalBuilder Paused(params IScriptActionBlock[] blocks) => new(_script, _options with { OnPaused = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is resumed.
		/// </summary>
		public CoroutineFinalBuilder Resumed(params IScriptActionBlock[] blocks) => new(_script, _options with { OnResumed = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine duration elapses.
		/// Terminal method that completes the coroutine and returns a controllable reference.
		/// </summary>
		public IScriptCoroutineBlock Elapsed(params IScriptActionBlock[] blocks)
		{
			var options = _options with { OnElapsed = blocks };
			var scriptInternal = (ILunyScriptInternal)_script;
			var coroutineBlock = scriptInternal.RuntimeContext.Coroutines.Register(_script, in options);
			return coroutineBlock;
		}

		/// <summary>
		/// Completes the coroutine without an elapsed handler.
		/// Terminal method that returns a controllable reference.
		/// </summary>
		public IScriptCoroutineBlock Build()
		{
			var scriptInternal = (ILunyScriptInternal)_script;
			var coroutineBlock = scriptInternal.RuntimeContext.Coroutines.Register(_script, in _options);
			return coroutineBlock;
		}
	}
}
