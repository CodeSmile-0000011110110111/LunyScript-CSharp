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
		public readonly struct EditorApi
		{
			private readonly ILunyScript _script;
			internal EditorApi(ILunyScript script) => _script = script;

			/// <summary>
			/// Pauses playmode.
			/// </summary>
			public IScriptActionBlock PausePlayer(String message = null) => EditorPausePlayerBlock.Create(message);
		}
	}
}
