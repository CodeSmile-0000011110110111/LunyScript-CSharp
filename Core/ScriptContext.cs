using System;

namespace LunyScript
{
	public struct ScriptOptions
	{
		public Boolean Singleton { get; set; }
		public Boolean PatternMatching { get; set; }
	}

	public sealed class ScriptSettings {}

	public sealed class ScriptContext
	{
		public ScriptOptions Options;
		public ScriptSettings Settings { get; internal set; }
	}
}
