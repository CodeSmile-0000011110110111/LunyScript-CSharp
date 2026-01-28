using Luny;
using System;

namespace LunyScript.SmokeTests
{
	public abstract class LunyScriptSmokeTestBase : LunyScript
	{
		protected void AssertDidRun() => SetTestPassedVariable(true);

		private void SetTestPassedVariable(Boolean result)
		{
			var name = GetType().Name;
			if (!GlobalVars[name].AsBoolean())
				GlobalVars[name] = LunyVariable.Create(result);
		}

		public override void Build()
		{
			LunyLogger.LogInfo($"{GetType().Name} BUILD", this);
			When.Self.Created(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} CREATED...", this)));
			When.Self.Destroyed(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} DESTROYED...", this)));
			When.Self.Ready(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} READY...", this)));
			When.Self.Enabled(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} ENABLED...", this)));
			When.Self.Disabled(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} DISABLED...", this)));
			When.Scene.Unloads(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} SCENE UNLOADS...", this)));
			When.Scene.Loads(Method.Run(() => LunyLogger.LogInfo($"{GetType().Name} SCENE LOADS...", this)));
		}
	}
}
