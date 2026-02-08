using System;

namespace LunyScript.Coroutines
{
	public struct TimeProgress
	{
		public Double Current;
		public Double Duration;
		public Double TimeScale;
		public void Reset() => Current = 0.0;
		public void AddDeltaTime(Double dt) => Current += dt * TimeScale;
		public Boolean IsElapsed => Duration > 0.0 && Current >= Duration;
	}

	public struct CountProgress
	{
		public Int32 Current;
		public Int32 Target;
		public void Reset() => Current = 0;
		public void IncrementCount() => Current++;
		public Boolean IsElapsed => Target > 0 && Current >= Target;
	}
}
