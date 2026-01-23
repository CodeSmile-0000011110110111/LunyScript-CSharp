using Luny;
using System;

namespace LunyScript.SmokeTests
{
	public abstract class LunyScriptTestBase : LunyScript
	{
		protected void AssertDidRun() => SetTestPassedVariable(true);

		private void SetTestPassedVariable(Boolean result)
		{
			var name = GetType().Name;
			if (!GlobalVariables[name].Boolean())
				GlobalVariables[name] = result;
		}

		public override void Build()
		{
			LunyLogger.LogInfo($"{GetType().Name} BUILD", this);
			When.Self.Created(Run(() => LunyLogger.LogInfo($"{GetType().Name} CREATED...", this)));
			When.Self.Destroyed(Run(() => LunyLogger.LogInfo($"{GetType().Name} DESTROYED...", this)));
			When.Self.Ready(Run(() => LunyLogger.LogInfo($"{GetType().Name} READY...", this)));
			When.Self.Enabled(Run(() => LunyLogger.LogInfo($"{GetType().Name} ENABLED...", this)));
			When.Self.Disabled(Run(() => LunyLogger.LogInfo($"{GetType().Name} DISABLED...", this)));
			When.Scene.Unloads(Run(() => LunyLogger.LogInfo($"{GetType().Name} SCENE UNLOADS...", this)));
			When.Scene.Loads(Run(() => LunyLogger.LogInfo($"{GetType().Name} SCENE LOADS...", this)));
		}
	}
}
