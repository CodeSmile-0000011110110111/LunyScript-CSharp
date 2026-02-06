using LunyScript.Blocks;
using LunyScript.Execution;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Wraps a CoroutineInstance as a block for use in script sequences.
	/// Provides control methods (Start, Stop, Pause, Resume) as action blocks.
	/// </summary>
	internal sealed class CoroutineBlock : IScriptCoroutineBlock
	{
		private readonly CoroutineInstance _instance;

		internal CoroutineBlock(CoroutineInstance instance) =>
			_instance = instance ?? throw new ArgumentNullException(nameof(instance));

		public void Execute(ILunyScriptContext context)
		{
			// Default execution: start the coroutine
			if (_instance.Start())
				_instance.OnStartedSequence?.Execute(context);
		}

		public IScriptActionBlock Start() => new CoroutineControlBlock(_instance, CoroutineControlAction.Start);
		public IScriptActionBlock Stop() => new CoroutineControlBlock(_instance, CoroutineControlAction.Stop);
		public IScriptActionBlock Pause() => new CoroutineControlBlock(_instance, CoroutineControlAction.Pause);
		public IScriptActionBlock Resume() => new CoroutineControlBlock(_instance, CoroutineControlAction.Resume);
		public void TimeScale(Double scale) => _instance.SetTimeScale(scale);
	}

	internal enum CoroutineControlAction
	{
		Start,
		Stop,
		Pause,
		Resume
	}

	/// <summary>
	/// Action block that performs a control operation on a coroutine.
	/// Executes corresponding control event sequences when the action succeeds.
	/// </summary>
	internal sealed class CoroutineControlBlock : IScriptActionBlock
	{
		private readonly CoroutineInstance _instance;
		private readonly CoroutineControlAction _action;

		internal CoroutineControlBlock(CoroutineInstance instance, CoroutineControlAction action)
		{
			_instance = instance;
			_action = action;
		}

		public void Execute(ILunyScriptContext context)
		{
			IScriptSequenceBlock eventSequence = null;

			switch (_action)
			{
				case CoroutineControlAction.Start:
					if (_instance.Start())
						eventSequence = _instance.OnStartedSequence;
					break;
				case CoroutineControlAction.Stop:
					if (_instance.Stop())
						eventSequence = _instance.OnStoppedSequence;
					break;
				case CoroutineControlAction.Pause:
					if (_instance.Pause())
						eventSequence = _instance.OnPausedSequence;
					break;
				case CoroutineControlAction.Resume:
					if (_instance.Resume())
						eventSequence = _instance.OnResumedSequence;
					break;
			}

			// Execute the control event sequence if one is registered
			eventSequence?.Execute(context);
		}
	}
}
