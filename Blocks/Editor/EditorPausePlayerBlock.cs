using Luny;
using LunyScript.Execution;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only block that pauses the player (Editor-only).
	/// Completely stripped in release builds unless DEBUG or LUNYSCRIPT_DEBUG defined.
	/// </summary>
	internal sealed class EditorPausePlayerBlock : ILunyScriptBlock
	{
		private readonly String _message;

		public static ILunyScriptBlock Create(String message) =>
			LunyEngine.Instance.Application.IsEditor ? new EditorPausePlayerBlock(message) : null;

		private EditorPausePlayerBlock() {}
		private EditorPausePlayerBlock(String message = null) => _message = message;

		public void Execute(ILunyScriptContext context) => DoPausePlayer(context);

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoPausePlayer(ILunyScriptContext context)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (_message != null)
				LunyLogger.LogInfo($"{nameof(EditorPausePlayerBlock)}: {_message}", context.LunyObject);

			LunyEngine.Instance.Editor.PausePlayer();
#endif
		}

		public override String ToString() => $"{nameof(EditorPausePlayerBlock)}({_message ?? String.Empty})";
	}
}
