namespace LunyScript
{
	public abstract partial class LunyScript
	{
		// User-facing API: Variables
		protected Variables Variables => _context.LocalVariables;
		protected Variables GlobalVariables => _context.GlobalVariables;
		protected Variables InspectorVariables => _context.InspectorVariables;
	}
}
