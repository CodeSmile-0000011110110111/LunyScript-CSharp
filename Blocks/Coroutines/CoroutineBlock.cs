using LunyScript.Coroutines;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks.Coroutines
{
	/// <summary>
	/// Wraps a CoroutineInstance as a block for use in script sequences.
	/// Provides control methods (Start, Stop, Pause, Resume) as action blocks.
	/// </summary>
	internal class CoroutineBlock : IScriptCoroutineBlock
	{
		protected readonly CoroutineBase _coroutine;
		internal CoroutineBlock(CoroutineBase instance) => _coroutine = instance ?? throw new ArgumentNullException(nameof(instance));

		public virtual void Execute(ILunyScriptContext context) =>
			throw new NotImplementedException($"{nameof(CoroutineBlock)} cannot be used in a block sequence");

		public IScriptActionBlock Start() => new CoroutineStartBlock(_coroutine);
		public IScriptActionBlock Stop() => new CoroutineStopBlock(_coroutine);
		public IScriptActionBlock Pause() => new CoroutinePauseBlock(_coroutine);
		public IScriptActionBlock Resume() => new CoroutineResumeBlock(_coroutine);
	}

	internal sealed class CoroutineTimerBlock : CoroutineBlock, IScriptCoroutineTimerBlock
	{
		internal CoroutineTimerBlock(CoroutineBase instance)
			: base(instance) {}

		public IScriptActionBlock TimeScale(Double scale) => new CoroutineSetTimeScaleBlock(_coroutine, scale);
	}

	internal sealed class CoroutineCounterBlock : CoroutineBlock, IScriptCoroutineCounterBlock
	{
		internal CoroutineCounterBlock(CoroutineBase instance)
			: base(instance) {}
	}
}
