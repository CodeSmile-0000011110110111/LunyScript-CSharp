using Luny;
using Luny.Engine.Bridge;
using LunyScript.Events;
using LunyScript.Execution;
using System;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript
{
	public interface ILunyScript
	{
		LunyScriptID ScriptID { get; }
		ILunyObject LunyObject { get; }
		ITable GlobalVars { get; }
		ITable LocalVars { get; }
		Boolean IsEditor { get; }
	}

	internal interface ILunyScriptInternal
	{
		LunyScriptEventScheduler Scheduler { get; }
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
	public abstract partial class LunyScript : ILunyScript, ILunyScriptInternal
	{
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
		[NotNull] public ITable GlobalVars => _context.GlobalVariables;
		/// <summary>
		/// Short alias for 'GlobalVariables'.
		/// </summary>
		[NotNull] public ITable GVars => _context.GlobalVariables;
		/// <summary>
		/// Local variables the current object and script owns.
		/// If multiple objects run the same script, each object has its own unique set of local variables.
		/// </summary>
		[NotNull] public ITable LocalVars => _context.LocalVariables;
		/// <summary>
		/// Short alias for 'LocalVariables'.
		/// </summary>
		[NotNull] public ITable LVars => _context.LocalVariables;
		/// <summary>
		/// True if the script runs within the engine's editor (play mode). False in builds.
		/// </summary>
		public Boolean IsEditor => LunyEngine.Instance.Application.IsEditor;

		LunyScriptEventScheduler ILunyScriptInternal.Scheduler => _context is LunyScriptContext context ? context.Scheduler : null;

		// API properties
		public DebugApi Debug => new(this);
		public EditorApi Editor => new(this);
		public EngineApi Engine => new(this);
		public MethodApi Method => new(this);
		public ObjectApi Object => new(this);
		public PrefabApi Prefab => new(this);
		public SceneApi Scene => new(this);
		public WhenApi When => new(this);

		internal void Initialize(ILunyScriptContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

		~LunyScript() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Destroy()
		{
			// cleanup if necessary
		}

		/// <summary>
		/// Called once when the script is initialized.
		/// Users construct their blocks (sequences, statemachines, behaviors) for execution here.
		/// Users can use regular C# syntax (ie call methods, use loops) to construct complex and/or reusable blocks.
		/// </summary>
		public abstract void Build();
	}
}
