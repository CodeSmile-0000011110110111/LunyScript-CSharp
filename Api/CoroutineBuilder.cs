using LunyScript.Blocks;
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
		public CoroutineFinalBuilder OnUpdate(params IScriptActionBlock[] blocks) =>
			CoroutineFinalBuilder.NoDuration(_script, _name).OnUpdate(blocks);

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
		private readonly Int32 _amount;

		internal CoroutineDurationBuilder(ILunyScript script, String name, Int32 amount)
		{
			_script = script;
			_name = name;
			_amount = amount;
		}

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public CoroutineFinalBuilder Seconds() => CoroutineFinalBuilder.TimeBased(_script, _name, _amount);

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public CoroutineFinalBuilder Milliseconds() => CoroutineFinalBuilder.TimeBased(_script, _name, _amount / 1000.0);

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public CoroutineFinalBuilder Minutes() => CoroutineFinalBuilder.TimeBased(_script, _name, _amount * 60.0);

		/// <summary>
		/// Duration in heartbeats (count-based, counts fixed steps).
		/// </summary>
		public CoroutineFinalBuilder Heartbeats() => CoroutineFinalBuilder.HeartbeatBased(_script, _name, _amount);
	}

	/// <summary>
	/// Final builder step for coroutines. Provides terminal methods to add handlers and complete.
	/// </summary>
	public readonly struct CoroutineFinalBuilder
	{
		private readonly ILunyScript _script;
		private readonly String _name;
		private readonly Double _durationSeconds;
		private readonly Int32 _heartbeatCount;
		private readonly Boolean _isCountBased;
		private readonly IScriptActionBlock[] _onUpdateBlocks;
		private readonly IScriptActionBlock[] _onHeartbeatBlocks;
		private readonly IScriptActionBlock[] _onElapsedBlocks;
		private readonly IScriptActionBlock[] _onStartedBlocks;
		private readonly IScriptActionBlock[] _onStoppedBlocks;
		private readonly IScriptActionBlock[] _onPausedBlocks;
		private readonly IScriptActionBlock[] _onResumedBlocks;

		private CoroutineFinalBuilder(ILunyScript script, String name, Double durationSeconds, Int32 heartbeatCount,
			Boolean isCountBased, IScriptActionBlock[] onUpdateBlocks = null,
			IScriptActionBlock[] onHeartbeatBlocks = null, IScriptActionBlock[] onElapsedBlocks = null,
			IScriptActionBlock[] onStartedBlocks = null, IScriptActionBlock[] onStoppedBlocks = null,
			IScriptActionBlock[] onPausedBlocks = null, IScriptActionBlock[] onResumedBlocks = null)
		{
			_script = script;
			_name = name;
			_durationSeconds = durationSeconds;
			_heartbeatCount = heartbeatCount;
			_isCountBased = isCountBased;
			_onUpdateBlocks = onUpdateBlocks;
			_onHeartbeatBlocks = onHeartbeatBlocks;
			_onElapsedBlocks = onElapsedBlocks;
			_onStartedBlocks = onStartedBlocks;
			_onStoppedBlocks = onStoppedBlocks;
			_onPausedBlocks = onPausedBlocks;
			_onResumedBlocks = onResumedBlocks;
		}

		internal static CoroutineFinalBuilder TimeBased(ILunyScript script, String name, Double durationSeconds) =>
			new(script, name, durationSeconds, 0, isCountBased: false);

		internal static CoroutineFinalBuilder HeartbeatBased(ILunyScript script, String name, Int32 heartbeatCount) =>
			new(script, name, 0, heartbeatCount, isCountBased: true);

		internal static CoroutineFinalBuilder NoDuration(ILunyScript script, String name) =>
			new(script, name, 0, 0, isCountBased: false);

		/// <summary>
		/// Adds blocks to run every frame update while coroutine is running.
		/// </summary>
		public CoroutineFinalBuilder OnUpdate(params IScriptActionBlock[] blocks) =>
			new(_script, _name, _durationSeconds, _heartbeatCount, _isCountBased, blocks, _onHeartbeatBlocks, _onElapsedBlocks,
				_onStartedBlocks, _onStoppedBlocks, _onPausedBlocks, _onResumedBlocks);

		/// <summary>
		/// Adds blocks to run every heartbeat (fixed step) while coroutine is running.
		/// </summary>
		public CoroutineFinalBuilder OnHeartbeat(params IScriptActionBlock[] blocks) =>
			new(_script, _name, _durationSeconds, _heartbeatCount, _isCountBased, _onUpdateBlocks, blocks, _onElapsedBlocks,
				_onStartedBlocks, _onStoppedBlocks, _onPausedBlocks, _onResumedBlocks);

		/// <summary>
		/// Adds blocks to run when the coroutine is started (not when restarted).
		/// </summary>
		public CoroutineFinalBuilder Started(params IScriptActionBlock[] blocks) =>
			new(_script, _name, _durationSeconds, _heartbeatCount, _isCountBased, _onUpdateBlocks, _onHeartbeatBlocks, _onElapsedBlocks,
				blocks, _onStoppedBlocks, _onPausedBlocks, _onResumedBlocks);

		/// <summary>
		/// Adds blocks to run when the coroutine is stopped.
		/// </summary>
		public CoroutineFinalBuilder Stopped(params IScriptActionBlock[] blocks) =>
			new(_script, _name, _durationSeconds, _heartbeatCount, _isCountBased, _onUpdateBlocks, _onHeartbeatBlocks, _onElapsedBlocks,
				_onStartedBlocks, blocks, _onPausedBlocks, _onResumedBlocks);

		/// <summary>
		/// Adds blocks to run when the coroutine is paused.
		/// </summary>
		public CoroutineFinalBuilder Paused(params IScriptActionBlock[] blocks) =>
			new(_script, _name, _durationSeconds, _heartbeatCount, _isCountBased, _onUpdateBlocks, _onHeartbeatBlocks, _onElapsedBlocks,
				_onStartedBlocks, _onStoppedBlocks, blocks, _onResumedBlocks);

		/// <summary>
		/// Adds blocks to run when the coroutine is resumed.
		/// </summary>
		public CoroutineFinalBuilder Resumed(params IScriptActionBlock[] blocks) =>
			new(_script, _name, _durationSeconds, _heartbeatCount, _isCountBased, _onUpdateBlocks, _onHeartbeatBlocks, _onElapsedBlocks,
				_onStartedBlocks, _onStoppedBlocks, _onPausedBlocks, blocks);

		/// <summary>
		/// Adds blocks to run when the coroutine duration elapses.
		/// Terminal method that completes the coroutine and returns a controllable reference.
		/// </summary>
		public IScriptCoroutineBlock Elapsed(params IScriptActionBlock[] blocks)
		{
			var finalBuilder = new CoroutineFinalBuilder(_script, _name, _durationSeconds, _heartbeatCount, _isCountBased,
				_onUpdateBlocks, _onHeartbeatBlocks, blocks, _onStartedBlocks, _onStoppedBlocks, _onPausedBlocks, _onResumedBlocks);
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

			var instance = context.Coroutines.Register(_name);

			// Set duration based on type
			if (_isCountBased)
				instance.SetHeartbeatCount(_heartbeatCount);
			else if (_durationSeconds > 0)
				instance.SetDuration(_durationSeconds);

			if (_onUpdateBlocks != null && _onUpdateBlocks.Length > 0)
				instance.SetOnUpdateBlocks(_onUpdateBlocks);

			if (_onHeartbeatBlocks != null && _onHeartbeatBlocks.Length > 0)
				instance.SetOnHeartbeatBlocks(_onHeartbeatBlocks);

			if (_onElapsedBlocks != null && _onElapsedBlocks.Length > 0)
				instance.SetOnElapsedBlocks(_onElapsedBlocks);

			if (_onStartedBlocks != null && _onStartedBlocks.Length > 0)
				instance.SetOnStartedBlocks(_onStartedBlocks);

			if (_onStoppedBlocks != null && _onStoppedBlocks.Length > 0)
				instance.SetOnStoppedBlocks(_onStoppedBlocks);

			if (_onPausedBlocks != null && _onPausedBlocks.Length > 0)
				instance.SetOnPausedBlocks(_onPausedBlocks);

			if (_onResumedBlocks != null && _onResumedBlocks.Length > 0)
				instance.SetOnResumedBlocks(_onResumedBlocks);

			return new CoroutineBlock(instance);
		}
	}
}
