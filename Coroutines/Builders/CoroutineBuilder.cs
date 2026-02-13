using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines.Builders
{
	/// <summary>
	/// Entry point for the Coroutine fluent builder chain.
	/// Usage: Coroutine("name").For(3).Seconds().OnFrameUpdate(blocks).WhenElapsed(blocks);
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
		/// Sets the coroutine duration. Returns a builder to specify the time unit.
		/// </summary>
		public CoroutineDurationBuilder For(Double duration) => new(_script, _name, duration);

		/// <summary>
		/// Creates an open-ended coroutine (runs until stopped) which runs the blocks every frame.
		/// </summary>
		public OpenEndedFrameCoroutineBuilder OnFrameUpdate(params IScriptActionBlock[] blocks) => new(_script,
			Coroutine.Options.ForOpenEnded(_name, Coroutine.Process.FrameUpdate) with { OnFrameUpdate = blocks });

		/// <summary>
		/// Creates an open-ended coroutine (runs until stopped) which runs the blocks every heartbeat (fixed step).
		/// </summary>
		public OpenEndedHeartbeatCoroutineBuilder OnHeartbeat(params IScriptActionBlock[] blocks) => new(_script,
			Coroutine.Options.ForOpenEnded(_name, Coroutine.Process.Heartbeat) with { OnHeartbeat = blocks });
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
		public FiniteFrameCoroutineBuilder Seconds() => new(_script,
			Coroutine.Options.ForTimer(_name, _duration, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public FiniteFrameCoroutineBuilder Milliseconds() => new(_script,
			Coroutine.Options.ForTimer(_name, _duration / 1000.0, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public FiniteFrameCoroutineBuilder Minutes() => new(_script,
			Coroutine.Options.ForTimer(_name, _duration * 60.0, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in heartbeats (count-based, counts fixed steps).
		/// </summary>
		public FiniteHeartbeatCoroutineBuilder Heartbeats() => new(_script,
			Coroutine.Options.ForCounter(_name, (Int32)_duration, Coroutine.Continuation.Finite, Coroutine.Process.Heartbeat));

		/// <summary>
		/// Duration in frames (count-based, counts frames).
		/// </summary>
		public FiniteFrameCoroutineBuilder Frames() => new(_script,
			Coroutine.Options.ForCounter(_name, (Int32)_duration, Coroutine.Continuation.Finite, Coroutine.Process.FrameUpdate));
	}

	/// <summary>
	/// Builder for finite coroutines running on frame updates.
	/// </summary>
	public readonly struct FiniteFrameCoroutineBuilder
	{
		private readonly IScript _script;
		private readonly Coroutine.Options _options;

		internal FiniteFrameCoroutineBuilder(IScript script, in Coroutine.Options options)
		{
			_script = script;
			_options = options;
		}

		public FiniteFrameCoroutineBuilder OnFrameUpdate(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnFrameUpdate = BuilderUtility.Append(_options.OnFrameUpdate, blocks) });

		public FiniteFrameCoroutineBuilder WhenStarted(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public FiniteFrameCoroutineBuilder WhenStopped(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public FiniteFrameCoroutineBuilder WhenPaused(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public FiniteFrameCoroutineBuilder WhenResumed(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public IScriptCoroutineBlock WhenElapsed(params IScriptActionBlock[] blocks) => BuilderUtility.Finalize(_script,
			_options with { OnElapsed = BuilderUtility.Append(_options.OnElapsed, blocks) });

		public IScriptCoroutineBlock Do() => BuilderUtility.Finalize(_script, _options);

		public IScriptCoroutineBlock Do(params IScriptActionBlock[] blocks) => BuilderUtility.Finalize(_script,
			_options with { OnFrameUpdate = BuilderUtility.Append(_options.OnFrameUpdate, blocks) });
	}

	/// <summary>
	/// Builder for finite coroutines running on heartbeats.
	/// </summary>
	public readonly struct FiniteHeartbeatCoroutineBuilder
	{
		private readonly IScript _script;
		private readonly Coroutine.Options _options;

		internal FiniteHeartbeatCoroutineBuilder(IScript script, in Coroutine.Options options)
		{
			_script = script;
			_options = options;
		}

		public FiniteHeartbeatCoroutineBuilder OnHeartbeat(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnHeartbeat = BuilderUtility.Append(_options.OnHeartbeat, blocks) });

		public FiniteHeartbeatCoroutineBuilder WhenStarted(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public FiniteHeartbeatCoroutineBuilder WhenStopped(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public FiniteHeartbeatCoroutineBuilder WhenPaused(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public FiniteHeartbeatCoroutineBuilder WhenResumed(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public IScriptCoroutineBlock WhenElapsed(params IScriptActionBlock[] blocks) => BuilderUtility.Finalize(_script,
			_options with { OnElapsed = BuilderUtility.Append(_options.OnElapsed, blocks) });

		public IScriptCoroutineBlock Do() => BuilderUtility.Finalize(_script, _options);

		public IScriptCoroutineBlock Do(params IScriptActionBlock[] blocks) => BuilderUtility.Finalize(_script,
			_options with { OnHeartbeat = BuilderUtility.Append(_options.OnHeartbeat, blocks) });
	}

	/// <summary>
	/// Builder for open-ended coroutines running on frame updates.
	/// </summary>
	public readonly struct OpenEndedFrameCoroutineBuilder
	{
		private readonly IScript _script;
		private readonly Coroutine.Options _options;

		internal OpenEndedFrameCoroutineBuilder(IScript script, in Coroutine.Options options)
		{
			_script = script;
			_options = options;
		}

		public OpenEndedFrameCoroutineBuilder WhenStarted(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public OpenEndedFrameCoroutineBuilder WhenStopped(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public OpenEndedFrameCoroutineBuilder WhenPaused(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public OpenEndedFrameCoroutineBuilder WhenResumed(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public IScriptCoroutineBlock Do() => BuilderUtility.Finalize(_script, _options);

		public IScriptCoroutineBlock Do(params IScriptActionBlock[] blocks) => BuilderUtility.Finalize(_script,
			_options with { OnFrameUpdate = BuilderUtility.Append(_options.OnFrameUpdate, blocks) });
	}

	/// <summary>
	/// Builder for open-ended coroutines running on heartbeats.
	/// </summary>
	public readonly struct OpenEndedHeartbeatCoroutineBuilder
	{
		private readonly IScript _script;
		private readonly Coroutine.Options _options;

		internal OpenEndedHeartbeatCoroutineBuilder(IScript script, in Coroutine.Options options)
		{
			_script = script;
			_options = options;
		}

		public OpenEndedHeartbeatCoroutineBuilder WhenStarted(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public OpenEndedHeartbeatCoroutineBuilder WhenStopped(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public OpenEndedHeartbeatCoroutineBuilder WhenPaused(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public OpenEndedHeartbeatCoroutineBuilder WhenResumed(params IScriptActionBlock[] blocks) => new(_script,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public IScriptCoroutineBlock Do() => BuilderUtility.Finalize(_script, _options);

		public IScriptCoroutineBlock Do(params IScriptActionBlock[] blocks) => BuilderUtility.Finalize(_script,
			_options with { OnHeartbeat = BuilderUtility.Append(_options.OnHeartbeat, blocks) });
	}
}
