using Luny;
using Luny.Engine.Identity;
using LunyScript.Execution;
using System;

namespace LunyScript
{
	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public interface ILunyScriptEngine
	{
		ILunyScriptVariables GlobalVariables { get; }
		ILunyScriptContext GetScriptContext(LunyNativeObjectID lunyNativeObjectID);
	}

	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public sealed class LunyScriptEngine : ILunyScriptEngine
	{
		private LunyScriptRunner _runner;
		public static ILunyScriptEngine Instance { get; private set; }
		public ILunyScriptVariables GlobalVariables => LunyScriptContext.GetGlobalVariables();

		private LunyScriptEngine() {} // hide default ctor

		internal LunyScriptEngine(LunyScriptRunner scriptRunner)
		{
			LunyTraceLogger.LogInfoCreateSingletonInstance(typeof(LunyScriptEngine));

			if (Instance != null)
				throw new InvalidOperationException($"{nameof(ILunyScriptEngine)} singleton duplication!");
			if (scriptRunner == null)
				throw new ArgumentNullException(nameof(scriptRunner));

			_runner = scriptRunner;
			Instance = this;
		}

		public ILunyScriptContext GetScriptContext(LunyNativeObjectID lunyNativeObjectID) =>
			_runner.Contexts.GetByNativeObjectID(lunyNativeObjectID);

		~LunyScriptEngine() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Shutdown()
		{
			LunyTraceLogger.LogInfoShuttingDown(this);
			Instance = null;
			_runner = null;
			LunyTraceLogger.LogInfoShutdownComplete(this);
		}
	}
}
