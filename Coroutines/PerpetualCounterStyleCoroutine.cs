using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Marker subclass of PerpetualCoroutine to identify time-sliced, non-elapsing coroutines
	/// that should use counter-style blocks in the scripting API.
	/// </summary>
	internal sealed class PerpetualCounterStyleCoroutine : PerpetualCoroutine
	{
		internal override Boolean IsCounterStyle => true;

		public PerpetualCounterStyleCoroutine(in Options options)
			: base(options)
		{
		}
	}
}
