using LunyScript.Coroutines;
using System;

namespace LunyScript.Blocks.Coroutines
{
	internal abstract class CoroutineControlBlock : IScriptActionBlock
	{
		protected readonly Coroutine _coroutine;

		protected CoroutineControlBlock(Coroutine coroutine) =>
			_coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

		public abstract void Execute(IScriptRuntimeContext runtimeContext);
	}

	internal sealed class CoroutineStartBlock : CoroutineControlBlock
	{
		public CoroutineStartBlock(Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Start();
	}

	internal sealed class CoroutineStopBlock : CoroutineControlBlock
	{
		public CoroutineStopBlock(Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Stop();
	}

	internal sealed class CoroutinePauseBlock : CoroutineControlBlock
	{
		public CoroutinePauseBlock(Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Pause();
	}

	internal sealed class CoroutineResumeBlock : CoroutineControlBlock
	{
		public CoroutineResumeBlock(Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Resume();
	}

	internal sealed class TimerCoroutineSetTimeScaleBlock : CoroutineControlBlock
	{
		private readonly Double _timeScale;

		public TimerCoroutineSetTimeScaleBlock(TimerCoroutine coroutine, Double timeScale)
			: base(coroutine) => _timeScale = timeScale;

		public override void Execute(IScriptRuntimeContext runtimeContext) => ((TimerCoroutine)_coroutine).TimeScale = _timeScale;
	}
}
