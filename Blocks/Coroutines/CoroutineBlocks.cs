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

		internal static T Create<T>(Coroutine coroutine) where T : class, IScriptCoroutineBlock
		{
			IScriptCoroutineBlock block = coroutine switch
			{
				TimerCoroutine timer => new CoroutineTimerBlock(timer),
				CounterCoroutine counter => new CoroutineCounterBlock(counter),
				var _ => new CoroutineBlock(coroutine),
			};

			if (block is T typedBlock)
				return typedBlock;

			throw new InvalidOperationException($"Coroutine type {coroutine.GetType().Name} has no matching {typeof(T).Name} block type");
		}

		protected CoroutineBlock(Coroutine coroutine) => _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

		public virtual void Execute(IScriptRuntimeContext runtimeContext) =>
			throw new NotImplementedException($"{nameof(CoroutineBlock)} cannot be used in a block sequence");

		public IScriptActionBlock Start() => new CoroutineStartBlock(_coroutine);
		public IScriptActionBlock Stop() => new CoroutineStopBlock(_coroutine);
		public IScriptActionBlock Pause() => new CoroutinePauseBlock(_coroutine);
		public IScriptActionBlock Resume() => new CoroutineResumeBlock(_coroutine);
	}

	internal sealed class CoroutineTimerBlock : CoroutineBlock, IScriptCoroutineTimerBlock
	{
		internal CoroutineTimerBlock(TimerCoroutine coroutine)
			: base(coroutine) {}

		public IScriptActionBlock TimeScale(Double scale) => new CoroutineSetTimeScaleBlock((TimerCoroutine)_coroutine, scale);
	}

	internal sealed class CoroutineCounterBlock : CoroutineBlock, IScriptCoroutineCounterBlock
	{
		internal CoroutineCounterBlock(CounterCoroutine coroutine)
			: base(coroutine) {}
	}
}
