using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		public readonly struct EngineApi
		{
			private readonly ILunyScript _script;
			internal EngineApi(ILunyScript script) => _script = script;

			/// <summary>
			/// Logs a message that appears in both debug and release builds.
			/// Posts to both Luny internal log (if enabled) and engine logging.
			/// </summary>
			public IScriptActionBlock Log(String message) => EngineLogBlock.Create(message);
		}
	}
}
