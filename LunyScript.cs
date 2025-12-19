using System;

namespace LunyScript
{
	/// <summary>
	/// Abstract base class for all LunyScripts.
	/// Provides the API interface for beginner-friendly visual scripting in C#.
	/// Users inherit from this class and implement Build() to construct their script logic.
	/// </summary>
	public abstract partial class LunyScript
	{
		private RunContext _context;

		internal void Initialize(RunContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

		/// <summary>
		/// Called once when the script is initialized.
		/// Users construct their blocks and register them for execution here.
		/// </summary>
		public abstract void Build();
	}
}
