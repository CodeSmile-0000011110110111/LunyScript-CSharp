using Luny;
using LunyScript.Execution;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that returns a constant variable value.
	/// </summary>
	internal sealed class ConstantVariable : IScriptVariable
	{
		private readonly Variable _value;

		public static IScriptVariable Create(Variable value) => new ConstantVariable(value);

		private ConstantVariable(Variable value) => _value = value;

		public Variable GetValue(ILunyScriptContext context) => _value;
	}

	// FIXME: I don't see the value of this using the IScriptValue interface
	/// <summary>
	/// Block that returns the current loop counter from the context stack.
	/// </summary>
	internal sealed class LoopCounterVariable : IScriptVariable
	{
		public static readonly LoopCounterVariable Instance = new();
		public static Variable Get(ILunyScriptContext context) => context.LoopCount;

		private LoopCounterVariable() {}

		public Variable GetValue(ILunyScriptContext context) => context.LoopCount;
	}
}
