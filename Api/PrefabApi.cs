using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	public readonly struct PrefabApi
	{
		private readonly IScript _script;
		internal PrefabApi(IScript script) => _script = script;

		public IScriptActionBlock Instantiate(String prefabName) => ObjectCreatePrefabBlock.Create(prefabName);
	}
}
