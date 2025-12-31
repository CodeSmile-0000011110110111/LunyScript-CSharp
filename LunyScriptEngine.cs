using Luny;
using Luny.Diagnostics;
using LunyScript.Execution;
using System;

namespace LunyScript
{
	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public interface ILunyScriptEngine
	{
		IVariables GlobalVariables { get; }
		IScriptContext GetScriptContext(NativeID nativeID);
	}

	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public sealed class LunyScriptEngine : ILunyScriptEngine
	{
		private LunyScriptRunner _runner;
		public static ILunyScriptEngine Instance { get; private set; }
		public IVariables GlobalVariables => ScriptContext.GetGlobalVariables();

		private LunyScriptEngine() {} // hide default ctor

		internal LunyScriptEngine(LunyScriptRunner scriptRunner)
		{
			if (Instance != null)
				throw new InvalidOperationException($"{nameof(ILunyScriptEngine)} singleton duplication!");
			if (scriptRunner == null)
				throw new ArgumentNullException(nameof(scriptRunner));

			_runner = scriptRunner;
			Instance = this;
			LunyLogger.LogInfo("Initialized.", this);
		}

		public IScriptContext GetScriptContext(NativeID nativeID) => _runner.Contexts.GetByNativeID(nativeID);
		~LunyScriptEngine() => LunyLogger.LogInfo($"finalized {GetHashCode()}", this);

		internal void Shutdown()
		{
			Instance = null;
			_runner = null;
			LunyLogger.LogInfo($"{nameof(Shutdown)}.", this);
		}
	}
}
