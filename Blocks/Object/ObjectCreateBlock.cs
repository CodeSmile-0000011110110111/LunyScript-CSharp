using Luny.Engine.Bridge.Enums;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	public readonly struct CreateObject
	{
		public enum Type
		{
			Empty,
			Clone,
			Prefab,
			Primitive,
		}

		internal readonly String Name;
		internal readonly Type CreateType;
		internal readonly PrimitiveType PrimitiveType;

		public CreateObject(String name, Type type, PrimitiveType primitiveType = PrimitiveType.Empty)
		{
			Name = name;
			CreateType = type;
			PrimitiveType = primitiveType;
		}

		// behaviour
		// position
		// rotation
		// scale
		// parent (by name?)
	}

	/// <summary>
	/// Creates an instance of an engine object.
	/// </summary>
	internal sealed class ObjectCreateBlock : ILunyScriptBlock
	{
		private CreateObject _data;

		internal static ILunyScriptBlock CreateEmpty(String name) => new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Empty));

		internal static ILunyScriptBlock CreateWithPrefab(String prefabName) =>
			new ObjectCreateBlock(new CreateObject(prefabName, CreateObject.Type.Prefab));

		internal static ILunyScriptBlock CreateClone(String originalName) =>
			new ObjectCreateBlock(new CreateObject(originalName, CreateObject.Type.Clone));

		internal static ILunyScriptBlock CreateCube(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Cube));

		internal static ILunyScriptBlock CreateSphere(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Sphere));

		internal static ILunyScriptBlock CreateCapsule(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Capsule));

		internal static ILunyScriptBlock CreateCylinder(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Cylinder));

		internal static ILunyScriptBlock CreatePlane(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Plane));

		internal static ILunyScriptBlock CreateQuad(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Quad));

		internal ObjectCreateBlock(CreateObject data) => _data = data;

		public void Execute(ILunyScriptContext context) => throw new NotImplementedException();
	}
}
