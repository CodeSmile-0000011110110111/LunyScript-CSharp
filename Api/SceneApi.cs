using LunyScript.Blocks;

namespace LunyScript.Api
{
	/// <summary>
	/// Provides operations for managing Scenes and accessing the scene hierarchy.
	/// </summary>
	public readonly struct SceneApi
	{
		private readonly IScript _script;
		internal SceneApi(IScript script) => _script = script;

		public IScriptActionBlock Reload() => SceneReloadBlock.Create();
	}
}
