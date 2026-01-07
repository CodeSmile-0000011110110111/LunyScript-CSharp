namespace LunyScript.Tests
{
	public sealed class Assert_Runs_WhenCreated : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Created(Run(AssertRanInFirstFrame));

			Every.FrameEnds(
				Debug.LogWarning("Reloading scene now ..."),
				Scene.Reload()
			);
			//Every.TimeInterval(TimeSpan.FromSeconds(1));
		}
	}

	public sealed class Assert_Runs_WhenDestroyed : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Created(Object.Destroy());
			When.Destroyed(Run(AssertRanInFirstFrame));
		}
	}

	public sealed class Assert_Runs_WhenEnabled : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Enabled(Run(AssertRanInFirstFrame));
		}
	}

	public sealed class Assert_Runs_WhenDisabled : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Created(Object.SetDisabled());
			When.Disabled(Run(AssertRanInFirstFrame));
		}
	}

	public sealed class Assert_Runs_WhenReady : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Ready(Run(AssertRanInFirstFrame));
		}
	}

	public sealed class Assert_Runs_EveryFixedStep : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			Every.FixedStep(
				Run(AssertRanInFirstFrame),
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
				Run(AssertRanInFirstFrame),
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
				Run(AssertRanInFirstFrame),
				Object.Destroy() // prevent log spam
			);
		}
	}
}
