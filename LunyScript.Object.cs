using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Provides operations for objects.
		/// </summary>
		public static class Object
		{
			public static ILunyScriptBlock Enable(String name = null) => 
				String.IsNullOrEmpty(name) ? ObjectEnableSelfBlock.Create() : ObjectEnableTargetBlock.Create(name);

			public static ILunyScriptBlock Disable(String name = null) => 
				String.IsNullOrEmpty(name) ? ObjectDisableSelfBlock.Create() : ObjectDisableTargetBlock.Create(name);

			public static ILunyScriptBlock Clone(String originalName) => ObjectCreateCloneBlock.Create(originalName);
			public static ILunyScriptBlock Create(String name) => ObjectCreateEmptyBlock.Create(name);
			public static ILunyScriptBlock CreateCube(String name = null) => ObjectCreateCubeBlock.Create(name);
			public static ILunyScriptBlock CreateSphere(String name = null) => ObjectCreateSphereBlock.Create(name);
			public static ILunyScriptBlock CreateCapsule(String name = null) => ObjectCreateCapsuleBlock.Create(name);
			public static ILunyScriptBlock CreateCylinder(String name = null) => ObjectCreateCylinderBlock.Create(name);
			public static ILunyScriptBlock CreatePlane(String name = null) => ObjectCreatePlaneBlock.Create(name);
			public static ILunyScriptBlock CreateQuad(String name = null) => ObjectCreateQuadBlock.Create(name);

			public static ILunyScriptBlock Destroy(String name = null) => 
				String.IsNullOrEmpty(name) ? ObjectDestroySelfBlock.Create() : ObjectDestroyTargetBlock.Create(name);
		}
	}
}
