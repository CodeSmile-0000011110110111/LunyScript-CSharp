using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Provides Editor-only functionality.
		/// In builds these blocks are ignored (no-op).
		/// </summary>
		public static class Editor
		{
			/// <summary>
			/// Pauses playmode.
			/// </summary>
			public static ILunyScriptBlock PausePlayer(String message = null) => EditorPausePlayerBlock.Create(message);
		}
	}
}
