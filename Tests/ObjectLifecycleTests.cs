namespace LunyScript.Tests
{
	public sealed class Assert_Runs_WhenCreated : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Created(Run(AssertRanInFirstFrame));
			//When.EveryFrameEnds(Object.Destroy());
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
			//When.EveryFrameEnds(Object.Destroy());
		}
	}

	public sealed class Assert_Runs_WhenDisabled : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Created(Object.SetDisabled());
			When.Disabled(Run(AssertRanInFirstFrame));
			//When.EveryFrameEnds(Object.Destroy());
		}
	}

	public sealed class Assert_Runs_WhenReady : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.Ready(Run(AssertRanInFirstFrame));
			//When.EveryFrameEnds(Object.Destroy());
		}
	}

	public sealed class Assert_Runs_EveryFixedStep : LunyScriptTestBase
	{
		public override void Build()
		{
			base.Build();
			When.EveryFixedStep(
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
			When.EveryFrame(
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
			When.EveryFrameEnds(
				Run(AssertRanInFirstFrame),
				Object.Destroy() // prevent log spam
			);
		}
	}
}
