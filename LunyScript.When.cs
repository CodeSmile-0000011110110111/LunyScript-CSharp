using Luny.Engine.Bridge.Enums;
using LunyScript.Blocks;
using LunyScript.Events;
using LunyScript.Runnables;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Handles infrequent events, ie Lifecycle, Input, Collision, Messages.
		/// </summary>
		public readonly struct WhenApi
		{
			private readonly ILunyScript _script;
			internal WhenApi(ILunyScript script) => _script = script;
			private LunyScriptEventScheduler Scheduler => ((ILunyScriptInternal)_script).Scheduler;

			/*
			/// <summary>
			/// Runs the given blocks at the specified time interval.
			/// </summary>
			public ILunyScriptRunnable Every(TimeSpan timeSpan, params ILunyScriptBlock[] blocks) =>
				Scheduler?.ScheduleSequence(blocks, timeSpan);
				*/

			public EngineApi Engine => new(_script);
			public SelfApi Self => new(_script);
			public SceneApi Scene => new(_script);

			public readonly struct EngineApi
			{
				private readonly ILunyScript _script;
				internal EngineApi(ILunyScript script) => _script = script;

				/* // TODO: this are considered "Global" updates ... needs corresponding event handler
				/// <summary>
				/// Runs on fixed-rate stepping. Continues to run even for disabled objects.
				/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
				/// May run multiple times per frame and may not run in every frame.
				/// It's therefore unsuitable for once-only events, such as Input.
				/// </summary>
				/// <param name="blocks"></param>
				public ILunyScriptRunnable Steps(params ILunyScriptBlock[] blocks) =>
					_script.GetScheduler()?.ScheduleSequence(blocks, LunyObjectEvent.OnFixedStep);

				/// <summary>
				/// Runs every frame. Continues to run even for disabled objects.
				/// </summary>
				/// <param name="blocks"></param>
				public ILunyScriptRunnable Updates(params ILunyScriptBlock[] blocks) =>
					_script.GetScheduler()?.ScheduleSequence(blocks, LunyObjectEvent.OnUpdate);

				/// <summary>
				/// Runs after frame update. Continues to run even for disabled objects.
				/// </summary>
				/// <param name="blocks"></param>
				public ILunyScriptRunnable LateUpdates(params ILunyScriptBlock[] blocks) =>
					_script.GetScheduler()?.ScheduleSequence(blocks, LunyObjectEvent.OnLateUpdate);
					*/
			}

			/// <summary>
			/// Self Events operate on the object in context
			/// </summary>
			public readonly struct SelfApi
			{
				private readonly ILunyScript _script;
				internal SelfApi(ILunyScript script) => _script = script;
				private LunyScriptEventScheduler Scheduler => ((ILunyScriptInternal)_script).Scheduler;

				/// <summary>
				/// Runs once the moment when the object is instantiated.
				/// </summary>
				/// <param name="blocks"></param>
				/// <exception cref="NotImplementedException"></exception>
				public ILunyScriptRunnable Created(params ILunyScriptBlock[] blocks) =>
					Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnCreate);

				/// <summary>
				/// Runs once when the object gets destroyed. The object is already disabled, the native engine instance still exists.
				/// </summary>
				/// <param name="blocks"></param>
				/// <exception cref="NotImplementedException"></exception>
				public ILunyScriptRunnable Destroyed(params ILunyScriptBlock[] blocks) =>
					Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnDestroy);

				/// <summary>
				/// Runs every time the object's state changes to 'enabled' (visible and participating).
				/// Runs directly after 'Created' if the object was just instantiated.
				/// </summary>
				/// <param name="blocks"></param>
				/// <exception cref="NotImplementedException"></exception>
				public ILunyScriptRunnable Enabled(params ILunyScriptBlock[] blocks) =>
					Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnEnable);

				/// <summary>
				/// Runs every time the object's state changes to 'disabled' (not visible, not participating).
				/// Runs directly before 'Destroyed' if the object was enabled as it got destroyed.
				/// </summary>
				/// <param name="blocks"></param>
				/// <exception cref="NotImplementedException"></exception>
				public ILunyScriptRunnable Disabled(params ILunyScriptBlock[] blocks) =>
					Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnDisable);

				/// <summary>
				/// Runs once per lifetime just before the object starts processing frame/time-step events,
				/// eg before 'Every.Frame' / 'Every.FixedStep'.
				/// </summary>
				/// <param name="blocks"></param>
				/// <exception cref="NotImplementedException"></exception>
				public ILunyScriptRunnable Ready(params ILunyScriptBlock[] blocks) =>
					Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnReady);

				/// <summary>
				/// Runs on fixed-rate stepping while object is enabled.
				/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
				/// May run multiple times per frame and may not run in every frame.
				/// It's therefore unsuitable for once-only events, such as Input.
				/// </summary>
				/// <param name="blocks"></param>
				public ILunyScriptRunnable Steps(params ILunyScriptBlock[] blocks) =>
					Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnFixedStep);

				/// <summary>
				/// Runs every frame while object is enabled.
				/// </summary>
				/// <param name="blocks"></param>
				public ILunyScriptRunnable Updates(params ILunyScriptBlock[] blocks) =>
					Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnUpdate);

				/// <summary>
				/// Runs after frame update while object is enabled.
				/// </summary>
				/// <param name="blocks"></param>
				public ILunyScriptRunnable LateUpdates(params ILunyScriptBlock[] blocks) =>
					Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnLateUpdate);
			}

			/// <summary>
			/// Scene Events
			/// </summary>
			public readonly struct SceneApi
			{
				private readonly ILunyScript _script;
				internal SceneApi(ILunyScript script) => _script = script;
				private LunyScriptEventScheduler Scheduler => ((ILunyScriptInternal)_script).Scheduler;

				/// <summary>
				/// Runs when a scene has loaded.
				/// </summary>
				/// <param name="blocks"></param>
				/// <returns></returns>
				public ILunyScriptRunnable Loads(params ILunyScriptBlock[] blocks) =>
					Scheduler?.ScheduleSequence(blocks, LunySceneEvent.OnSceneLoaded);

				/// <summary>
				/// Runs when a scene has loaded.
				/// </summary>
				/// <param name="sceneName"></param>
				/// <param name="blocks"></param>
				/// <returns></returns>
				/// <exception cref="NotImplementedException"></exception>
				public ILunyScriptRunnable Loads(String sceneName, params ILunyScriptBlock[] blocks) =>
					throw new NotImplementedException(nameof(Loads));

				/// <summary>
				/// Runs when a scene has unloaded.
				/// </summary>
				/// <param name="blocks"></param>
				/// <returns></returns>
				public ILunyScriptRunnable Unloads(params ILunyScriptBlock[] blocks) =>
					Scheduler?.ScheduleSequence(blocks, LunySceneEvent.OnSceneUnloaded);

				/// <summary>
				/// Runs when a scene has unloaded.
				/// </summary>
				/// <param name="sceneName"></param>
				/// <param name="blocks"></param>
				/// <returns></returns>
				/// <exception cref="NotImplementedException"></exception>
				public ILunyScriptRunnable Unloads(String sceneName, params ILunyScriptBlock[] blocks) =>
					throw new NotImplementedException(nameof(Unloads));
			}
		}
	}
}
