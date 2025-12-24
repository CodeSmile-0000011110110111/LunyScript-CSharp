using Luny.Proxies;
using System;

namespace LunyScript
{
	/// <summary>
	/// Abstract base class for all LunyScripts.
	/// Provides the API interface for beginner-friendly visual scripting in C#.
	/// Users inherit from this class and implement Build() to construct their script logic.
	/// </summary>
	/// <remarks>
	/// Example script template (duplicate LunyScript.LunyScript avoids usings):
	///
	///		public sealed class ExampleLunyScript : LunyScript.LunyScript
	///		{
	///			public override void Build()
	///			{
	///				// define behaviour using LunyScript API here ...
	///			}
	///		}
	/// </remarks>
	public abstract partial class LunyScript
	{
		private ScriptContext _context;

		/// <summary>
		/// ScriptID of the script for identification.
		/// </summary>
		protected ScriptID ScriptID => _context.ScriptID;
		
		/// <summary>
		/// Reference to proxy for engine object.
		/// Caution: native engine reference could be null.
		/// Check EngineObject.IsValid before accessing.
		/// </summary>
		protected LunyObject EngineObject => _context.EngineObject;

		internal void Initialize(ScriptContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

		/// <summary>
		/// Called once when the script is initialized.
		/// Users construct their blocks (sequences, statemachines, behaviors) for execution here.
		/// Users can use regular C# syntax (ie call methods, use loops) to construct complex and/or reusable blocks.
		/// </summary>
		public abstract void Build();
	}
}
