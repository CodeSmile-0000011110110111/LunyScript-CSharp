using LunyScript.Blocks;
using LunyScript.Blocks.Coroutines;
using System;

namespace LunyScript.Coroutines.ApiBuilders
{
	/// <summary>
	/// Entry point for the Coroutine fluent builder chain.
	/// Usage: Coroutine("name").For(3).Seconds().OnFrameUpdate(blocks).Elapsed(blocks);
	/// </summary>
	public readonly struct CoroutineBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;

		internal CoroutineBuilder(ILunyScript script, String name)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrEmpty(name) ? name : throw new ArgumentException("Coroutine name is null or empty", nameof(name));
		}

		/// <summary>
		/// Sets the coroutine duration.
		/// </summary>
		public CoroutineDurationBuilder For(Double duration) => new(_script, _name, duration);

		/// <summary>
		/// Creates a coroutine without duration (runs until stopped) which runs the blocks after all scripts ran On.FrameUpdate() event, but before On.FrameLateUpdate().
		/// </summary>
		public CoroutineFinalBuilder OnFrameUpdate(params IScriptActionBlock[] blocks) =>
			CoroutineFinalBuilder.NoDuration(_script, _name, Coroutine.Process.FrameUpdate).OnFrameUpdate(blocks);

		/// <summary>
		/// Creates a coroutine without duration (runs until stopped) which runs the blocks after all scripts ran On.Hearbeat() event.
		/// </summary>
		public CoroutineFinalBuilder OnHeartbeat(params IScriptActionBlock[] blocks) =>
			CoroutineFinalBuilder.NoDuration(_script, _name, Coroutine.Process.Heartbeat).OnHeartbeat(blocks);
	}

	/// <summary>
	/// Builder step after duration amount is set. Next: specify time unit.
	/// </summary>
	public readonly struct CoroutineDurationBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;
		private readonly Double _amount;

		internal CoroutineDurationBuilder(ILunyScript script, String name, Double duration)
		{
			_script = script;
			_name = name;
			_amount = duration;
		}

		private CoroutineFinalBuilder CreateFinal(Coroutine.Options options) => CoroutineFinalBuilder.FromConfig(_script, options);

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public CoroutineFinalBuilder Seconds() => CreateFinal(Coroutine.Options.ForTimer(_name, _amount, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public CoroutineFinalBuilder Milliseconds() =>
			CreateFinal(Coroutine.Options.ForTimer(_name, _amount / 1000.0, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public CoroutineFinalBuilder Minutes() => CreateFinal(Coroutine.Options.ForTimer(_name, _amount * 60.0, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in heartbeats (count-based, counts fixed steps).
		/// </summary>
		public CoroutineFinalBuilder Heartbeats() =>
			CreateFinal(Coroutine.Options.ForCounter(_name, (Int32)_amount, Coroutine.Continuation.Finite, Coroutine.Process.Heartbeat));

		/// <summary>
		/// Duration in frames (count-based, counts frames).
		/// </summary>
		public CoroutineFinalBuilder Frames() =>
			CreateFinal(Coroutine.Options.ForCounter(_name, (Int32)_amount, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));
	}

	/// <summary>
	/// Final builder step for coroutines. Provides terminal methods to add handlers and complete.
	/// </summary>
	public readonly struct CoroutineFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly Coroutine.Options _options;

		private CoroutineFinalBuilder(ILunyScript script, in Coroutine.Options options)
		{
			_script = script;
			_options = options;
		}

		internal static CoroutineFinalBuilder FromConfig(ILunyScript script, in Coroutine.Options options) => new(script, options);

		internal static CoroutineFinalBuilder NoDuration(ILunyScript script, String name, Coroutine.Process processMode) =>
			new(script, Coroutine.Options.ForOpenEnded(name, processMode));

		/// <summary>
		/// Adds blocks to run every frame update while coroutine is running.
		/// </summary>
		public CoroutineFinalBuilder OnFrameUpdate(params IScriptActionBlock[] blocks)
		{
			if (_options.ProcessMode == Coroutine.Process.Heartbeat)
				throw new NotSupportedException($"{nameof(OnFrameUpdate)} not possible while counting Heartbeats");

			return new CoroutineFinalBuilder(_script, _options with { OnFrameUpdate = blocks });
		}

		/// <summary>
		/// Adds blocks to run every heartbeat (fixed step) while coroutine is running.
		/// </summary>
		public CoroutineFinalBuilder OnHeartbeat(params IScriptActionBlock[] blocks)
		{
			if (_options.ProcessMode == Coroutine.Process.FrameUpdate)
				throw new NotSupportedException($"{nameof(OnHeartbeat)} not possible while counting FrameUpdates");

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
			var finalBuilder = new CoroutineFinalBuilder(_script, _options with { OnElapsed = blocks });
			return finalBuilder.Build();
		}

		/// <summary>
		/// Completes the coroutine without an elapsed handler.
		/// Terminal method that returns a controllable reference.
		/// </summary>
		public IScriptCoroutineBlock Build()
		{
			var scriptInternal = (ILunyScriptInternal)_script;
			var instance = scriptInternal.RuntimeContext.Coroutines.Register(in _options);
			return CoroutineBlock.Create<IScriptCoroutineBlock>(instance);
		}
	}
}
