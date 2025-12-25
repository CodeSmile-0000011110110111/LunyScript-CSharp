using System.Diagnostics.CodeAnalysis;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		// User-facing API: Variables
		[NotNull] public static Variables GlobalVariables => ScriptContext.GlobalVariables;
		[NotNull] protected Variables LocalVariables => _context.LocalVariables;
		[NotNull] protected Variables InspectorVariables => _context.InspectorVariables;
	}
}
