using Luny;
using LunyScript.Execution;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that returns a constant variable value.
	/// </summary>
	internal sealed class Constant : IScriptVariable
	{
		private readonly Variable _value;

		public static IScriptVariable Create(Variable value) => new Constant(value);

		private Constant(Variable value) => _value = value;

		public Variable GetValue(ILunyScriptContext context) => _value;
	}

	/// <summary>
	/// Block that returns the current loop counter from the context stack.
	/// </summary>
	internal sealed class LoopCounter : IScriptVariable
	{
		public static readonly LoopCounter Instance = new();
		public static Variable Get(ILunyScriptContext context) => context.LoopCount;

		private LoopCounter() {}

		public Variable GetValue(ILunyScriptContext context) => context.LoopCount;
	}
}
