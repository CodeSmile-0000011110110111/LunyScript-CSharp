using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ObjectEnableSelfBlock : ILunyScriptBlock
	{
		public static ILunyScriptBlock Create() => new ObjectEnableSelfBlock();

		private ObjectEnableSelfBlock() {}

		public void Execute(ILunyScriptContext context) => context.LunyObject.IsEnabled = true;
	}

	internal sealed class ObjectEnableTargetBlock : ILunyScriptBlock
	{
		private readonly String _name;

		public static ILunyScriptBlock Create(String name) => new ObjectEnableTargetBlock(name);

		private ObjectEnableTargetBlock(String name) => _name = name;

		public void Execute(ILunyScriptContext context) => 
			throw new NotImplementedException($"{nameof(ObjectEnableTargetBlock)} with name '{_name}' not implemented");
	}
}
