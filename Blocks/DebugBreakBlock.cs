using Luny;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only block that triggers a breakpoint when hit.
	/// Completely stripped in release builds unless DEBUG/LUNY_DEBUG/LUNYSCRIPT_DEBUG defined.
	/// </summary>
	public sealed class DebugBreakBlock : IBlock
	{
		private readonly String _message;

		public DebugBreakBlock(String message = null)
		{
			_message = message;
		}

		public void Execute(RunContext context)
		{
			DoBreak(context);
		}

		[Conditional("DEBUG")]
		[Conditional("LUNY_DEBUG")]
		[Conditional("LUNYSCRIPT_DEBUG")]
		private void DoBreak(RunContext context)
		{
#if DEBUG || LUNY_DEBUG || LUNYSCRIPT_DEBUG
			if (_message != null)
				LunyLogger.LogWarning($"DebugBreak: {_message}", context.Object);

			Debugger.Break();
#endif
		}

		public override String ToString() => $"DebugBreak({_message ?? "no message"})";
	}
}
