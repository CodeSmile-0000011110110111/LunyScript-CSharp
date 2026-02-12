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

		public IScriptActionBlock Clone(String originalName) => ObjectCreateCloneBlock.Create(originalName);
		public IScriptActionBlock Create(String name) => ObjectCreateEmptyBlock.Create(name);
		public IScriptActionBlock CreateCube(String name = null) => ObjectCreateCubeBlock.Create(name);
		public IScriptActionBlock CreateSphere(String name = null) => ObjectCreateSphereBlock.Create(name);
		public IScriptActionBlock CreateCapsule(String name = null) => ObjectCreateCapsuleBlock.Create(name);
		public IScriptActionBlock CreateCylinder(String name = null) => ObjectCreateCylinderBlock.Create(name);
		public IScriptActionBlock CreatePlane(String name = null) => ObjectCreatePlaneBlock.Create(name);
		public IScriptActionBlock CreateQuad(String name = null) => ObjectCreateQuadBlock.Create(name);

		public IScriptActionBlock Destroy(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectDestroySelfBlock.Create() : ObjectDestroyTargetBlock.Create(name);
	}
}
