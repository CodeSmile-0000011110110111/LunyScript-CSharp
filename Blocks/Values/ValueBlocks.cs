using Luny;
using LunyScript.Execution;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that returns a constant variable value.
	/// </summary>
	internal sealed class ConstantValue : IScriptValue
	{
		private readonly Variable _value;

		public static IScriptValue Create(Variable value) => new ConstantValue(value);

		private ConstantValue(Variable value) => _value = value;

		public Variable Evaluate(ILunyScriptContext context) => _value;
	}

	/// <summary>
	/// Block that returns the current loop counter from the context stack.
	/// </summary>
	internal sealed class LoopCounterValue : IScriptValue
	{
		public static readonly LoopCounterValue Instance = new();

		private LoopCounterValue() {}

		public Variable Evaluate(ILunyScriptContext context) => context.LoopCount;
	}
}
