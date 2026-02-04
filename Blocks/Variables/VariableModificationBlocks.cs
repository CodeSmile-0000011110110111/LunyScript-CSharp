using Luny;
using LunyScript.Execution;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal abstract class VariableModificationBlockBase
	{
		protected readonly Table.VarHandle _handle;
		protected readonly IScriptVariable _value;

		protected VariableModificationBlockBase(Table.VarHandle handle, IScriptVariable value)
		{
			_handle = handle ?? throw new ArgumentNullException(nameof(handle));
			_value = value ?? throw new ArgumentNullException(nameof(value));
		}
	}

	internal sealed class SetVariableBlock : VariableModificationBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static SetVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private SetVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Variable GetValue(ILunyScriptContext context) => _value.GetValue(context);
	}

	internal sealed class AddVariableBlock : VariableModificationBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static AddVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private AddVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Variable GetValue(ILunyScriptContext context) => _handle.Value + (Double)_value.GetValue(context);
	}

	internal sealed class SubtractVariableBlock : VariableModificationBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static SubtractVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private SubtractVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Variable GetValue(ILunyScriptContext context) => _handle.Value - (Double)_value.GetValue(context);
	}

	internal sealed class MultiplyVariableBlock : VariableModificationBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static MultiplyVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private MultiplyVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Variable GetValue(ILunyScriptContext context) => _handle.Value * (Double)_value.GetValue(context);
	}

	internal sealed class DivideVariableBlock : VariableModificationBlockBase, IScriptActionBlock, IScriptVariable
	{
		public static DivideVariableBlock Create(Table.VarHandle handle, IScriptVariable value) => new(handle, value);

		private DivideVariableBlock(Table.VarHandle handle, IScriptVariable value)
			: base(handle, value) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Variable GetValue(ILunyScriptContext context) => _handle.Value / (Double)_value.GetValue(context);
	}

	internal sealed class ToggleBooleanVariableBlock : IScriptActionBlock, IScriptVariable
	{
		private readonly Table.VarHandle _handle;

		public static IScriptActionBlock Create(Table.VarHandle handle) => new ToggleBooleanVariableBlock(handle);

		private ToggleBooleanVariableBlock(Table.VarHandle handle) => _handle = handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Execute(ILunyScriptContext context) => _handle.Value = GetValue(context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Variable GetValue(ILunyScriptContext context) => !_handle.Value.AsBoolean();
	}
}
