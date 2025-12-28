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
		ScriptContext GetScriptContext(NativeID nativeID);
		IVariables GlobalVariables { get; }
	}

	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public sealed class LunyScriptEngine : ILunyScriptEngine
	{
		private LunyScriptRunner _runner;
		public static LunyScriptEngine Instance { get; private set; }

		private LunyScriptEngine() {} // hide default ctor

		internal LunyScriptEngine(LunyScriptRunner lunyScriptRunner)
		{
			if (Instance != null)
				throw new InvalidOperationException($"{nameof(LunyScriptEngine)} singleton duplication!");
			if (lunyScriptRunner == null)
				throw new ArgumentNullException(nameof(lunyScriptRunner));

			_runner = lunyScriptRunner;
			Instance = this;
		}

		internal void Shutdown() => Instance = null;

		public ScriptContext GetScriptContext(NativeID nativeID) => _runner.Contexts.GetByNativeID(nativeID);
		public IVariables GlobalVariables => ScriptContext.GetGlobalVariables();
	}
}
