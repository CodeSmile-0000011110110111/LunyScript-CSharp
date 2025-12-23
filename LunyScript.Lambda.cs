using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		// User-facing API: Block factory methods
		protected ActionBlock Do(Action action) => new(_ => action());
		protected ActionBlock Is(Func<Boolean> condition) => new(_ => condition());
	}
}
