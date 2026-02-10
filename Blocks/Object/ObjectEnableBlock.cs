using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectEnableSelfBlock : IScriptActionBlock
	{
		public static IScriptActionBlock Create() => new ObjectEnableSelfBlock();

		private ObjectEnableSelfBlock() {}

		public void Execute(IScriptRuntimeContext runtimeContext) => runtimeContext.LunyObject.IsEnabled = true;
	}

	internal sealed class ObjectEnableTargetBlock : IScriptActionBlock
	{
		private readonly String _name;

		public static IScriptActionBlock Create(String name) => new ObjectEnableTargetBlock(name);

		private ObjectEnableTargetBlock(String name) => _name = name;

		public void Execute(IScriptRuntimeContext runtimeContext) =>
			throw new NotImplementedException($"{nameof(ObjectEnableTargetBlock)} with name '{_name}' not implemented");
	}
}
