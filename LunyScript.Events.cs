using Luny;
using LunyScript.Execution;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		private static Boolean HasBlocks(IBlock[] blocks) => blocks?.Length > 0;
		private static RunnableSequence CreateSequence(IBlock[] blocks) => HasBlocks(blocks) ? new RunnableSequence(blocks) : null;

		private void ScheduleRunnable(RunnableSequence sequence, ObjectLifecycleEvents eventType) =>
			((ScriptContext)_context).Schedule(sequence, eventType);

		/// <summary>
		/// Provides scheduling of frequently repeating events.
		/// </summary>
		public static class Every
		{
			/// <summary>
			/// Schedules blocks to run on fixed-rate updates.
			/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
			/// May run multiple times per frame and may not run in every frame.
			/// It's therefore unsuitable for once-only events, such as Input.
			/// </summary>
			/// <param name="blocks"></param>
			public static void FixedStep(params IBlock[] blocks) =>
				_script.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnFixedStep);

			/// <summary>
			/// Schedules blocks to run on every-frame updates.
			/// </summary>
			/// <param name="blocks"></param>
			public static void Frame(params IBlock[] blocks) =>
				_script.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnUpdate);

			/// <summary>
			/// Schedules blocks to run on every-frame updates but runs after OnUpdate.
			/// </summary>
			/// <param name="blocks"></param>
			public static void FrameEnds(params IBlock[] blocks) =>
				_script.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnLateUpdate);
		}

		/// <summary>
		/// Handles infrequently occuring events, ie Input, Collision, Messages.
		/// </summary>
		public static class When
		{
			/// <summary>
			/// Runs once the moment when the object is instantiated.
			/// </summary>
			/// <param name="blocks"></param>
			/// <exception cref="NotImplementedException"></exception>
			public static void Created(params IBlock[] blocks) =>
				_script.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnCreate);

			/// <summary>
			/// Runs once when the object gets destroyed. The object is already disabled, the native engine instance still exists.
			/// </summary>
			/// <param name="blocks"></param>
			/// <exception cref="NotImplementedException"></exception>
			public static void Destroyed(params IBlock[] blocks) =>
				_script.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnDestroy);

			/// <summary>
			/// Runs every time the object's state changes to 'enabled' (visible and participating).
			/// Runs directly after 'Created' if the object was just instantiated.
			/// </summary>
			/// <param name="blocks"></param>
			/// <exception cref="NotImplementedException"></exception>
			public static void Enabled(params IBlock[] blocks) =>
				_script.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnEnable);

			/// <summary>
			/// Runs every time the object's state changes to 'disabled' (not visible, not participating).
			/// Runs directly before 'Destroyed' if the object was enabled as it got destroyed.
			/// </summary>
			/// <param name="blocks"></param>
			/// <exception cref="NotImplementedException"></exception>
			public static void Disabled(params IBlock[] blocks) =>
				_script.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnDisable);

			/// <summary>
			/// Runs once per lifetime just before the object starts processing frame/time-step events,
			/// eg before 'Every.Frame' / 'Every.FixedStep'.
			/// </summary>
			/// <param name="blocks"></param>
			/// <exception cref="NotImplementedException"></exception>
			public static void Ready(params IBlock[] blocks) => _script.ScheduleRunnable(CreateSequence(blocks), ObjectLifecycleEvents.OnReady);
		}
	}
}
