using Luny;
using Luny.Proxies;
using LunyScript.Blocks;
using LunyScript.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript
{
	public interface ILunyScript
	{
		ScriptID ScriptID { get; }
		ILunyObject EngineObject { get; }
		IVariables GlobalVariables { get; }
		IVariables LocalVariables { get; }
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
		private static LunyScript _instance;

		private IScriptContext _context;

		/// <summary>
		/// ScriptID of the script for identification.
		/// </summary>
		public ScriptID ScriptID => _context.ScriptID;
		/// <summary>
		/// Reference to proxy for engine object.
		/// Caution: native engine reference could be null.
		/// Check EngineObject.IsValid before accessing.
		/// </summary>
		[MaybeNull] public ILunyObject EngineObject => _context.EngineObject;
		// User-facing API: Variables
		/// <summary>
		/// Global variables which all objects and scripts can read/write.
		/// </summary>
		[NotNull] public IVariables GlobalVariables => _context.GlobalVariables;
		/// <summary>
		/// Local variables the current object and script owns.
		/// If multiple objects run the same script, each object has its own unique set of local variables.
		/// </summary>
		[NotNull] public IVariables LocalVariables => _context.LocalVariables;

		/// <summary>
		/// Logs a message that appears in both debug and release builds.
		/// Posts to both Luny internal log (if enabled) and engine logging.
		/// </summary>
		protected static IBlock Log(String message) => new EngineLogBlock(message);

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
		protected static IBlock Run(Action action) => new RunActionBlock(_ => action());

		internal void Initialize(IScriptContext context)
		{
			_instance = this;
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		internal void Shutdown()
		{
			// FIXME: keep context - Lambdas capturing context would throw if they access LunyScript properties
			// _context = null;

			_instance = null;
		}

		/// <summary>
		/// Called once when the script is initialized.
		/// Users construct their blocks (sequences, statemachines, behaviors) for execution here.
		/// Users can use regular C# syntax (ie call methods, use loops) to construct complex and/or reusable blocks.
		/// </summary>
		public abstract void Build();

		/// <summary>
		/// Provides diagnostics blocks which are omitted from release builds,
		/// unless the scripting symbol LUNYSCRIPT_DEBUG is defined.
		/// </summary>
		public static class Debug
		{
			/// <summary>
			/// Logs a debug message that is completely stripped in release builds.
			/// Only logs when DEBUG or LUNYSCRIPT_DEBUG is defined.
			/// </summary>
			public static IBlock Log(String message) => new DebugLogBlock(message);

			/// <summary>
			/// Triggers a debugger breakpoint (if debugger is attached).
			/// Completely stripped in release builds.
			/// Only breaks when DEBUG or LUNYSCRIPT_DEBUG is defined.
			/// </summary>
			public static IBlock Break(String message = null) => new DebugBreakBlock(message);
		}

		/// <summary>
		/// Provides Editor-only functionality.
		/// In builds these blocks are ignored (no-op).
		/// </summary>
		public static class Editor
		{
			/// <summary>
			/// Pauses playmode.
			/// </summary>
			public static IBlock PausePlayer(String message = null) => new EditorPausePlayerBlock(message);
		}

		/// <summary>
		/// Provides scheduling of frequently repeating events.
		/// </summary>
		public static class Every
		{
			/// <summary>
			/// Schedules blocks to run on fixed-rate updates.
			/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
			/// May run multiple times per frame and may not run in every frame.
			/// It's therefore unsuitable for once-only events, such as Input.
			/// </summary>
			/// <param name="blocks"></param>
			public static void FixedStep(params IBlock[] blocks) =>
				_instance.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnFixedStep);

			/// <summary>
			/// Schedules blocks to run on every-frame updates.
			/// </summary>
			/// <param name="blocks"></param>
			public static void Frame(params IBlock[] blocks) =>
				_instance.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnUpdate);

			/// <summary>
			/// Schedules blocks to run on every-frame updates but runs after OnUpdate.
			/// </summary>
			/// <param name="blocks"></param>
			public static void FrameEnd(params IBlock[] blocks) =>
				_instance.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnLateUpdate);
		}
	}
}
