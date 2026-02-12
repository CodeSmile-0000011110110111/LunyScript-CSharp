using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Provides Editor-only functionality.
	/// In builds these blocks are ignored (no-op).
	/// </summary>
	public readonly struct EditorApi
	{
		private readonly IScript _script;
		internal EditorApi(IScript script) => _script = script;

		/// <summary>
		/// Pauses playmode.
		/// </summary>
		public IScriptActionBlock PausePlayer(String message = null) => EditorPausePlayerBlock.Create(message);
	}
}
