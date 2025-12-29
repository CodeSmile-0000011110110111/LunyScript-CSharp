using System;

namespace LunyScript.Blocks
{
	public enum ObjectCreateType
	{
		Empty,
		Clone,
		Prefab,
		Primitive,
	}

	public enum PrimitiveType
	{
		Empty,
		Cube,
		Sphere,
		Capsule,
		Plane,
		Cylinder,
		Quad,
	}

	public readonly struct CreateObject
	{
		public readonly String Name;
		public readonly ObjectCreateType CreateType;
		public readonly PrimitiveType PrimitiveType;

		public CreateObject(String name, ObjectCreateType createType, PrimitiveType primitiveType = PrimitiveType.Empty)
		{
			Name = name;
			CreateType = createType;
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

		public ObjectCreateBlock(CreateObject data) => _data = data;

		public void Execute(IScriptContext context) => throw new NotImplementedException();
	}
}
