using Luny.Diagnostics;
using LunyScript.Interfaces;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only block that pauses the player (Editor-only).
	/// Completely stripped in release builds unless DEBUG or LUNYSCRIPT_DEBUG defined.
	/// </summary>
	public sealed class EditorPausePlayerBlock : IBlock
	{
		private readonly String _message;

		public EditorPausePlayerBlock(String message = null) => _message = message;

		public void Execute(ScriptContext context) => DoPausePlayer(context);

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoPausePlayer(ScriptContext context)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (_message != null)
				LunyLogger.LogInfo($"{nameof(EditorPausePlayerBlock)}: {_message}", context.EngineObject);

			context.Engine.Editor.PausePlayer();
#endif
		}

		public override String ToString() => $"{nameof(EditorPausePlayerBlock)}({_message ?? String.Empty})";
	}
}
