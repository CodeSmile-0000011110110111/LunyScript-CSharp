using LunyScript.Blocks;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Provides operations for objects.
		/// </summary>
		public static class Object
		{
			public static ILunyScriptBlock SetEnabled(String name = null) => ObjectSetEnabledBlock.Create(name);
			public static ILunyScriptBlock SetDisabled(String name = null) => ObjectSetDisabledBlock.Create(name);

			public static ILunyScriptBlock CreateEmpty(String name) => ObjectCreateBlock.CreateEmpty(name);
			public static ILunyScriptBlock CreateWithPrefab(String prefabName) => ObjectCreateBlock.CreateWithPrefab(prefabName);
			public static ILunyScriptBlock CreateClone(String originalName) => ObjectCreateBlock.CreateClone(originalName);
			public static ILunyScriptBlock CreateCube(String name = null) => ObjectCreateBlock.CreateCube(name);
			public static ILunyScriptBlock CreateSphere(String name = null) => ObjectCreateBlock.CreateSphere(name);
			public static ILunyScriptBlock CreateCapsule(String name = null) => ObjectCreateBlock.CreateCapsule(name);
			public static ILunyScriptBlock CreateCylinder(String name = null) => ObjectCreateBlock.CreateCylinder(name);
			public static ILunyScriptBlock CreatePlane(String name = null) => ObjectCreateBlock.CreatePlane(name);
			public static ILunyScriptBlock CreateQuad(String name = null) => ObjectCreateBlock.CreateQuad(name);

			public static ILunyScriptBlock Destroy(String name = null) => ObjectDestroyBlock.Create(name);
			public static ILunyScriptBlock Test(String name = null, [CallerMemberName] string callerMemberName = null)
				=> ObjectDestroyBlock.Create(name);
		}
	}
}
