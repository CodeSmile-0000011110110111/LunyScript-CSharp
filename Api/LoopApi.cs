using LunyScript.Blocks;

namespace LunyScript.Api
{
	public readonly struct LoopApi
	{
		private readonly LunyScript _script;
		internal LoopApi(LunyScript script) => _script = script;

		/// <summary>
		/// Returns the current iteration count of the innermost surrounding loop.
		/// Resolves at runtime via ILunyScriptContext.
		/// </summary>
		public IScriptVariable Counter => LoopCounter.Instance;
	}
}
