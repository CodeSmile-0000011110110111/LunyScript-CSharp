namespace LunyScript
{
	public abstract partial class LunyScript
	{
		// User-facing API: Variables
		protected Variables LocalVariables => _context.LocalVariables;
		protected Variables GlobalVariables => ScriptContext.GlobalVariables;
		protected Variables InspectorVariables => _context.InspectorVariables;
	}
}
