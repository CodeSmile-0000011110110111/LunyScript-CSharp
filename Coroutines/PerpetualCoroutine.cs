using Luny.Engine.Services;
using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that never elapses automatically (runs until explicitly stopped).
	/// Base for all other coroutine types that utilize full lifecycle events.
	/// </summary>
	internal class PerpetualCoroutine : CoroutineBase
	{
		internal override IScriptSequenceBlock OnFrameUpdateSequence { get; }
		internal override IScriptSequenceBlock OnHeartbeatSequence { get; }
		internal override IScriptSequenceBlock OnStartedSequence { get; }
		internal override IScriptSequenceBlock OnStoppedSequence { get; }
		internal override IScriptSequenceBlock OnPausedSequence { get; }
		internal override IScriptSequenceBlock OnResumedSequence { get; }
		internal override IScriptSequenceBlock OnElapsedSequence { get; }

		public PerpetualCoroutine(in CoroutineConfig config)
			: base(config)
		{
			OnFrameUpdateSequence = SequenceBlock.TryCreate(config.OnFrameUpdate);
			OnHeartbeatSequence = SequenceBlock.TryCreate(config.OnHeartbeat);
			OnStartedSequence = SequenceBlock.TryCreate(config.OnStarted);
			OnStoppedSequence = SequenceBlock.TryCreate(config.OnStopped);
			OnPausedSequence = SequenceBlock.TryCreate(config.OnPaused);
			OnResumedSequence = SequenceBlock.TryCreate(config.OnResumed);
			OnElapsedSequence = SequenceBlock.TryCreate(config.OnElapsed);
		}

		protected override void ResetState() {}
		protected override Boolean OnFrameUpdate() => false;
		protected override Boolean OnHeartbeat() => false;
		public override String ToString() => $"{GetType().Name}({Name}, {State})";
	}
}
