using Luny.Engine.Bridge.Enums;
using LunyScript.Blocks;
using LunyScript.Execution;
using LunyScript.Runnables;
using System;
using System.Collections.Generic;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		private static Boolean HasBlocks(IReadOnlyList<ILunyScriptBlock> blocks) => blocks?.Count > 0;

		private static ILunyScriptRunnable CreateSequence(IReadOnlyList<ILunyScriptBlock> blocks) =>
			HasBlocks(blocks) ? new LunyScriptBlockSequence(blocks) : null;

		private ILunyScriptRunnable ScheduleRunnable(ILunyScriptRunnable sequence, LunyObjectLifecycleEvents eventType) =>
			((LunyScriptContext)_context).Scheduler.Schedule(sequence, eventType);

		/// <summary>
		/// Handles events, ie Lifecycle, Input, Collision, Messages.
		/// </summary>
		public static class When
		{
			/// <summary>
			/// Schedules blocks to run on fixed-rate updates.
			/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
			/// May run multiple times per frame and may not run in every frame.
			/// It's therefore unsuitable for once-only events, such as Input.
			/// </summary>
			/// <param name="blocks"></param>
			public static ILunyScriptRunnable EveryFixedStep(params ILunyScriptBlock[] blocks) =>
				s_Instance.ScheduleRunnable(CreateSequence(blocks), LunyObjectLifecycleEvents.OnFixedStep);

			/// <summary>
			/// Schedules blocks to run on every-frame updates.
			/// </summary>
			/// <param name="blocks"></param>
			public static ILunyScriptRunnable EveryFrame(params ILunyScriptBlock[] blocks) =>
				s_Instance.ScheduleRunnable(CreateSequence(blocks), LunyObjectLifecycleEvents.OnUpdate);

			/// <summary>
			/// Schedules blocks to run on every-frame updates but runs after OnUpdate.
			/// </summary>
			/// <param name="blocks"></param>
			public static ILunyScriptRunnable EveryFrameEnds(params ILunyScriptBlock[] blocks) =>
				s_Instance.ScheduleRunnable(CreateSequence(blocks), LunyObjectLifecycleEvents.OnLateUpdate);

			/// <summary>
			/// Runs once the moment when the object is instantiated.
			/// </summary>
			/// <param name="blocks"></param>
			/// <exception cref="NotImplementedException"></exception>
			public static ILunyScriptRunnable Created(params ILunyScriptBlock[] blocks) =>
				s_Instance.ScheduleRunnable(CreateSequence(blocks), LunyObjectLifecycleEvents.OnCreate);

			/// <summary>
			/// Runs once when the object gets destroyed. The object is already disabled, the native engine instance still exists.
			/// </summary>
			/// <param name="blocks"></param>
			/// <exception cref="NotImplementedException"></exception>
			public static ILunyScriptRunnable Destroyed(params ILunyScriptBlock[] blocks) =>
				s_Instance.ScheduleRunnable(CreateSequence(blocks), LunyObjectLifecycleEvents.OnDestroy);

			/// <summary>
			/// Runs every time the object's state changes to 'enabled' (visible and participating).
			/// Runs directly after 'Created' if the object was just instantiated.
			/// </summary>
			/// <param name="blocks"></param>
			/// <exception cref="NotImplementedException"></exception>
			public static ILunyScriptRunnable Enabled(params ILunyScriptBlock[] blocks) =>
				s_Instance.ScheduleRunnable(CreateSequence(blocks), LunyObjectLifecycleEvents.OnEnable);

			/// <summary>
			/// Runs every time the object's state changes to 'disabled' (not visible, not participating).
			/// Runs directly before 'Destroyed' if the object was enabled as it got destroyed.
			/// </summary>
			/// <param name="blocks"></param>
			/// <exception cref="NotImplementedException"></exception>
			public static ILunyScriptRunnable Disabled(params ILunyScriptBlock[] blocks) =>
				s_Instance.ScheduleRunnable(CreateSequence(blocks), LunyObjectLifecycleEvents.OnDisable);

			/// <summary>
			/// Runs once per lifetime just before the object starts processing frame/time-step events,
			/// eg before 'Every.Frame' / 'Every.FixedStep'.
			/// </summary>
			/// <param name="blocks"></param>
			/// <exception cref="NotImplementedException"></exception>
			public static ILunyScriptRunnable Ready(params ILunyScriptBlock[] blocks) =>
				s_Instance.ScheduleRunnable(CreateSequence(blocks), LunyObjectLifecycleEvents.OnReady);
		}
	}
}
