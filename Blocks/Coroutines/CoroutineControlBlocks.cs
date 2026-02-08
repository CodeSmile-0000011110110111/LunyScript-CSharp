using LunyScript.Coroutines;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks.Coroutines
{
	internal abstract class CoroutineControlBlockBase : IScriptActionBlock
	{
		protected readonly CoroutineBase _coroutine;

		protected CoroutineControlBlockBase(CoroutineBase coroutine) =>
			_coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

		public abstract void Execute(ILunyScriptContext context);
	}

	internal sealed class CoroutineStartBlock : CoroutineControlBlockBase
	{
		public CoroutineStartBlock(CoroutineBase coroutine)
			: base(coroutine) {}

		public override void Execute(ILunyScriptContext context)
		{
			if (_coroutine.Start())
				_coroutine.OnStartedSequence?.Execute(context);
		}
	}

	internal sealed class CoroutineStopBlock : CoroutineControlBlockBase
	{
		public CoroutineStopBlock(CoroutineBase coroutine)
			: base(coroutine) {}

		public override void Execute(ILunyScriptContext context)
		{
			if (_coroutine.Stop())
				_coroutine.OnStoppedSequence?.Execute(context);
		}
	}

	internal sealed class CoroutinePauseBlock : CoroutineControlBlockBase
	{
		public CoroutinePauseBlock(CoroutineBase coroutine)
			: base(coroutine) {}

		public override void Execute(ILunyScriptContext context)
		{
			if (_coroutine.Pause())
				_coroutine.OnPausedSequence?.Execute(context);
		}
	}

	internal sealed class CoroutineResumeBlock : CoroutineControlBlockBase
	{
		public CoroutineResumeBlock(CoroutineBase coroutine)
			: base(coroutine) {}

		public override void Execute(ILunyScriptContext context)
		{
			if (_coroutine.Resume())
				_coroutine.OnResumedSequence?.Execute(context);
		}
	}

	internal sealed class CoroutineSetTimeScaleBlock : CoroutineControlBlockBase
	{
		private readonly Double _timeScale;

		public CoroutineSetTimeScaleBlock(CoroutineBase coroutine, Double timeScale)
			: base(coroutine) => _timeScale = timeScale;

		public override void Execute(ILunyScriptContext context) => _coroutine.TimeScale = _timeScale;
	}
}
