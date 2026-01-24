using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectDestroySelfBlock : ILunyScriptBlock
	{
		public static ILunyScriptBlock Create() => new ObjectDestroySelfBlock();

		private ObjectDestroySelfBlock() {}

		public void Execute(ILunyScriptContext context) => context.LunyObject.Destroy();
	}

	internal sealed class ObjectDestroyTargetBlock : ILunyScriptBlock
	{
		private readonly String _name;

		public static ILunyScriptBlock Create(String name) => new ObjectDestroyTargetBlock(name);

		private ObjectDestroyTargetBlock(String name) => _name = name;

		public void Execute(ILunyScriptContext context) => LunyEngine.Instance.Objects.GetByName(_name)?.Destroy();
	}
}
