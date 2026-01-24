using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		public static class Prefab
		{
			public static ILunyScriptBlock Instantiate(String prefabName) => ObjectCreatePrefabBlock.Create(prefabName);
		}
	}
}
