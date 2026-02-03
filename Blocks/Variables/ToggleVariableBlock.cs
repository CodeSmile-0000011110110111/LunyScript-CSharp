using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal sealed class ToggleVariableBlock : IScriptActionBlock
	{
		private readonly Table.VarHandle _handle;

		public static IScriptActionBlock Create(Table.VarHandle handle) =>
			new ToggleVariableBlock(handle);

		private ToggleVariableBlock(Table.VarHandle handle) => _handle = handle;

		public void Execute(ILunyScriptContext context) => _handle.Value = (Variable)!_handle.Value.AsBoolean();
	}
}
