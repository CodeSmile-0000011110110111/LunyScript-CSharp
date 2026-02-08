using LunyScript.Coroutines;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks.Coroutines
{
	/// <summary>
	/// Wraps a CoroutineInstance as a block for use in script sequences.
	/// Provides control methods (Start, Stop, Pause, Resume) as action blocks.
	/// </summary>
	internal sealed class CoroutineBlock : IScriptCoroutineBlock
	{
		private readonly CoroutineBase _coroutine;
		internal CoroutineBlock(CoroutineBase instance) => _coroutine = instance ?? throw new ArgumentNullException(nameof(instance));

		public void Execute(ILunyScriptContext context) =>
			throw new NotImplementedException($"{nameof(CoroutineBlock)} should not be used in a block sequence");

		public IScriptActionBlock Start() => new CoroutineStartBlock(_coroutine);
		public IScriptActionBlock Stop() => new CoroutineStopBlock(_coroutine);
		public IScriptActionBlock Pause() => new CoroutinePauseBlock(_coroutine);
		public IScriptActionBlock Resume() => new CoroutineResumeBlock(_coroutine);
		public void SetTimeScale(Double scale) => _coroutine.TimeScale = scale;
	}
}
