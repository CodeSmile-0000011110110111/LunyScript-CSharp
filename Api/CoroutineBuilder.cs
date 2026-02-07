using LunyScript.Blocks;
using LunyScript.Blocks.Coroutines;
using LunyScript.Coroutines;
using LunyScript.Execution;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Entry point for the Coroutine fluent builder chain.
	/// Usage: Coroutine("name").Duration(3).Seconds().OnUpdate(blocks);
	/// </summary>
	public readonly struct CoroutineBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;

		internal CoroutineBuilder(ILunyScript script, String name)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrEmpty(name) ? name : throw new ArgumentException("Coroutine name cannot be null or empty", nameof(name));
		}

		/// <summary>
		/// Sets the coroutine duration.
		/// </summary>
		public CoroutineDurationBuilder Duration(Int32 amount) => new(_script, _name, amount);

		/// <summary>
		/// Creates a coroutine without duration (runs until stopped).
		/// Terminal method that registers the coroutine.
		/// </summary>
		public CoroutineFinalBuilder OnFrameUpdate(params IScriptActionBlock[] blocks) =>
			CoroutineFinalBuilder.NoDuration(_script, _name).OnFrameUpdate(blocks);

		/// <summary>
		/// Creates a coroutine without duration (runs until stopped).
		/// Terminal method that registers the coroutine.
		/// </summary>
		public CoroutineFinalBuilder OnHeartbeat(params IScriptActionBlock[] blocks) =>
			CoroutineFinalBuilder.NoDuration(_script, _name).OnHeartbeat(blocks);
	}

	/// <summary>
	/// Builder step after duration amount is set. Next: specify time unit.
	/// </summary>
	public readonly struct CoroutineDurationBuilder
	{
		private readonly TimeUnitBuilder<CoroutineFinalBuilder> _builder;

		internal CoroutineDurationBuilder(ILunyScript script, String name, Int32 amount)
		{
			_builder = new TimeUnitBuilder<CoroutineFinalBuilder>(script, name, amount, false, false,
				options => CoroutineFinalBuilder.FromOptions(script, options));
		}

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public CoroutineFinalBuilder Seconds() => _builder.Seconds();

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public CoroutineFinalBuilder Milliseconds() => _builder.Milliseconds();

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public CoroutineFinalBuilder Minutes() => _builder.Minutes();

		/// <summary>
		/// Duration in heartbeats (count-based, counts fixed steps).
		/// </summary>
		public CoroutineFinalBuilder Heartbeats() => _builder.Heartbeats();
	}

	/// <summary>
	/// Final builder step for coroutines. Provides terminal methods to add handlers and complete.
	/// </summary>
	public readonly struct CoroutineFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly CoroutineOptions _options;

		private CoroutineFinalBuilder(ILunyScript script, in CoroutineOptions options)
		{
			_script = script;
			_options = options;
		}

		internal static CoroutineFinalBuilder FromOptions(ILunyScript script, in CoroutineOptions options) => new(script, options);

		internal static CoroutineFinalBuilder TimeBased(ILunyScript script, String name, Double durationSeconds) =>
			new(script, CoroutineOptions.ForDuration(name, durationSeconds, false, false, false));

		internal static CoroutineFinalBuilder HeartbeatBased(ILunyScript script, String name, Int32 heartbeatCount) =>
			new(script, CoroutineOptions.ForDuration(name, heartbeatCount, false, true, false));

		internal static CoroutineFinalBuilder NoDuration(ILunyScript script, String name) =>
			new(script, CoroutineOptions.ForCoroutine(name));

		/// <summary>
		/// Adds blocks to run every frame update while coroutine is running.
		/// </summary>
		public CoroutineFinalBuilder OnFrameUpdate(params IScriptActionBlock[] blocks) =>
			new(_script, _options with { OnUpdate = blocks });

		/// <summary>
		/// Adds blocks to run every heartbeat (fixed step) while coroutine is running.
		/// </summary>
		public CoroutineFinalBuilder OnHeartbeat(params IScriptActionBlock[] blocks) =>
			new(_script, _options with { OnHeartbeat = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is started (not when restarted).
		/// </summary>
		public CoroutineFinalBuilder Started(params IScriptActionBlock[] blocks) =>
			new(_script, _options with { OnStarted = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is stopped.
		/// </summary>
		public CoroutineFinalBuilder Stopped(params IScriptActionBlock[] blocks) =>
			new(_script, _options with { OnStopped = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is paused.
		/// </summary>
		public CoroutineFinalBuilder Paused(params IScriptActionBlock[] blocks) =>
			new(_script, _options with { OnPaused = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is resumed.
		/// </summary>
		public CoroutineFinalBuilder Resumed(params IScriptActionBlock[] blocks) =>
			new(_script, _options with { OnResumed = blocks });

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
			var context = scriptInternal.Context;

			if (context == null)
				return null;

			var instance = context.Coroutines.Register(in _options);
			return new CoroutineBlock(instance);
		}
	}
}
