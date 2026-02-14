using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that holds a reference to a script variable.
	/// </summary>
	public sealed class TableVariableBlock : VariableBlock
	{
		private readonly Table.VarHandle _handle;

		internal override Table.VarHandle TargetHandle => _handle;
		internal Table.VarHandle VarHandle => _handle;

		public String Name => _handle.Name;
		public Variable Value => _handle.Value;

		internal static TableVariableBlock Create(Table.VarHandle handle) => new(handle);

		private TableVariableBlock(Table.VarHandle handle) => _handle = handle;

		public override String ToString() => _handle.ToString();

		// VariableBlock
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) => _handle.Value;
	}
}
