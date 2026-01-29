using LunyScript.Blocks;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Provides access to loop-related dynamic values.
		/// </summary>
		public LoopApi Loop => new(this);

		public readonly struct LoopApi
		{
			private readonly LunyScript _script;
			internal LoopApi(LunyScript script) => _script = script;

			/// <summary>
			/// Returns the current iteration count of the innermost surrounding loop.
			/// Resolves at runtime via ILunyScriptContext.
			/// </summary>
			public IScriptVariable Counter => LoopCounterVariable.Instance;
		}
	}
}
