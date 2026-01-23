using Luny.Engine.Bridge.Enums;
using LunyScript.Blocks;
using LunyScript.Events;
using LunyScript.Execution;
using LunyScript.Runnables;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		private static LunyScriptEventScheduler Scheduler => ((LunyScriptContext)s_Instance._context).Scheduler;

		public static class Every
		{
			/// <summary>
			/// Schedules blocks to run on fixed-rate updates.
			/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
			/// May run multiple times per frame and may not run in every frame.
			/// It's therefore unsuitable for once-only events, such as Input.
			/// </summary>
			/// <param name="blocks"></param>
			public static ILunyScriptRunnable FixedStep(params ILunyScriptBlock[] blocks) =>
				Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnFixedStep);

			/// <summary>
			/// Schedules blocks to run on every-frame updates.
			/// </summary>
			/// <param name="blocks"></param>
			public static ILunyScriptRunnable Frame(params ILunyScriptBlock[] blocks) =>
				Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnUpdate);

			/// <summary>
			/// Schedules blocks to run on every-frame updates but runs after OnUpdate.
			/// </summary>
			/// <param name="blocks"></param>
			public static ILunyScriptRunnable FrameEnds(params ILunyScriptBlock[] blocks) =>
				Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnLateUpdate);

			public static ILunyScriptRunnable TimeInterval(TimeSpan timeSpan, params ILunyScriptBlock[] blocks) =>
				throw new NotImplementedException(nameof(TimeInterval));
		}

		/// <summary>
		/// Handles infrequent events, ie Lifecycle, Input, Collision, Messages.
		/// </summary>
		public static class When
		{
			/// <summary>
			/// Self Events operate on the object in context
			/// </summary>
			public static class Self
			{
				/// <summary>
				/// Runs once the moment when the object is instantiated.
				/// </summary>
				/// <param name="blocks"></param>
				/// <exception cref="NotImplementedException"></exception>
				public static ILunyScriptRunnable Created(params ILunyScriptBlock[] blocks) =>
					Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnCreate);

				/// <summary>
				/// Runs once when the object gets destroyed. The object is already disabled, the native engine instance still exists.
				/// </summary>
				/// <param name="blocks"></param>
				/// <exception cref="NotImplementedException"></exception>
				public static ILunyScriptRunnable Destroyed(params ILunyScriptBlock[] blocks) =>
					Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnDestroy);

				/// <summary>
				/// Runs every time the object's state changes to 'enabled' (visible and participating).
				/// Runs directly after 'Created' if the object was just instantiated.
				/// </summary>
				/// <param name="blocks"></param>
				/// <exception cref="NotImplementedException"></exception>
				public static ILunyScriptRunnable Enabled(params ILunyScriptBlock[] blocks) =>
					Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnEnable);

				/// <summary>
				/// Runs every time the object's state changes to 'disabled' (not visible, not participating).
				/// Runs directly before 'Destroyed' if the object was enabled as it got destroyed.
				/// </summary>
				/// <param name="blocks"></param>
				/// <exception cref="NotImplementedException"></exception>
				public static ILunyScriptRunnable Disabled(params ILunyScriptBlock[] blocks) =>
					Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnDisable);

				/// <summary>
				/// Runs once per lifetime just before the object starts processing frame/time-step events,
				/// eg before 'Every.Frame' / 'Every.FixedStep'.
				/// </summary>
				/// <param name="blocks"></param>
				/// <exception cref="NotImplementedException"></exception>
				public static ILunyScriptRunnable Ready(params ILunyScriptBlock[] blocks) =>
					Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnReady);

				/// <summary>
				/// Schedules blocks to run on fixed-rate updates.
				/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
				/// May run multiple times per frame and may not run in every frame.
				/// It's therefore unsuitable for once-only events, such as Input.
				/// </summary>
				/// <param name="blocks"></param>
				public static ILunyScriptRunnable Steps(params ILunyScriptBlock[] blocks) =>
					Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnFixedStep);

				/// <summary>
				/// Schedules blocks to run on every frame update.
				/// </summary>
				/// <param name="blocks"></param>
				public static ILunyScriptRunnable Updates(params ILunyScriptBlock[] blocks) =>
					Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnUpdate);

				/// <summary>
				/// Schedules blocks to run after every frame update.
				/// </summary>
				/// <param name="blocks"></param>
				public static ILunyScriptRunnable LateUpdates(params ILunyScriptBlock[] blocks) =>
					Scheduler.ScheduleSequence(blocks, LunyObjectEvent.OnLateUpdate);

			}

			/// <summary>
			/// Scene Events
			/// </summary>
			public static class Scene
			{
				/// <summary>
				/// Runs when a scene has loaded.
				/// </summary>
				/// <param name="blocks"></param>
				/// <returns></returns>
				public static ILunyScriptRunnable Loads(params ILunyScriptBlock[] blocks) =>
					Scheduler.ScheduleSequence(blocks, LunySceneEvent.OnSceneLoaded);

				/// <summary>
				/// Runs when a scene has loaded.
				/// </summary>
				/// <param name="sceneName"></param>
				/// <param name="blocks"></param>
				/// <returns></returns>
				/// <exception cref="NotImplementedException"></exception>
				public static ILunyScriptRunnable Loads(String sceneName, params ILunyScriptBlock[] blocks) =>
					throw new NotImplementedException(nameof(Loads));

				/// <summary>
				/// Runs when a scene has unloaded.
				/// </summary>
				/// <param name="blocks"></param>
				/// <returns></returns>
				public static ILunyScriptRunnable Unloads(params ILunyScriptBlock[] blocks) =>
					Scheduler.ScheduleSequence(blocks, LunySceneEvent.OnSceneUnloaded);

				/// <summary>
				/// Runs when a scene has unloaded.
				/// </summary>
				/// <param name="sceneName"></param>
				/// <param name="blocks"></param>
				/// <returns></returns>
				/// <exception cref="NotImplementedException"></exception>
				public static ILunyScriptRunnable Unloads(String sceneName, params ILunyScriptBlock[] blocks) =>
					throw new NotImplementedException(nameof(Unloads));
			}
		}
	}
}
