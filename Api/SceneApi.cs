using LunyScript.Blocks;

namespace LunyScript
{
	/// <summary>
	/// Provides operations for managing Scenes and accessing the scene hierarchy.
	/// </summary>
	public readonly struct SceneApi
	{
		private readonly ILunyScript _script;
		internal SceneApi(ILunyScript script) => _script = script;

		public IScriptActionBlock Reload() => SceneReloadBlock.Create();
	}
}
