using Luny.Diagnostics;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only block that triggers a breakpoint when hit.
	/// Completely stripped in release builds unless DEBUG or LUNYSCRIPT_DEBUG defined.
	/// </summary>
	internal sealed class DebugBreakBlock : IBlock
	{
		private readonly String _message;

		public DebugBreakBlock(String message = null) => _message = message;

		public void Execute(IScriptContext context) => DoBreak(context);

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoBreak(IScriptContext context)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (_message != null)
				LunyLogger.LogInfo($"{nameof(DebugBreakBlock)}: {_message}", context.LunyObject);

			Debugger.Break();
#endif
		}

		public override String ToString() => $"{nameof(DebugBreakBlock)}({_message ?? String.Empty})";
	}
}
