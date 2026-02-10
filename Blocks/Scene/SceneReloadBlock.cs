using Luny;

namespace LunyScript.Blocks
{
	internal sealed class SceneReloadBlock : IScriptActionBlock
	{
		public static IScriptActionBlock Create() => new SceneReloadBlock();

		private SceneReloadBlock() {}

		public void Execute(IScriptRuntimeContext runtimeContext) => LunyEngine.Instance.Scene.ReloadScene();
	}
}
