using Luny;
using Luny.Engine.Bridge.Identity;
using LunyScript.Runners;
using System;

namespace LunyScript
{
	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public interface ILunyScriptEngine
	{
		ITable GlobalVariables { get; }
		IScriptRuntimeContext GetScriptContext(LunyNativeObjectID lunyNativeObjectID);
	}

	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public sealed class LunyScriptEngine : ILunyScriptEngine
	{
		private LunyScriptBlockRunner _blockRunner;
		public static ILunyScriptEngine Instance { get; private set; }
		public ITable GlobalVariables => ScriptRuntimeContext.GetGlobalVariables();

		/// <summary>
		/// Maximum allowed iterations for While/For loops to prevent engine hangs.
		/// Only active in DEBUG or UNITY_EDITOR builds.
		/// </summary>
		public static Int32 MaxLoopIterations => 1000000;

		internal static void ForceReset_UnitTestsOnly()
		{
			Instance = null;
			ScriptRuntimeContext.ClearGlobalVariables();
		}

		private LunyScriptEngine() {} // hide default ctor

		internal LunyScriptEngine(LunyScriptBlockRunner scriptBlockRunner)
		{
			LunyTraceLogger.LogInfoCreateSingletonInstance(typeof(LunyScriptEngine));

			if (Instance != null)
				throw new InvalidOperationException($"{nameof(ILunyScriptEngine)} singleton duplication!");
			if (scriptBlockRunner == null)
				throw new ArgumentNullException(nameof(scriptBlockRunner));

			_blockRunner = scriptBlockRunner;
			Instance = this;
		}

		public IScriptRuntimeContext GetScriptContext(LunyNativeObjectID lunyNativeObjectID) =>
			_blockRunner.Contexts.GetByNativeObjectID(lunyNativeObjectID);

		~LunyScriptEngine() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Shutdown()
		{
			LunyTraceLogger.LogInfoShuttingDown(this);
			Instance = null;
			_blockRunner = null;
			LunyTraceLogger.LogInfoShutdownComplete(this);
		}
	}
}
