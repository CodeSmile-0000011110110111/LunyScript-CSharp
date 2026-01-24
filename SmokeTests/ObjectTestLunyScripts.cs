using System;

namespace LunyScript.Unity.SmokeTests
{
	public sealed class ObjectTestLunyScript : LunyScript
	{
		public const String DestroyedObjectName = "destroyed";
		public const String EmptyObjectName = "empty";
		public const String CubeObjectName = "cube";
		public const String SphereObjectName = "sphere";

		public override void Build()
		{
			When.Self.Created(Object.Create(DestroyedObjectName));
			When.Self.LateUpdates(Object.Destroy(DestroyedObjectName));

			When.Self.Ready(Object.Create(EmptyObjectName));
			When.Self.Ready(Object.CreateCube(CubeObjectName));
			When.Self.Ready(Object.CreateSphere(SphereObjectName));
		}
	}
}
