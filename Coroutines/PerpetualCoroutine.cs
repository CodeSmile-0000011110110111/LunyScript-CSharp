using LunyScript.Blocks;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that never elapses automatically (runs until explicitly stopped).
	/// Base for all other coroutine types that utilize full lifecycle events.
	/// </summary>
	internal class PerpetualCoroutine : Coroutine
	{
		public PerpetualCoroutine(in Options options)
			: base(options)
		{
		}

		protected override void OnStart() {}
		protected override void OnStop() {}
		protected override Boolean OnFrameUpdate() => false;
		protected override Boolean OnHeartbeat() => false;
		public override String ToString() => $"{GetType().Name}({Name}, {State})";
	}
}
