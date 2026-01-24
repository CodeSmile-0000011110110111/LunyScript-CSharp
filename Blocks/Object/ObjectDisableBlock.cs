using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectDisableSelfBlock : ILunyScriptBlock
	{
		public static ILunyScriptBlock Create() => new ObjectDisableSelfBlock();

		private ObjectDisableSelfBlock() {}

		public void Execute(ILunyScriptContext context) => context.LunyObject.IsEnabled = false;
	}

	internal sealed class ObjectDisableTargetBlock : ILunyScriptBlock
	{
		private readonly String _name;

		public static ILunyScriptBlock Create(String name) => new ObjectDisableTargetBlock(name);

		private ObjectDisableTargetBlock(String name) => _name = name;

		public void Execute(ILunyScriptContext context) =>
			throw new NotImplementedException($"{nameof(ObjectDisableTargetBlock)} with name '{_name}' not implemented");
	}
}
