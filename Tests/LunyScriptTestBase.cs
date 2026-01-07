using Luny;
using System;

namespace LunyScript.Tests
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
			When.Object.Created(Run(() => LunyLogger.LogInfo($"{GetType().Name} CREATED...", this)));
			When.Object.Destroyed(Run(() => LunyLogger.LogInfo($"{GetType().Name} DESTROYED...", this)));
			When.Object.Ready(Run(() => LunyLogger.LogInfo($"{GetType().Name} READY...", this)));
			When.Object.Enabled(Run(() => LunyLogger.LogInfo($"{GetType().Name} ENABLED...", this)));
			When.Object.Disabled(Run(() => LunyLogger.LogInfo($"{GetType().Name} DISABLED...", this)));
			When.Scene.Unloads(Run(() => LunyLogger.LogInfo($"{GetType().Name} SCENE UNLOADS...", this)));
			When.Scene.Loads(Run(() => LunyLogger.LogInfo($"{GetType().Name} SCENE LOADS...", this)));
		}
	}
}
