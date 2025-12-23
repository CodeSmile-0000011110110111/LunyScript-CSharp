using Luny;
using Luny.Proxies;
using LunyScript.Interfaces;
using System;
using System.Diagnostics;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Debug-only block that triggers a breakpoint when hit.
	/// Completely stripped in release builds unless DEBUG or LUNYSCRIPT_DEBUG defined.
	/// </summary>
	public sealed class DebugBreakBlock : IBlock
	{
		private readonly String _message;

		public DebugBreakBlock(String message = null)
		{
			_message = message;
		}

		public void Execute(ScriptContext context)
		{
			DoBreak(context);
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void DoBreak(ScriptContext context)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			if (_message != null)
				LunyLogger.LogWarning($"DebugBreak: {_message}", context.EngineObject);

			Debugger.Break();
#endif
		}

		public override String ToString() => $"DebugBreak({_message ?? "no message"})";
	}
}
