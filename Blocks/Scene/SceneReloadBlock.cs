using Luny;
using LunyScript.Execution;

namespace LunyScript.Blocks
{
	internal sealed class SceneReloadBlock : IScriptActionBlock
	{
		public static IScriptActionBlock Create() => new SceneReloadBlock();

		private SceneReloadBlock() {}

		public void Execute(ILunyScriptContext context) => LunyEngine.Instance.Scene.ReloadScene();
	}
}
