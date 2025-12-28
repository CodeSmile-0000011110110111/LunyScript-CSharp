using LunyScript.Blocks;
using LunyScript.Interfaces;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		// User-facing API: Block factory methods
		protected IBlock Do(Action action) => new ActionBlock(_ => action());
		protected IBlock Is(Func<Boolean> condition) => new ActionBlock(_ => condition());
	}
}
