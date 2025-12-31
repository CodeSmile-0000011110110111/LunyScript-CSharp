using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript
{
	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public interface ILunyScriptEngine
	{
		IScriptContext GetScriptContext(NativeID nativeID);
		IVariables GlobalVariables { get; }
	}

	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public sealed class LunyScriptEngine : ILunyScriptEngine
	{
		private ScriptRunner _runner;
		public static ILunyScriptEngine Instance { get; private set; }

		private LunyScriptEngine() {} // hide default ctor

		internal LunyScriptEngine(ScriptRunner scriptRunner)
		{
			if (Instance != null)
				throw new InvalidOperationException($"{nameof(ILunyScriptEngine)} singleton duplication!");
			if (scriptRunner == null)
				throw new ArgumentNullException(nameof(scriptRunner));

			_runner = scriptRunner;
			Instance = this;
		}

		internal void Shutdown() => Instance = null;

		public IScriptContext GetScriptContext(NativeID nativeID) => _runner.Contexts.GetByNativeID(nativeID);
		public IVariables GlobalVariables => ScriptContext.GetGlobalVariables();
	}
}
