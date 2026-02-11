using LunyScript.Coroutines;
using System;

namespace LunyScript.Blocks.Coroutines
{
	/// <summary>
	/// Wraps a CoroutineInstance as a block for use in script sequences.
	/// Provides control methods (Start, Stop, Pause, Resume) as action blocks.
	/// </summary>
	internal class CoroutineBlock : IScriptCoroutineBlock
	{
		protected readonly Coroutine _coroutine;

		internal static IScriptCoroutineBlock Create(Coroutine coroutine) => coroutine switch
		{
			TimerCoroutine timer => new TimerCoroutineBlock(timer),
			CounterCoroutine counter => new CounterCoroutineBlock(counter),
			var _ => new CoroutineBlock(coroutine),
		};

		protected CoroutineBlock(Coroutine coroutine) => _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

		public virtual void Execute(IScriptRuntimeContext runtimeContext) =>
			throw new NotImplementedException($"{nameof(CoroutineBlock)} cannot be used in a block sequence");

		public IScriptActionBlock Start() => new CoroutineStartBlock(_coroutine);
		public IScriptActionBlock Stop() => new CoroutineStopBlock(_coroutine);
		public IScriptActionBlock Pause() => new CoroutinePauseBlock(_coroutine);
		public IScriptActionBlock Resume() => new CoroutineResumeBlock(_coroutine);
	}

	internal sealed class TimerCoroutineBlock : CoroutineBlock, IScriptTimerCoroutineBlock
	{
		internal TimerCoroutineBlock(TimerCoroutine coroutine)
			: base(coroutine) {}

		public IScriptActionBlock TimeScale(Double scale) => new TimerCoroutineSetTimeScaleBlock((TimerCoroutine)_coroutine, scale);
	}

	internal sealed class CounterCoroutineBlock : CoroutineBlock, IScriptCounterCoroutineBlock
	{
		internal CounterCoroutineBlock(CounterCoroutine coroutine)
			: base(coroutine) {}
	}
}
