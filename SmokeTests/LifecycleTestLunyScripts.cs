namespace LunyScript.SmokeTests
{
	public sealed class Assert_Runs_WhenCreated : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			When.Self.Created(Method.Run(AssertDidRun));

			When.Self.LateUpdates(
				//Debug.LogWarning("Reloading scene now ..."),
				//Scene.Reload()
			);
			//Every.TimeInterval(TimeSpan.FromSeconds(1));
		}
	}

	public sealed class Assert_Runs_WhenDestroyed : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			When.Self.Created(Object.Destroy());
			When.Self.Destroyed(Method.Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_WhenEnabled : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			When.Self.Enabled(Method.Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_WhenDisabled : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			When.Self.Created(Object.Disable());
			When.Self.Disabled(Method.Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_WhenReady : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			When.Self.Ready(Method.Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_EveryFixedStep : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			When.Self.Steps(Method.Run(AssertDidRun),
				Object.Destroy() // prevent log spam
			);
		}
	}

	public sealed class Assert_Runs_EveryFrame : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			When.Self.Updates(Method.Run(AssertDidRun),
				Object.Destroy() // prevent log spam
			);
		}
	}

	public sealed class Assert_Runs_EveryFrameEnds : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			When.Self.LateUpdates(Method.Run(AssertDidRun),
				Object.Destroy() // prevent log spam
			);
		}
	}
}
