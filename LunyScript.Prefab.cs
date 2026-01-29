using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		public readonly struct PrefabApi
		{
			private readonly ILunyScript _script;
			internal PrefabApi(ILunyScript script) => _script = script;

			public IScriptActionBlock Instantiate(String prefabName) => ObjectCreatePrefabBlock.Create(prefabName);
		}
	}
}
