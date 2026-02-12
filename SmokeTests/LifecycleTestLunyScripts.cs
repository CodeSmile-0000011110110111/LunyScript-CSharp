using LunyScript.Activation;

namespace LunyScript.SmokeTests
{
	public sealed class Assert_Runs_WhenCreated : LunyScriptSmokeTestBase
	{
		public override void Build(ScriptContext context)
		{
			On.Created(Method.Run(AssertDidRun));

			On.FrameLateUpdate(
				//Debug.LogWarning("Reloading scene now ..."),
				//Scene.Reload()
			);
			//Every.TimeInterval(TimeSpan.FromSeconds(1));
		}
	}

	public sealed class Assert_Runs_WhenDestroyed : LunyScriptSmokeTestBase
	{
		public override void Build(ScriptContext context)
		{
			On.Created(Object.Destroy());
			On.Destroyed(Method.Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_WhenEnabled : LunyScriptSmokeTestBase
	{
		public override void Build(ScriptContext context) => On.Enabled(Method.Run(AssertDidRun));
	}

	public sealed class Assert_Runs_WhenDisabled : LunyScriptSmokeTestBase
	{
		public override void Build(ScriptContext context)
		{
			On.Created(Object.Disable());
			On.Disabled(Method.Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_WhenReady : LunyScriptSmokeTestBase
	{
		public override void Build(ScriptContext context) => On.Ready(Method.Run(AssertDidRun));
	}

	public sealed class Assert_Runs_EveryFixedStep : LunyScriptSmokeTestBase
	{
		public override void Build(ScriptContext context) => On.Heartbeat(Method.Run(AssertDidRun),
			Object.Destroy() // prevent log spam
		);
	}

	public sealed class Assert_Runs_EveryFrame : LunyScriptSmokeTestBase
	{
		public override void Build(ScriptContext context) => On.FrameUpdate(Method.Run(AssertDidRun),
			Object.Destroy() // prevent log spam
		);
	}

	public sealed class Assert_Runs_EveryFrameEnds : LunyScriptSmokeTestBase
	{
		public override void Build(ScriptContext context) => On.FrameLateUpdate(Method.Run(AssertDidRun),
			Object.Destroy() // prevent log spam
		);
	}
}
