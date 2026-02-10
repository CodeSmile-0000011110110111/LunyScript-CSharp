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
				PerpetualCounterStyleCoroutine counterStyle => new CoroutineCounterBlock(counterStyle),
				_ => new CoroutineBlock(coroutine)
			};

			if (block is T typedBlock)
				return typedBlock;

			throw new InvalidOperationException($"Coroutine of type {coroutine.GetType().Name} cannot be wrapped as {typeof(T).Name}");
		}

		protected CoroutineBlock(Coroutine coroutine) =>
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
		internal CoroutineTimerBlock(Coroutine coroutine)
			: base(coroutine) {}

		public IScriptActionBlock TimeScale(Double scale) => new CoroutineSetTimeScaleBlock(_coroutine, scale);
	}

	internal sealed class CoroutineCounterBlock : CoroutineBlock, IScriptCoroutineCounterBlock
	{
		internal CoroutineCounterBlock(Coroutine coroutine)
			: base(coroutine) {}
	}
}
