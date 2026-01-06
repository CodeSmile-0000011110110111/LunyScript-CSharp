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

		public static ILunyScriptBlock CreateEmpty(String name) => new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Empty));

		public static ILunyScriptBlock CreateWithPrefab(String prefabName) =>
			new ObjectCreateBlock(new CreateObject(prefabName, CreateObject.Type.Prefab));

		public static ILunyScriptBlock CreateClone(String originalName) =>
			new ObjectCreateBlock(new CreateObject(originalName, CreateObject.Type.Clone));

		public static ILunyScriptBlock CreateCube(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Cube));

		public static ILunyScriptBlock CreateSphere(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Sphere));

		public static ILunyScriptBlock CreateCapsule(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Capsule));

		public static ILunyScriptBlock CreateCylinder(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Cylinder));

		public static ILunyScriptBlock CreatePlane(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Plane));

		public static ILunyScriptBlock CreateQuad(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Quad));

		private ObjectCreateBlock() {}
		private ObjectCreateBlock(CreateObject data) => _data = data;

		public void Execute(ILunyScriptContext context) => throw new NotImplementedException();
	}
}
