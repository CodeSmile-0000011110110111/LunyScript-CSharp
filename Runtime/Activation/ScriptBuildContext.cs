using System;

namespace LunyScript.Activation
{
	public struct ScriptBuildOptions
	{
		public Boolean Singleton { get; set; }
		public Boolean PatternMatching { get; set; }
	}

	public sealed class ScriptBuildSettings {}

	public sealed class ScriptBuildContext
	{
		public ScriptBuildOptions Options;
		public ScriptBuildSettings Settings { get; internal set; }
	}
}
