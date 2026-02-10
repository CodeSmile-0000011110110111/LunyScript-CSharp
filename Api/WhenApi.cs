using Luny.Engine.Bridge.Enums;
using LunyScript.Blocks;
using LunyScript.Events;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Handles external events: Scene, Input, Collision, Messages.
	/// </summary>
	public readonly struct WhenApi
	{
		private readonly ILunyScript _script;
		internal WhenApi(ILunyScript script) => _script = script;

		public SceneApi Scene => new(_script);

		/// <summary>
		/// Scene Events
		/// </summary>
		public readonly struct SceneApi
		{
			private readonly ILunyScript _script;
			internal SceneApi(ILunyScript script) => _script = script;
			private ScriptEventScheduler Scheduler => ((ILunyScriptInternal)_script).Scheduler;

			/// <summary>
			/// Runs when a scene has loaded.
			/// </summary>
			/// <param name="blocks"></param>
			/// <returns></returns>
			public IScriptSequenceBlock Loads(params IScriptActionBlock[] blocks) =>
				Scheduler?.ScheduleSequence(blocks, LunySceneEvent.OnSceneLoaded);

			/// <summary>
			/// Runs when a scene has loaded.
			/// </summary>
			/// <param name="sceneName"></param>
			/// <param name="blocks"></param>
			/// <returns></returns>
			/// <exception cref="NotImplementedException"></exception>
			public IScriptSequenceBlock Loads(String sceneName, params IScriptActionBlock[] blocks) =>
				throw new NotImplementedException(nameof(Loads));

			/// <summary>
			/// Runs when a scene has unloaded.
			/// </summary>
			/// <param name="blocks"></param>
			/// <returns></returns>
			public IScriptSequenceBlock Unloads(params IScriptActionBlock[] blocks) =>
				Scheduler?.ScheduleSequence(blocks, LunySceneEvent.OnSceneUnloaded);

			/// <summary>
			/// Runs when a scene has unloaded.
			/// </summary>
			/// <param name="sceneName"></param>
			/// <param name="blocks"></param>
			/// <returns></returns>
			/// <exception cref="NotImplementedException"></exception>
			public IScriptSequenceBlock Unloads(String sceneName, params IScriptActionBlock[] blocks) =>
				throw new NotImplementedException(nameof(Unloads));
		}
	}
}
