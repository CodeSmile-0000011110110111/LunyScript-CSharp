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
	internal sealed class ObjectCreateBlock : IBlock
	{
		private CreateObject _data;

		internal static IBlock CreateEmpty(String name) => new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Empty));

		internal static IBlock CreateWithPrefab(String prefabName) =>
			new ObjectCreateBlock(new CreateObject(prefabName, CreateObject.Type.Prefab));

		internal static IBlock CreateClone(String originalName) =>
			new ObjectCreateBlock(new CreateObject(originalName, CreateObject.Type.Clone));

		internal static IBlock CreateCube(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Cube));

		internal static IBlock CreateSphere(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Sphere));

		internal static IBlock CreateCapsule(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Capsule));

		internal static IBlock CreateCylinder(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Cylinder));

		internal static IBlock CreatePlane(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Plane));

		internal static IBlock CreateQuad(String name = null) =>
			new ObjectCreateBlock(new CreateObject(name, CreateObject.Type.Primitive, PrimitiveType.Quad));

		internal ObjectCreateBlock(CreateObject data) => _data = data;

		public void Execute(IScriptContext context) => throw new NotImplementedException();
	}
}
