using LunyScript.Coroutines;
using System;
using Coroutines_Coroutine = LunyScript.Coroutines.Coroutine;

namespace LunyScript.Blocks.Coroutines
{
	/// <summary>
	/// Wraps a CoroutineInstance as a block for use in script sequences.
	/// Provides control methods (Start, Stop, Pause, Resume) as action blocks.
	/// </summary>
	internal class CoroutineBlock : IScriptCoroutineBlock
	{
		protected readonly Coroutines_Coroutine _coroutine;

		internal static T Create<T>(Coroutines_Coroutine coroutine) where T : class, IScriptCoroutineBlock => coroutine switch
		{
			TimerCoroutine => new CoroutineTimerBlock(coroutine),
			CounterCoroutine or TimeSliceCoroutine => new CoroutineCounterBlock(coroutine),
			var _ => new CoroutineBlock(coroutine),
		} as T;

		protected CoroutineBlock(Coroutines_Coroutine coroutine) =>
			_coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

		public virtual void Execute(IScriptRuntimeContext runtimeContext) =>
			throw new NotImplementedException($"{nameof(CoroutineBlock)} cannot be used in a block sequence");

		public IScriptActionBlock Start() => new CoroutineStartBlock(_coroutine);
		public IScriptActionBlock Stop() => new CoroutineStopBlock(_coroutine);
		public IScriptActionBlock Pause() => new CoroutinePauseBlock(_coroutine);
		public IScriptActionBlock Resume() => new CoroutineResumeBlock(_coroutine);
	}

	internal sealed class CoroutineTimerBlock : CoroutineBlock, IScriptCoroutineTimerBlock
	{
		internal CoroutineTimerBlock(Coroutines_Coroutine coroutine)
			: base(coroutine) {}

		public IScriptActionBlock TimeScale(Double scale) => new CoroutineSetTimeScaleBlock(_coroutine, scale);
	}

	internal sealed class CoroutineCounterBlock : CoroutineBlock, IScriptCoroutineCounterBlock
	{
		internal CoroutineCounterBlock(Coroutines_Coroutine coroutine)
			: base(coroutine) {}
	}
}
