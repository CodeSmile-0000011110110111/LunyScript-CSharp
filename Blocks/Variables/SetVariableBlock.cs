using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal sealed class SetVariableBlock : IScriptActionBlock
	{
		private readonly Table.VarHandle _handle;
		private readonly IScriptVariable _value;

		public static IScriptActionBlock Create(Table.VarHandle handle, IScriptVariable value) =>
			new SetVariableBlock(handle, value);

		private SetVariableBlock(Table.VarHandle handle, IScriptVariable value)
		{
			_handle = handle ?? throw new ArgumentNullException(nameof(handle));
			_value = value ?? throw new ArgumentNullException(nameof(value));
		}

		public void Execute(ILunyScriptContext context) => _handle.Value = _value.GetValue(context);
	}
}
