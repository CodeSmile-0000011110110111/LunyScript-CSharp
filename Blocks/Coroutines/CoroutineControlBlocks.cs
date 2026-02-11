using LunyScript.Coroutines;
using System;

namespace LunyScript.Blocks.Coroutines
{
	internal abstract class CoroutineControlBlockBase : IScriptActionBlock
	{
		protected readonly Coroutine _coroutine;

		protected CoroutineControlBlockBase(Coroutine coroutine) =>
			_coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

		public abstract void Execute(IScriptRuntimeContext runtimeContext);
	}

	internal sealed class CoroutineStartBlock : CoroutineControlBlockBase
	{
		public CoroutineStartBlock(Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Start();
	}

	internal sealed class CoroutineStopBlock : CoroutineControlBlockBase
	{
		public CoroutineStopBlock(Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Stop();
	}

	internal sealed class CoroutinePauseBlock : CoroutineControlBlockBase
	{
		public CoroutinePauseBlock(Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Pause();
	}

	internal sealed class CoroutineResumeBlock : CoroutineControlBlockBase
	{
		public CoroutineResumeBlock(Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Resume();
	}

	internal sealed class TimerCoroutineSetTimeScaleBlock : CoroutineControlBlockBase
	{
		private readonly Double _timeScale;

		public TimerCoroutineSetTimeScaleBlock(TimerCoroutine coroutine, Double timeScale)
			: base(coroutine) => _timeScale = timeScale;

		public override void Execute(IScriptRuntimeContext runtimeContext) => ((TimerCoroutine)_coroutine).TimeScale = _timeScale;
	}
}
