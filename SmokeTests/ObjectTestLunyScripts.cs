using System;

namespace LunyScript.SmokeTests
{
	public sealed class ObjectTestLunyScript : LunyScript
	{
		public const String DestroyedObjectName = "destroyed";
		public const String EmptyObjectName = "empty";
		public const String CubeObjectName = "cube";
		public const String SphereObjectName = "sphere";

		public override void Build()
		{
			On.Created(Object.Create(DestroyedObjectName));
			On.FrameEnd(Object.Destroy(DestroyedObjectName));

			On.Ready(Object.Create(EmptyObjectName));
			On.Ready(Object.CreateCube(CubeObjectName));
			On.Ready(Object.CreateSphere(SphereObjectName));
			On.Ready(Prefab.Instantiate("TestPrefab"));
		}
	}
}
