using LunyScript.Execution;
using LunyScript.Coroutines;
using System;

namespace LunyScript.Blocks.Coroutines
{
	/// <summary>
	/// Wraps a CoroutineInstance as a block for use in script sequences.
	/// Provides control methods (Start, Stop, Pause, Resume) as action blocks.
	/// </summary>
	internal sealed class CoroutineBlock : IScriptCoroutineBlock
	{
		private readonly CoroutineBase _instance;

		internal CoroutineBlock(CoroutineBase instance) =>
			_instance = instance ?? throw new ArgumentNullException(nameof(instance));

		public void Execute(ILunyScriptContext context) => new CoroutineStartBlock(_instance).Execute(context);

		public IScriptActionBlock Start() => new CoroutineStartBlock(_instance);
		public IScriptActionBlock Stop() => new CoroutineStopBlock(_instance);
		public IScriptActionBlock Pause() => new CoroutinePauseBlock(_instance);
		public IScriptActionBlock Resume() => new CoroutineResumeBlock(_instance);
		public void TimeScale(Double scale) => _instance.SetTimeScale(scale);
	}
}
