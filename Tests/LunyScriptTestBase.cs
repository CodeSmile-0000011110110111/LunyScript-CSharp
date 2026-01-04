using Luny;
using System;

namespace LunyScript.Tests
{
	public abstract class LunyScriptTestBase : LunyScript
	{
		private Int64 _firstFrame;
		private Boolean _loggedOnce_NotInFirstFrame;

		protected void AssertRanInFirstFrame()
		{
			var currentFrame = LunyEngine.Instance.Time.FrameCount;
			if (currentFrame > _firstFrame)
			{
				if (!_loggedOnce_NotInFirstFrame)
				{
					_loggedOnce_NotInFirstFrame = true;
					LunyLogger.LogError($"{GetType().Name} expected to run in first frame! Got: {currentFrame}, expected: {_firstFrame}");
				}

				SetTestPassedVariable(false);
			}
			else
				SetTestPassedVariable(true);
		}

		private void SetTestPassedVariable(Boolean result)
		{
			var name = GetType().Name;
			if (!GlobalVariables[name].Boolean())
				GlobalVariables[name] = result;
		}

		public override void Build() =>
			// When.Created(Debug.LogInfo("RUNNING: When.Created"));
			// When.Destroyed(Debug.LogInfo("RUNNING: When.Destroyed"));
			// When.Enabled(Debug.LogInfo("RUNNING: When.Enabled"));
			// When.Disabled(Debug.LogInfo("RUNNING: When.Disabled"));
			When.Created(Run(() => _firstFrame = LunyEngine.Instance.Time.FrameCount));
	}
}
