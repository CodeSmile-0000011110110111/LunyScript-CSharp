using Luny;
using LunyScript.Execution;

namespace LunyScript.Blocks
{
	internal sealed class SceneReloadBlock : ILunyScriptBlock
	{
		public static ILunyScriptBlock Create() => new SceneReloadBlock();

		private SceneReloadBlock() {}

		public void Execute(ILunyScriptContext context) => LunyEngineInternal.Instance.Scene.ReloadScene();
	}
}
