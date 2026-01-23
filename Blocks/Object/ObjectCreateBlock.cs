using Luny;
using Luny.Engine.Bridge.Enums;
using Luny.Engine.Services;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	public readonly struct CreateObjectData
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

		public CreateObjectData(String name, Type type, PrimitiveType primitiveType = PrimitiveType.Empty)
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
		private CreateObjectData _data;

		// TODO: refactor to individual blocks with base class (improves debugging, omit unused parameters in type)
		public static ILunyScriptBlock CreateEmpty(String name) => new ObjectCreateBlock(new CreateObjectData(name, CreateObjectData.Type.Empty));

		public static ILunyScriptBlock CreateWithPrefab(String prefabName) =>
			new ObjectCreateBlock(new CreateObjectData(prefabName, CreateObjectData.Type.Prefab));

		public static ILunyScriptBlock CreateClone(String originalName) =>
			new ObjectCreateBlock(new CreateObjectData(originalName, CreateObjectData.Type.Clone));

		public static ILunyScriptBlock CreateCube(String name = null) =>
			new ObjectCreateBlock(new CreateObjectData(name, CreateObjectData.Type.Primitive, PrimitiveType.Cube));

		public static ILunyScriptBlock CreateSphere(String name = null) =>
			new ObjectCreateBlock(new CreateObjectData(name, CreateObjectData.Type.Primitive, PrimitiveType.Sphere));

		public static ILunyScriptBlock CreateCapsule(String name = null) =>
			new ObjectCreateBlock(new CreateObjectData(name, CreateObjectData.Type.Primitive, PrimitiveType.Capsule));

		public static ILunyScriptBlock CreateCylinder(String name = null) =>
			new ObjectCreateBlock(new CreateObjectData(name, CreateObjectData.Type.Primitive, PrimitiveType.Cylinder));

		public static ILunyScriptBlock CreatePlane(String name = null) =>
			new ObjectCreateBlock(new CreateObjectData(name, CreateObjectData.Type.Primitive, PrimitiveType.Plane));

		public static ILunyScriptBlock CreateQuad(String name = null) =>
			new ObjectCreateBlock(new CreateObjectData(name, CreateObjectData.Type.Primitive, PrimitiveType.Quad));

		private ObjectCreateBlock() {}
		private ObjectCreateBlock(CreateObjectData data) => _data = data;

		public void Execute(ILunyScriptContext context)
		{
			var service = LunyEngine.Instance.Object;
			switch (_data.CreateType)
			{
				case CreateObjectData.Type.Empty:
					service.CreateEmpty(_data.Name);
					break;
				case CreateObjectData.Type.Primitive:
					service.CreatePrimitive(_data.Name, _data.PrimitiveType);
					break;
				case CreateObjectData.Type.Clone:
				case CreateObjectData.Type.Prefab:
					throw new NotImplementedException($"{_data.CreateType} instantiation is not yet implemented.");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
