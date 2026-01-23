namespace LunyScript.SmokeTests
{
	public sealed class Assert_Runs_WhenCreated : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Object.Created(Run(AssertDidRun));

			Every.FrameEnds(
				//Debug.LogWarning("Reloading scene now ..."),
				//Scene.Reload()
			);
			//Every.TimeInterval(TimeSpan.FromSeconds(1));
		}
	}

	public sealed class Assert_Runs_WhenDestroyed : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Object.Created(Object.Destroy());
			When.Object.Destroyed(Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_WhenEnabled : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Object.Enabled(Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_WhenDisabled : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Object.Created(Object.SetDisabled());
			When.Object.Disabled(Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_WhenReady : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Object.Ready(Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_EveryFixedStep : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			Every.FixedStep(
				Run(AssertDidRun),
				Object.Destroy() // prevent log spam
			);
		}
	}

	public sealed class Assert_Runs_EveryFrame : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			Every.Frame(
				Run(AssertDidRun),
				Object.Destroy() // prevent log spam
			);
		}
	}

	public sealed class Assert_Runs_EveryFrameEnds : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			Every.FrameEnds(
				Run(AssertDidRun),
				Object.Destroy() // prevent log spam
			);
		}
	}
}
