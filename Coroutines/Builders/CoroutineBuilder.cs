using LunyScript.Blocks;
using LunyScript.Blocks.Coroutines;
using System;

namespace LunyScript.Coroutines.Builders
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
			_name = !String.IsNullOrEmpty(name) ? name : throw new ArgumentException("Coroutine name is null or empty", nameof(name));
		}

		/// <summary>
		/// Sets the coroutine duration.
		/// </summary>
		public CoroutineDurationBuilder For(Double duration) => new(_script, _name, duration);

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
		private readonly ILunyScript _script;
		private readonly String _name;
		private readonly Double _amount;

		internal CoroutineDurationBuilder(ILunyScript script, String name, Double duration)
		{
			_script = script;
			_name = name;
			_amount = duration;
		}

		private CoroutineFinalBuilder CreateFinal(CoroutineConfig config) => CoroutineFinalBuilder.FromConfig(_script, config);

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public CoroutineFinalBuilder Seconds() => CreateFinal(CoroutineConfig.ForTimer(_name, _amount, CoroutineContinuationMode.Finite));

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public CoroutineFinalBuilder Milliseconds() => CreateFinal(CoroutineConfig.ForTimer(_name, UnitConverter.ToMilliseconds(_amount), CoroutineContinuationMode.Finite));

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public CoroutineFinalBuilder Minutes() => CreateFinal(CoroutineConfig.ForTimer(_name, UnitConverter.ToMinutes(_amount), CoroutineContinuationMode.Finite));

		/// <summary>
		/// Duration in heartbeats (count-based, counts fixed steps).
		/// </summary>
		public CoroutineFinalBuilder Heartbeats() => CreateFinal(CoroutineConfig.ForCounter(_name, (Int32)_amount, CoroutineContinuationMode.Finite));

		/// <summary>
		/// Duration in frames (count-based, counts frames).
		/// </summary>
		public CoroutineFinalBuilder Frames() => CreateFinal(CoroutineConfig.ForCounter(_name, (Int32)_amount, CoroutineContinuationMode.Finite));
	}

	/// <summary>
	/// Final builder step for coroutines. Provides terminal methods to add handlers and complete.
	/// </summary>
	public readonly struct CoroutineFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly CoroutineConfig _config;

		private CoroutineFinalBuilder(ILunyScript script, in CoroutineConfig config)
		{
			_script = script;
			_config = config;
		}

		internal static CoroutineFinalBuilder FromConfig(ILunyScript script, in CoroutineConfig config) => new(script, config);

		internal static CoroutineFinalBuilder NoDuration(ILunyScript script, String name) => new(script, CoroutineConfig.ForOpenEnded(name));

		/// <summary>
		/// Adds blocks to run every frame update while coroutine is running.
		/// </summary>
		public CoroutineFinalBuilder OnFrameUpdate(params IScriptActionBlock[] blocks) => new(_script, _config with { OnFrameUpdate = blocks });

		/// <summary>
		/// Adds blocks to run every heartbeat (fixed step) while coroutine is running.
		/// </summary>
		public CoroutineFinalBuilder OnHeartbeat(params IScriptActionBlock[] blocks) => new(_script, _config with { OnHeartbeat = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is started (not when restarted).
		/// </summary>
		public CoroutineFinalBuilder Started(params IScriptActionBlock[] blocks) => new(_script, _config with { OnStarted = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is stopped.
		/// </summary>
		public CoroutineFinalBuilder Stopped(params IScriptActionBlock[] blocks) => new(_script, _config with { OnStopped = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is paused.
		/// </summary>
		public CoroutineFinalBuilder Paused(params IScriptActionBlock[] blocks) => new(_script, _config with { OnPaused = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine is resumed.
		/// </summary>
		public CoroutineFinalBuilder Resumed(params IScriptActionBlock[] blocks) => new(_script, _config with { OnResumed = blocks });

		/// <summary>
		/// Adds blocks to run when the coroutine duration elapses.
		/// Terminal method that completes the coroutine and returns a controllable reference.
		/// </summary>
		public IScriptCoroutineBlock Elapsed(params IScriptActionBlock[] blocks)
		{
			var finalBuilder = new CoroutineFinalBuilder(_script, _config with { OnElapsed = blocks });
			return finalBuilder.Build();
		}

		/// <summary>
		/// Completes the coroutine without an elapsed handler.
		/// Terminal method that returns a controllable reference.
		/// </summary>
		public IScriptCoroutineBlock Build()
		{
			var scriptInternal = (ILunyScriptInternal)_script;
			var instance = scriptInternal.Context.Coroutines.Register(in _config);
			return CoroutineBlock.Create<IScriptCoroutineBlock>(instance);
		}
	}
}
