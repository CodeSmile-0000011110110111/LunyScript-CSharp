using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectDestroySelfBlock : IScriptActionBlock
	{
		public static IScriptActionBlock Create() => new ObjectDestroySelfBlock();

		private ObjectDestroySelfBlock() {}

		public void Execute(ILunyScriptContext context) => context.LunyObject.Destroy();
	}

	internal sealed class ObjectDestroyTargetBlock : IScriptActionBlock
	{
		private readonly String _name;

		public static IScriptActionBlock Create(String name) => new ObjectDestroyTargetBlock(name);

		private ObjectDestroyTargetBlock(String name) => _name = name;

		public void Execute(ILunyScriptContext context) => LunyEngine.Instance.Objects.GetByName(_name)?.Destroy();
	}
}
