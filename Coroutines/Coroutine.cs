using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that never elapses automatically (runs until explicitly stopped).
	/// Base for all other coroutine types that utilize full lifecycle events.
	/// </summary>
	internal class Coroutine : CoroutineBase
	{
		protected readonly IScriptSequenceBlock _onUpdateSequence;
		protected readonly IScriptSequenceBlock _onHeartbeatSequence;
		protected readonly IScriptSequenceBlock _onStartedSequence;
		protected readonly IScriptSequenceBlock _onStoppedSequence;
		protected readonly IScriptSequenceBlock _onPausedSequence;
		protected readonly IScriptSequenceBlock _onResumedSequence;

		internal override IScriptSequenceBlock OnUpdateSequence => _onUpdateSequence;
		internal override IScriptSequenceBlock OnHeartbeatSequence => _onHeartbeatSequence;
		internal override IScriptSequenceBlock OnStartedSequence => _onStartedSequence;
		internal override IScriptSequenceBlock OnStoppedSequence => _onStoppedSequence;
		internal override IScriptSequenceBlock OnPausedSequence => _onPausedSequence;
		internal override IScriptSequenceBlock OnResumedSequence => _onResumedSequence;

		public Coroutine(in CoroutineOptions options)
			: base(options)
		{
			_onUpdateSequence = CreateSequenceIfNotEmpty(options.OnUpdate);
			_onHeartbeatSequence = CreateSequenceIfNotEmpty(options.OnHeartbeat);
			_onStartedSequence = CreateSequenceIfNotEmpty(options.OnStarted);
			_onStoppedSequence = CreateSequenceIfNotEmpty(options.OnStopped);
			_onPausedSequence = CreateSequenceIfNotEmpty(options.OnPaused);
			_onResumedSequence = CreateSequenceIfNotEmpty(options.OnResumed);
		}

		public override String ToString() => $"Coroutine({Name}, {State}, Infinite)";
	}
}
