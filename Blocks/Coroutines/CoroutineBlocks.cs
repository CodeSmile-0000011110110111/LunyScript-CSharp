using LunyScript.Coroutines;
using System;

namespace LunyScript.Blocks.Coroutines
{
	/// <summary>
	/// Wraps a CoroutineInstance as a block for use in script sequences.
	/// Provides control methods (Start, Stop, Pause, Resume) as action blocks.
	/// </summary>
	internal class CoroutineBlock : Blocks.CoroutineBlock
	{
		protected readonly Coroutine _coroutine;

		internal static Blocks.CoroutineBlock Create(Coroutine coroutine) => coroutine switch
		{
			TimerCoroutine timer => new TimerCoroutineBlock(timer),
			CounterCoroutine counter => new CounterCoroutineBlock(counter),
			var _ => new CoroutineBlock(coroutine),
		};

		protected CoroutineBlock(Coroutine coroutine) => _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

		public override void Execute(IScriptRuntimeContext runtimeContext) =>
			throw new NotImplementedException($"{nameof(CoroutineBlock)} cannot be used in a block sequence");

		public override ScriptActionBlock Start() => new CoroutineStartBlock(_coroutine);
		public override ScriptActionBlock Stop() => new CoroutineStopBlock(_coroutine);
		public override ScriptActionBlock Pause() => new CoroutinePauseBlock(_coroutine);
		public override ScriptActionBlock Resume() => new CoroutineResumeBlock(_coroutine);
	}

	internal sealed class TimerCoroutineBlock : CoroutineBlock, ITimerCoroutineBlock
	{
		internal TimerCoroutineBlock(TimerCoroutine coroutine)
			: base(coroutine) {}

		public ScriptActionBlock TimeScale(Double scale) => new TimerCoroutineSetTimeScaleBlock((TimerCoroutine)_coroutine, scale);
	}

	internal sealed class CounterCoroutineBlock : CoroutineBlock, ICounterCoroutineBlock
	{
		internal CounterCoroutineBlock(CounterCoroutine coroutine)
			: base(coroutine) {}
	}
}
