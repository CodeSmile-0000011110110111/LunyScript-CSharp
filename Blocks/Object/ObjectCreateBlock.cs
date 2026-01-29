using Luny;
using Luny.Engine.Bridge.Enums;
using Luny.Engine.Services;
using LunyScript.Execution;
using System;

namespace LunyScript.Blocks
{
	internal abstract class ObjectCreateBlockBase : IScriptActionBlock
	{
		protected readonly String Name;
		protected static ILunyObjectService Object => LunyEngine.Instance.Object;

		protected ObjectCreateBlockBase(String name) => Name = name;

		public abstract void Execute(ILunyScriptContext context);
	}

	internal sealed class ObjectCreateEmptyBlock : ObjectCreateBlockBase
	{
		public static IScriptActionBlock Create(String name) => new ObjectCreateEmptyBlock(name);

		private ObjectCreateEmptyBlock(String name)
			: base(name) {}

		public override void Execute(ILunyScriptContext context) => Object.CreateEmpty(Name);
	}

	internal sealed class ObjectCreateCubeBlock : ObjectCreateBlockBase
	{
		public static IScriptActionBlock Create(String name) => new ObjectCreateCubeBlock(name);

		private ObjectCreateCubeBlock(String name)
			: base(name) {}

		public override void Execute(ILunyScriptContext context) => Object.CreatePrimitive(Name, LunyPrimitiveType.Cube);
	}

	internal sealed class ObjectCreateSphereBlock : ObjectCreateBlockBase
	{
		public static IScriptActionBlock Create(String name) => new ObjectCreateSphereBlock(name);

		private ObjectCreateSphereBlock(String name)
			: base(name) {}

		public override void Execute(ILunyScriptContext context) => Object.CreatePrimitive(Name, LunyPrimitiveType.Sphere);
	}

	internal sealed class ObjectCreateCapsuleBlock : ObjectCreateBlockBase
	{
		public static IScriptActionBlock Create(String name) => new ObjectCreateCapsuleBlock(name);

		private ObjectCreateCapsuleBlock(String name)
			: base(name) {}

		public override void Execute(ILunyScriptContext context) => Object.CreatePrimitive(Name, LunyPrimitiveType.Capsule);
	}

	internal sealed class ObjectCreateCylinderBlock : ObjectCreateBlockBase
	{
		public static IScriptActionBlock Create(String name) => new ObjectCreateCylinderBlock(name);

		private ObjectCreateCylinderBlock(String name)
			: base(name) {}

		public override void Execute(ILunyScriptContext context) => Object.CreatePrimitive(Name, LunyPrimitiveType.Cylinder);
	}

	internal sealed class ObjectCreatePlaneBlock : ObjectCreateBlockBase
	{
		public static IScriptActionBlock Create(String name) => new ObjectCreatePlaneBlock(name);

		private ObjectCreatePlaneBlock(String name)
			: base(name) {}

		public override void Execute(ILunyScriptContext context) => Object.CreatePrimitive(Name, LunyPrimitiveType.Plane);
	}

	internal sealed class ObjectCreateQuadBlock : ObjectCreateBlockBase
	{
		public static IScriptActionBlock Create(String name) => new ObjectCreateQuadBlock(name);

		private ObjectCreateQuadBlock(String name)
			: base(name) {}

		public override void Execute(ILunyScriptContext context) => Object.CreatePrimitive(Name, LunyPrimitiveType.Quad);
	}

	internal sealed class ObjectCreatePrefabBlock : ObjectCreateBlockBase
	{
		public static IScriptActionBlock Create(String prefabName) => new ObjectCreatePrefabBlock(prefabName);

		private ObjectCreatePrefabBlock(String prefabName)
			: base(prefabName) {}

		public override void Execute(ILunyScriptContext context) =>
			throw new NotImplementedException($"{nameof(ObjectCreatePrefabBlock)}.{nameof(Execute)} is not yet implemented.");
	}

	internal sealed class ObjectCreateCloneBlock : ObjectCreateBlockBase
	{
		public static IScriptActionBlock Create(String originalName) => new ObjectCreateCloneBlock(originalName);

		private ObjectCreateCloneBlock(String originalName)
			: base(originalName) {}

		public override void Execute(ILunyScriptContext context) =>
			throw new NotImplementedException($"{nameof(ObjectCreateCloneBlock)}.{nameof(Execute)} is not yet implemented.");
	}
}
