using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		public static class Engine
		{
			/// <summary>
			/// Logs a message that appears in both debug and release builds.
			/// Posts to both Luny internal log (if enabled) and engine logging.
			/// </summary>
			public static ILunyScriptBlock Log(String message) => EngineLogBlock.Create(message);
		}
	}
}
