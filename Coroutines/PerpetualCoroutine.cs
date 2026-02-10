using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that never elapses automatically (runs until explicitly stopped).
	/// Base for all other coroutine types that utilize full lifecycle events.
	/// </summary>
	internal class PerpetualCoroutine : Coroutine
	{
		internal override IScriptSequenceBlock OnFrameUpdateSequence { get; }
		internal override IScriptSequenceBlock OnHeartbeatSequence { get; }
		internal override IScriptSequenceBlock OnStartedSequence { get; }
		internal override IScriptSequenceBlock OnStoppedSequence { get; }
		internal override IScriptSequenceBlock OnPausedSequence { get; }
		internal override IScriptSequenceBlock OnResumedSequence { get; }
		internal override IScriptSequenceBlock OnElapsedSequence { get; }

		public PerpetualCoroutine(in Options options)
			: base(options)
		{
			OnFrameUpdateSequence = SequenceBlock.TryCreate(options.OnFrameUpdate);
			OnHeartbeatSequence = SequenceBlock.TryCreate(options.OnHeartbeat);
			OnStartedSequence = SequenceBlock.TryCreate(options.OnStarted);
			OnStoppedSequence = SequenceBlock.TryCreate(options.OnStopped);
			OnPausedSequence = SequenceBlock.TryCreate(options.OnPaused);
			OnResumedSequence = SequenceBlock.TryCreate(options.OnResumed);
			OnElapsedSequence = SequenceBlock.TryCreate(options.OnElapsed);
		}

		protected override void OnStart() {}
		protected override void OnStop() {}
		protected override Boolean OnFrameUpdate() => false;
		protected override Boolean OnHeartbeat() => false;
		public override String ToString() => $"{GetType().Name}({Name}, {State})";
	}
}
