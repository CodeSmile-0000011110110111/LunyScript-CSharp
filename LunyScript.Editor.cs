using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Pauses playmode. Editor only.
		/// </summary>
		protected static EditorPausePlayerBlock EditorPausePlayer(String message = null) => new(message);
	}
}
