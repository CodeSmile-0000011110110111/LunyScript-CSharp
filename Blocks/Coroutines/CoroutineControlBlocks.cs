using System;
using Coroutines_Coroutine = LunyScript.Coroutines.Coroutine;

namespace LunyScript.Blocks.Coroutines
{
	internal abstract class CoroutineControlBlockBase : IScriptActionBlock
	{
		protected readonly Coroutines_Coroutine _coroutine;

		protected CoroutineControlBlockBase(Coroutines_Coroutine coroutine) =>
			_coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

		public abstract void Execute(IScriptRuntimeContext runtimeContext);
	}

	internal sealed class CoroutineStartBlock : CoroutineControlBlockBase
	{
		public CoroutineStartBlock(Coroutines_Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Start();
	}

	internal sealed class CoroutineStopBlock : CoroutineControlBlockBase
	{
		public CoroutineStopBlock(Coroutines_Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Stop();
	}

	internal sealed class CoroutinePauseBlock : CoroutineControlBlockBase
	{
		public CoroutinePauseBlock(Coroutines_Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Pause();
	}

	internal sealed class CoroutineResumeBlock : CoroutineControlBlockBase
	{
		public CoroutineResumeBlock(Coroutines_Coroutine coroutine)
			: base(coroutine) {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.Resume();
	}

	internal sealed class CoroutineSetTimeScaleBlock : CoroutineControlBlockBase
	{
		private readonly Double _timeScale;

		public CoroutineSetTimeScaleBlock(Coroutines_Coroutine coroutine, Double timeScale)
			: base(coroutine) => _timeScale = timeScale;

		public override void Execute(IScriptRuntimeContext runtimeContext) => _coroutine.TimeScale = _timeScale;
	}
}
