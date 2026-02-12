using Luny;
using Luny.Engine.Bridge;
using LunyScript.Activation;
using LunyScript.Api;
using LunyScript.Blocks;
using LunyScript.Coroutines.ApiBuilders;
using LunyScript.Events;
using System;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript
{
	public interface IScript
	{
		ScriptDefID ScriptDefId { get; }
		ILunyObject LunyObject { get; }
		//ITable GlobalVariables { get; }
		//ITable LocalVariables { get; }
		Boolean IsEditor { get; }

		DebugApi Debug { get; }
		EditorApi Editor { get; }
		EngineApi Engine { get; }
		MethodApi Method { get; }
		ObjectApi Object { get; }
		OnApi On { get; }
		PrefabApi Prefab { get; }
		SceneApi Scene { get; }
		WhenApi When { get; }

		VariableBlock Const(String name, Variable value);
		VariableBlock Const(Variable value);
		VariableBlock Var(String name);
		VariableBlock GVar(String name);
	}

	internal interface ILunyScriptInternal
	{
		ScriptEventScheduler Scheduler { get; }
		ScriptRuntimeContext RuntimeContext { get; }
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
	public abstract class Script : IScript, ILunyScriptInternal
	{
		private IScriptRuntimeContext _runtimeContext;

		/// <summary>
		/// ScriptID of the script for identification.
		/// </summary>
		public ScriptDefID ScriptDefId => _runtimeContext.ScriptDefId;
		/// <summary>
		/// Reference to proxy for engine object.
		/// Caution: native engine reference could be null.
		/// Check EngineObject.IsValid before accessing.
		/// </summary>
		[MaybeNull] public ILunyObject LunyObject => _runtimeContext.LunyObject;
		/// <summary>
		/// Global variables which all objects and scripts can read/write.
		/// </summary>
		//[NotNull] public ITable GlobalVariables => _context.GlobalVariables;
		/// <summary>
		/// Local variables the current object and script owns.
		/// If multiple objects run the same script, each object has its own unique set of local variables.
		/// </summary>
		//[NotNull] public ITable LocalVariables => _context.LocalVariables;
		/// <summary>
		/// True if the script runs within the engine's editor (play mode). False in builds.
		/// </summary>
		public Boolean IsEditor => LunyEngine.Instance.Application.IsEditor;

		ScriptEventScheduler ILunyScriptInternal.Scheduler => _runtimeContext is ScriptRuntimeContext context ? context.Scheduler : null;
		ScriptRuntimeContext ILunyScriptInternal.RuntimeContext => _runtimeContext as ScriptRuntimeContext;

		// implemented APIs
		public DebugApi Debug => new(this);
		public EditorApi Editor => new(this);
		public EngineApi Engine => new(this);
		public LoopApi Loop => new(this);
		public MethodApi Method => new(this);
		public ObjectApi Object => new(this);
		public OnApi On => new(this);
		public PrefabApi Prefab => new(this);
		public SceneApi Scene => new(this);
		public WhenApi When => new(this);

		// these API outlines exist to get a feel for the intellisense/autocompletion behaviour ...

		// planned API outline
		public ApiPlaceholders.AnimationApi Animation => new(this);
		public ApiPlaceholders.ApplicationApi Application => new(this);
		public ApiPlaceholders.AssetApi Asset => new(this);
		public ApiPlaceholders.AudioApi Audio => new(this);
		public ApiPlaceholders.CameraApi Camera => new(this);
		public ApiPlaceholders.DiagnosticsApi Diagnostics => new(this);
		public ApiPlaceholders.HUDApi HUD => new(this);
		public ApiPlaceholders.InputApi Input => new(this);
		public ApiPlaceholders.MenuApi Menu => new(this);
		public ApiPlaceholders.PhysicsApi Physics => new(this);
		public ApiPlaceholders.PlayerApi Player => new(this);
		public ApiPlaceholders.StorageApi Storage => new(this);
		public TimeApi Time => new(this);

		// possible future expansions
		public ApiPlaceholders.AccessibilityApi Accessibility => new(this);
		public ApiPlaceholders.AccountApi Account => new(this);
		public ApiPlaceholders.AIApi AI => new(this);
		public ApiPlaceholders.AsyncApi Async => new(this);
		public ApiPlaceholders.AvatarApi Avatar => new(this);
		public ApiPlaceholders.CloudApi Cloud => new(this);
		public ApiPlaceholders.CutsceneApi Cutscene => new(this);
		public ApiPlaceholders.EnvironmentApi Environment => new(this);
		public ApiPlaceholders.GraphicsApi Graphics => new(this);
		public ApiPlaceholders.L18nApi L18n => new(this);
		public ApiPlaceholders.LocaleApi Locale => new(this);
		public ApiPlaceholders.LocalizationApi Localization => new(this);
		public ApiPlaceholders.NavigationApi Navigation => new(this);
		public ApiPlaceholders.NetworkApi Network => new(this);
		public ApiPlaceholders.NPCApi NPC => new(this);
		public ApiPlaceholders.ParticlesApi Particles => new(this);
		public ApiPlaceholders.PlatformApi Platform => new(this);
		public ApiPlaceholders.PoolApi Pool => new(this);
		public ApiPlaceholders.PostFxApi PostFx => new(this);
		public ApiPlaceholders.ProgressApi Progress => new(this);
		public ApiPlaceholders.QualityApi Quality => new(this);
		//public ApiPlaceholders.ScriptApi Script => new(this);
		public ApiPlaceholders.SessionApi Session => new(this);
		public ApiPlaceholders.SettingsApi Settings => new(this);
		public ApiPlaceholders.SpawnApi Spawn => new(this);
		public ApiPlaceholders.SpriteApi Sprite => new(this);
		public ApiPlaceholders.StageApi Stage => new(this);
		public ApiPlaceholders.StoreApi Store => new(this);
		public ApiPlaceholders.TerrainApi Terrain => new(this);
		public ApiPlaceholders.TilemapApi Tilemap => new(this);
		public ApiPlaceholders.TutorialApi Tutorial => new(this);
		public ApiPlaceholders.UIApi UI => new(this);
		public ApiPlaceholders.VFXApi VFX => new(this);
		public ApiPlaceholders.VideoApi Video => new(this);

		internal void Initialize(IScriptRuntimeContext runtimeContext) =>
			_runtimeContext = runtimeContext ?? throw new ArgumentNullException(nameof(runtimeContext));

		// Variables and Constants
		public VariableBlock Const(String name, Variable value) =>
			TableVariableBlock.Create(_runtimeContext.GlobalVariables.DefineConstant(name, value));

		public VariableBlock Const(Variable value) => ConstantVariableBlock.Create(value);
		public VariableBlock GVar(String name) => TableVariableBlock.Create(_runtimeContext.GlobalVariables.GetHandle(name));
		public VariableBlock Var(String name) => TableVariableBlock.Create(_runtimeContext.LocalVariables.GetHandle(name));

		// Logic Flow API

		/// <summary>
		/// Conditional execution: If(conditions).Then(blocks).ElseIf(conditions).Then(blocks).Else(blocks);
		/// Multiple conditions are implicitly AND combined.
		/// </summary>
		public IfBlockBuilder If(params IScriptConditionBlock[] conditions) => new(conditions);

		/// <summary>
		/// Loop execution: While(conditions).Do(blocks);
		/// Multiple conditions are implicitly AND combined.
		/// </summary>
		public WhileBlockBuilder While(params IScriptConditionBlock[] conditions) => new(conditions);

		/// <summary>
		/// For loop (1-based index): For(limit).Do(blocks);
		/// Starts at 1 and increments by 1 until limit is reached (inclusive).
		/// </summary>
		public ForBlockBuilder For(Int32 limit) => new(limit);

		/// <summary>
		/// For loop (1-based index): For(limit, step).Do(blocks);
		/// If step > 0: starts at 1 and increments by step until limit is reached.
		/// If step < 0: starts at limit and decrements by step until 1 is reached.
		/// </summary>
		public ForBlockBuilder For(Int32 limit, Int32 step) => new(limit, step);

		// Boolean Modifiers for Conditions

		/// <summary>
		/// Logical AND: Returns true if all conditions are true.
		/// </summary>
		public IScriptConditionBlock AND(params IScriptConditionBlock[] conditions) => AndBlock.Create(conditions);

		/// <summary>
		/// Logical OR: Returns true if at least one condition is true.
		/// </summary>
		public IScriptConditionBlock OR(params IScriptConditionBlock[] conditions) => OrBlock.Create(conditions);

		/// <summary>
		/// Logical NOT: Returns the inverse of the condition.
		/// </summary>
		public IScriptConditionBlock NOT(IScriptConditionBlock condition) => NotBlock.Create(condition);

		// Coroutines & Timers

		/// <summary>
		/// Creates a named timer.
		/// Usage: Timer("name").In(3).Seconds().Do(blocks);
		/// </summary>
		protected TimerBuilder Timer(String name) => new(this, name);

		/// <summary>
		/// Creates a named counter.
		/// Usage: Counter("name").In(5).Frames().Do(blocks);
		/// </summary>
		protected CounterBuilder Counter(String name) => new(this, name);

		/// <summary>
		/// Creates a named coroutine.
		/// Usage: Coroutine("name").Duration(3).Seconds().OnUpdate(blocks).Elapsed(blocks);
		/// </summary>
		protected CoroutineBuilder Coroutine(String name) => new(this, name);

		/// <summary>
		/// Time-sliced execution: Every(n).Frames().Do(blocks) or Every(n).Heartbeats().Do(blocks).
		/// Supports optional phase offset: Every(n).Frames().DelayBy(offset).Do(blocks).
		/// Use Even or Odd constants for alternating execution.
		/// </summary>
		protected EveryBuilder Every(Int32 interval) => new(this, interval);

		~Script() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Destroy() {} // placeholder for future cleanup tasks

		/// <summary>
		/// Called once when the script is initialized.
		/// Users construct their blocks (sequences, statemachines, behaviors) for execution here.
		/// Users can use regular C# syntax (ie call methods, use loops) to construct complex and/or reusable blocks.
		/// </summary>
		/// <param name="context"></param>
		public abstract void Build(ScriptContext context);

		override public String ToString() => _runtimeContext != null ? _runtimeContext.ToString() : GetType().FullName;
	}
}
