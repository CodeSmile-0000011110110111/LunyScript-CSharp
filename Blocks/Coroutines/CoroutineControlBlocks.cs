using LunyScript.Execution;
using LunyScript.Coroutines;
using System;

namespace LunyScript.Blocks.Coroutines
{
	internal abstract class CoroutineControlBlockBase : IScriptActionBlock
	{
		protected readonly CoroutineBase Instance;
		protected CoroutineControlBlockBase(CoroutineBase instance) => Instance = instance;
		public abstract void Execute(ILunyScriptContext context);
	}

	internal sealed class CoroutineStartBlock : CoroutineControlBlockBase
	{
		public CoroutineStartBlock(CoroutineBase instance) : base(instance) {}
		public override void Execute(ILunyScriptContext context)
		{
			if (Instance.Start())
				Instance.OnStartedSequence?.Execute(context);
		}
	}

	internal sealed class CoroutineStopBlock : CoroutineControlBlockBase
	{
		public CoroutineStopBlock(CoroutineBase instance) : base(instance) {}
		public override void Execute(ILunyScriptContext context)
		{
			if (Instance.Stop())
				Instance.OnStoppedSequence?.Execute(context);
		}
	}

	internal sealed class CoroutinePauseBlock : CoroutineControlBlockBase
	{
		public CoroutinePauseBlock(CoroutineBase instance) : base(instance) {}
		public override void Execute(ILunyScriptContext context)
		{
			if (Instance.Pause())
				Instance.OnPausedSequence?.Execute(context);
		}
	}

	internal sealed class CoroutineResumeBlock : CoroutineControlBlockBase
	{
		public CoroutineResumeBlock(CoroutineBase instance) : base(instance) {}
		public override void Execute(ILunyScriptContext context)
		{
			if (Instance.Resume())
				Instance.OnResumedSequence?.Execute(context);
		}
	}
}
