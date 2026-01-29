using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectEnableSelfBlock : IScriptActionBlock
	{
		public static IScriptActionBlock Create() => new ObjectEnableSelfBlock();

		private ObjectEnableSelfBlock() {}

		public void Execute(ILunyScriptContext context) => context.LunyObject.IsEnabled = true;
	}

	internal sealed class ObjectEnableTargetBlock : IScriptActionBlock
	{
		private readonly String _name;

		public static IScriptActionBlock Create(String name) => new ObjectEnableTargetBlock(name);

		private ObjectEnableTargetBlock(String name) => _name = name;

		public void Execute(ILunyScriptContext context) =>
			throw new NotImplementedException($"{nameof(ObjectEnableTargetBlock)} with name '{_name}' not implemented");
	}
}
