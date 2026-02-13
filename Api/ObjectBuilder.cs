using Luny.Engine.Bridge.Enums;
using LunyScript.Blocks;
using LunyScript.Coroutines.Builders;
using System;

namespace LunyScript.Api
{
	public interface IObjectBuilderState { }
	public interface IStart : IObjectBuilderState { }
	public interface IStateNameSet : IObjectBuilderState, ICanBuild { }
	public interface ICanBuild { }

	public struct Start : IStart { }
	public struct StateNameSet : IStateNameSet { }

	public readonly struct ObjectBuilder<T> where T : struct, IObjectBuilderState
	{
		internal readonly IScript Script;
		internal readonly ObjectCreateOptions Options;
		internal readonly BuilderToken Token;

		internal ObjectBuilder(IScript script, ObjectCreateOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;
		}

		/// <summary>
		/// Completes the builder and returns the executable block.
		/// </summary>
		public IScriptActionBlock Do() => BuilderUtility.Finalize(Script, Options, Token);
	}

	public static class ObjectBuilderExtensions
	{
		public static ObjectBuilder<StateNameSet> AsCube<T>(this ObjectBuilder<T> b) where T : struct, IStateNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Cube);

		public static ObjectBuilder<StateNameSet> AsSphere<T>(this ObjectBuilder<T> b) where T : struct, IStateNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Sphere);

		public static ObjectBuilder<StateNameSet> AsCapsule<T>(this ObjectBuilder<T> b) where T : struct, IStateNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Capsule);

		public static ObjectBuilder<StateNameSet> AsCylinder<T>(this ObjectBuilder<T> b) where T : struct, IStateNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Cylinder);

		public static ObjectBuilder<StateNameSet> AsPlane<T>(this ObjectBuilder<T> b) where T : struct, IStateNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Plane);

		public static ObjectBuilder<StateNameSet> AsQuad<T>(this ObjectBuilder<T> b) where T : struct, IStateNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Quad);

		private static ObjectBuilder<StateNameSet> WithPrimitive<T>(this ObjectBuilder<T> b, LunyPrimitiveType type) where T : struct, IStateNameSet
		{
			var options = b.Options;
			options.Mode = ObjectCreationMode.Primitive;
			options.PrimitiveType = type;
			return new ObjectBuilder<StateNameSet>(b.Script, options, b.Token);
		}

		public static ObjectBuilder<StateNameSet> From<T>(this ObjectBuilder<T> b, String prefabName) where T : struct, IStateNameSet
		{
			var options = b.Options;
			options.Mode = ObjectCreationMode.Prefab;
			options.AssetName = prefabName;
			return new ObjectBuilder<StateNameSet>(b.Script, options, b.Token);
		}

		public static ObjectBuilder<StateNameSet> Clone<T>(this ObjectBuilder<T> b, String existingName) where T : struct, IStateNameSet
		{
			var options = b.Options;
			options.Mode = ObjectCreationMode.Clone;
			options.AssetName = existingName;
			return new ObjectBuilder<StateNameSet>(b.Script, options, b.Token);
		}
	}
}
