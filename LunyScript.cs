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
		/// <summary>
		/// Global variables which all objects and scripts can read/write.
		/// </summary>
		[NotNull] public ILunyScriptVariables GlobalVariables => _context.GlobalVariables;
		/// <summary>
		/// Short alias for 'GlobalVariables'.
		/// </summary>
		[NotNull] public ILunyScriptVariables GVars => _context.GlobalVariables;
		/// <summary>
		/// Local variables the current object and script owns.
		/// If multiple objects run the same script, each object has its own unique set of local variables.
		/// </summary>
		[NotNull] public ILunyScriptVariables LocalVariables => _context.LocalVariables;
		/// <summary>
		/// Short alias for 'LocalVariables'.
		/// </summary>
		[NotNull] public ILunyScriptVariables LVars => _context.LocalVariables;
		/// <summary>
		/// True if the script runs within the engine's editor (play mode). False in builds.
		/// </summary>
		public Boolean IsEditor => LunyEngineInternal.Instance.Application.IsEditor;

		/// <summary>
		/// Logs a message that appears in both debug and release builds.
		/// Posts to both Luny internal log (if enabled) and engine logging.
		/// </summary>
		protected static ILunyScriptBlock Log(String message) => EngineLogBlock.Create(message);

		/// <summary>
		/// Runs the contained method or lambda when this block executes.
		/// </summary>
		/// <remarks>
		/// Intended for quick prototyping and testing only: lambdas are not reusable building blocks.
		///
		/// Prefer to convert "Run" code into a custom IBlock class after its initial development and testing,
		/// If not that, use named methods rather than lambdas - this ensures the block-based code continues to read like intent.
		///
		///		// Even a single-line lambda adds notable 'syntax noise' (worse for multi-line lambdas):
		/// 	OnUpdate(Run(() => LunyLogger.LogInfo("custom log inline")));
		///
		///		// A named method is much cleaner, and re-usable in the same script:
		///		OnUpdate(Run(MyLog));
		///		private Action MyLog() => () => LunyLogger.LogInfo("custom log method");
		///
		///		// C# extension methods for LunyScript also work nice but require the 'this' prefix:
		///		OnUpdate(Run(this.MyLog()));
		///		public static MyLunyScriptExtensions
		///		{
		///			public static Action MyLog(this LunyScript ls) =>
		///				() => LunyLogger.LogInfo("custom log ext method");
		///		}
		///
		///		// Best: Create your own static factory class returning IBlock instances.
		///		// The block's Execute method has access to the context (object reference, variables, etc.).
		///		// The static Create() method helps to later adapt the creation code without having to modify callers.
		///		OnUpdate(MyBlocks.MyLog());
		///		internal static MyBlocks
		///		{
		///			public static ILunyScriptBlock MyLog() => MyLogBlock.Create();
		///			internal sealed class MyLogBlock : ILunyScriptBlock
		///			{
		///				public static ILunyScriptBlock Create() => new MyLogBlock();
		///				public void Execute(ILunyScriptContext context) => LunyLogger.LogInfo("custom log block");
		///			}
		///		}
		/// </remarks>
		/// <param name="action"></param>
		/// <returns></returns>
		protected static ILunyScriptBlock Run(Action action) => RunActionBlock.Create(_ => action());
		/// <summary>
		/// Run overload whose action receives the ILunyScriptContext instance.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		protected static ILunyScriptBlock Run(Action<ILunyScriptContext> action) => RunActionBlock.Create(action);

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
