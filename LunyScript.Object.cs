using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Provides operations for objects.
		/// </summary>
		public readonly struct ObjectApi
		{
			private readonly ILunyScript _script;
			internal ObjectApi(ILunyScript script) => _script = script;

			public ILunyScriptBlock Enable(String name = null) =>
				String.IsNullOrEmpty(name) ? ObjectEnableSelfBlock.Create() : ObjectEnableTargetBlock.Create(name);

			public ILunyScriptBlock Disable(String name = null) =>
				String.IsNullOrEmpty(name) ? ObjectDisableSelfBlock.Create() : ObjectDisableTargetBlock.Create(name);

			public ILunyScriptBlock Clone(String originalName) => ObjectCreateCloneBlock.Create(originalName);
			public ILunyScriptBlock Create(String name) => ObjectCreateEmptyBlock.Create(name);
			public ILunyScriptBlock CreateCube(String name = null) => ObjectCreateCubeBlock.Create(name);
			public ILunyScriptBlock CreateSphere(String name = null) => ObjectCreateSphereBlock.Create(name);
			public ILunyScriptBlock CreateCapsule(String name = null) => ObjectCreateCapsuleBlock.Create(name);
			public ILunyScriptBlock CreateCylinder(String name = null) => ObjectCreateCylinderBlock.Create(name);
			public ILunyScriptBlock CreatePlane(String name = null) => ObjectCreatePlaneBlock.Create(name);
			public ILunyScriptBlock CreateQuad(String name = null) => ObjectCreateQuadBlock.Create(name);

			public ILunyScriptBlock Destroy(String name = null) =>
				String.IsNullOrEmpty(name) ? ObjectDestroySelfBlock.Create() : ObjectDestroyTargetBlock.Create(name);
		}
	}
}
