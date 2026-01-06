using LunyScript.Blocks;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Provides operations for managing Scenes and accessing the scene hierarchy.
		/// </summary>
		public static class Scene
		{
			public static ILunyScriptBlock Reload() => SceneReloadBlock.Create();
		}
	}
}
