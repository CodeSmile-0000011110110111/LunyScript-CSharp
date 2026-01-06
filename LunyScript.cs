using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Execution;
using System;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript
{
	public interface ILunyScript
	{
		LunyScriptID ScriptID { get; }
		ILunyObject LunyObject { get; }
		ILunyScriptVariables GlobalVariables { get; }
		ILunyScriptVariables LocalVariables { get; }
		Boolean IsEditor { get; }
	}

	/// <summary>
	/// Abstract base class for all LunyScripts.
	/// Provides the API interface for beginner-friendly visual scripting in C#.
	/// Users inherit from this class and implement Build() to construct their script logic.
	/// </summary>
	/// <remarks>
	/// Example script template (duplicate LunyScript.LunyScript is correct):
	///
	///		public class ExampleLunyScript : LunyScript.LunyScript
	///		{
	///			public override void Build()
	///			{
	///				// define behaviour using LunyScript API here ...
	///				OnUpdate(Debug.Log("Hello, LunyScript!"));
	///			}
	///		}
	/// </remarks>
	public abstract partial class LunyScript : ILunyScript
	{
		// temporary 'singleton' for static subclasses (eg 'Every')
		private static LunyScript s_Instance;

		private ILunyScriptContext _context;

		/// <summary>
		/// ScriptID of the script for identification.
		/// </summary>
		public LunyScriptID ScriptID => _context.ScriptID;
		/// <summary>
		/// Reference to proxy for engine object.
		/// Caution: native engine reference could be null.
		/// Check EngineObject.IsValid before accessing.
		/// </summary>
		[MaybeNull] public ILunyObject LunyObject => _context.LunyObject;
		// User-facing API: Variables
		/// <summary>
		/// Global variables which all objects and scripts can read/write.
		/// </summary>
		[NotNull] public ILunyScriptVariables GlobalVariables => _context.GlobalVariables;
		/// <summary>
		/// Local variables the current object and script owns.
		/// If multiple objects run the same script, each object has its own unique set of local variables.
		/// </summary>
		[NotNull] public ILunyScriptVariables LocalVariables => _context.LocalVariables;
		/// <summary>
		/// True if the script runs within the engine's editor (play mode). False in builds.
		/// </summary>
		public Boolean IsEditor => LunyEngine.Instance.Application.IsEditor;

		/// <summary>
		/// Logs a message that appears in both debug and release builds.
		/// Posts to both Luny internal log (if enabled) and engine logging.
		/// </summary>
		protected static ILunyScriptBlock Log(String message) => new EngineLogBlock(message);

		/// <summary>
		/// Runs the contained method or lambda when this block executes. Meant for custom code and quick prototyping.
		/// </summary>
		/// <remarks>
		/// Prefer to convert "Run" code into a custom IBlock class after its initial development and testing,
		/// or at least prefer named methods over lambdas or assign lambdas to fields. Any of these makes that code
		/// re-usable and more readable. Example:
		///
		///		// even a single-line lambda adds more noise (worse for multi-line lambdas):
		/// 	OnUpdate(Run(() => LunyLogger.LogInfo("custom lambda runs")));
		///
		///		// a named method or lambda field (not shown) is cleaner, and re-usable in the same script:
		///		OnUpdate(Run(MyCustomCode));
		///
		///		// a custom IBlock implementation is also clean, and re-usable in all scripts:
		///		OnUpdate(new MyCustomCodeBlock());
		///
		///		// even better: create your own static factory class returning IBlock instances:
		///		OnUpdate(MyBlocks.MyCustomCode());
		///
		///		// a LunyScript C# extension methods are also fine but require the 'this' prefix:
		///		OnUpdate(this.MyCustomCode());
		/// </remarks>
		/// <param name="action"></param>
		/// <returns></returns>
		protected static ILunyScriptBlock Run(Action action) => new RunActionBlock(_ => action());

		internal void Initialize(ILunyScriptContext context)
		{
			s_Instance = this;
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		~LunyScript() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Shutdown() => s_Instance = null; // temp singleton no longer needed

		/// <summary>
		/// Called once when the script is initialized.
		/// Users construct their blocks (sequences, statemachines, behaviors) for execution here.
		/// Users can use regular C# syntax (ie call methods, use loops) to construct complex and/or reusable blocks.
		/// </summary>
		public abstract void Build();
	}
}
