namespace LunyScript.SmokeTests.Coroutines
{
	public sealed class PhysicsSphereSpawner : Script
	{
		public override void Build(ScriptContext context)
		{
			var instantiate = Prefab.Instantiate("Prefabs/PhysicsSphere");
			var log = Debug.LogInfo("SPAWN SPHERE");
			On.Ready(log, instantiate);

			var createCounter = Counter("Sphere: Create").Every(50).Heartbeats().Do(log, instantiate);
			Counter("SphereSpawner: Destroy").In(50).Heartbeats().Do(createCounter.Stop());

			Timer("RELOAD").In(8).Seconds().Do(Scene.Reload());
		}
	}

	public sealed class PhysicsCubeSpawner : Script
	{
		public override void Build(ScriptContext context)
		{
			var instantiate = Prefab.Instantiate("Prefabs/PhysicsCube");
			var log = Debug.LogInfo("SPAWN CUBE");

			Counter("Cube: Create").Every(70).Heartbeats().Do(log, instantiate);
			Counter("CubeSpawner: Destroy").In(150).Heartbeats().Do(Object.Destroy());
		}
	}

	public sealed class PhysicsSphere : Script
	{
		public override void Build(ScriptContext context) => Timer("Sphere: Destroy").In(7.3).Seconds().Do(Object.Destroy());
	}

	public sealed class PhysicsCube : Script
	{
		public override void Build(ScriptContext context) => Timer("Cube: Destroy").In(5.5).Seconds().Do(Object.Destroy());
	}
}
