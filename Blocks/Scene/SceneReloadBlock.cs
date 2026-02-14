using Luny;

namespace LunyScript.Blocks
{
	internal sealed class SceneReloadBlock : ScriptActionBlock
	{
		public static ScriptActionBlock Create() => new SceneReloadBlock();

		private SceneReloadBlock() {}

		public override void Execute(IScriptRuntimeContext runtimeContext) => LunyEngine.Instance.Scene.ReloadScene();
	}
}
