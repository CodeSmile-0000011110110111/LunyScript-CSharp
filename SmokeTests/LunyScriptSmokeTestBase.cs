using Luny;
using LunyScript.Activation;
using System;

namespace LunyScript.SmokeTests
{
	public abstract class LunyScriptSmokeTestBase : Script
	{
		protected void AssertDidRun() => SetTestPassedVariable(true);

		private void SetTestPassedVariable(Boolean result)
		{
			var gvars = ScriptEngine.Instance.GlobalVariables;
			var name = GetType().Name;

			LunyLogger.LogInfo($"{nameof(LunyScriptSmokeTestBase)}.SetTestPassedVariable() => " +
			                   $"{name} = {result.ToString().ToUpper()}", this);

			if (!gvars[name].AsBoolean())
				gvars[name] = result;
		}

		public override void Build(ScriptContext context)
		{
			LunyLogger.LogInfo($"{GetType().Name} BUILD", this);
			On.Created(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} CREATED...", this)));
			On.Destroyed(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} DESTROYED...", this)));
			On.Ready(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} READY...", this)));
			On.Enabled(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} ENABLED...", this)));
			On.Disabled(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} DISABLED...", this)));
			When.Scene.Unloads(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} SCENE UNLOADS...", this)));
			When.Scene.Loads(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} SCENE LOADS...", this)));
		}
	}
}
