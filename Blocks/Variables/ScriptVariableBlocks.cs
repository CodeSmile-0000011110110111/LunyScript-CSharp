using Luny;
using LunyScript.Execution;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that returns the current loop counter from the context stack.
	/// </summary>
	internal sealed class LoopCounter : ScriptVariableBlockBase
	{
		public static readonly LoopCounter Instance = new();
		private LoopCounter() {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => context.LoopCount;
	}

	/// <summary>
	/// Block that returns a constant variable value.
	/// </summary>
	public sealed class Constant : ScriptVariableBlockBase
	{
		private readonly Variable _value;

		public static Constant Create(Variable value) => new(value);

		private Constant(Variable value) => _value = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(ILunyScriptContext context) => _value;

		public override String ToString() => _value.ToString();
	}
}
