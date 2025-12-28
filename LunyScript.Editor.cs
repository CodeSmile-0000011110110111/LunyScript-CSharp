using LunyScript.Blocks;
using LunyScript.Interfaces;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Pauses playmode. Editor only.
		/// </summary>
		protected static IBlock EditorPausePlayer(String message = null) => new EditorPausePlayerBlock(message);
	}
}
