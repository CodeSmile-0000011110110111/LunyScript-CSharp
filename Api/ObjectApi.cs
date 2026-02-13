using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Provides operations for objects.
	/// </summary>
	public readonly struct ObjectApi
	{
		private readonly IScript _script;
		internal ObjectApi(IScript script) => _script = script;

		public IScriptActionBlock Enable(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectEnableSelfBlock.Create() : ObjectEnableTargetBlock.Create(name);

		public IScriptActionBlock Disable(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectDisableSelfBlock.Create() : ObjectDisableTargetBlock.Create(name);

		public ObjectBuilder<StateNameSet> Create(String name)
		{
			var options = new ObjectCreateOptions { Name = name, Mode = ObjectCreationMode.Empty };
			var token = ((ILunyScriptInternal)_script).CreateToken(name, "ObjectCreate");
			return new ObjectBuilder<StateNameSet>(_script, options, token);
		}

		public IScriptActionBlock Destroy(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectDestroySelfBlock.Create() : ObjectDestroyTargetBlock.Create(name);
	}
}
