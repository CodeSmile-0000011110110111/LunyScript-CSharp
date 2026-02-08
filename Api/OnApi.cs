using Luny.Engine.Bridge.Enums;
using LunyScript.Blocks;
using LunyScript.Events;

namespace LunyScript.Api
{
	/// <summary>
	/// Handles object lifecycle and update events.
	/// </summary>
	public readonly struct OnApi
	{
		private readonly ILunyScript _script;
		internal OnApi(ILunyScript script) => _script = script;
		private LunyScriptEventScheduler Scheduler => ((ILunyScriptInternal)_script).Scheduler;

		/// <summary>
		/// Runs once the moment when the object is instantiated.
		/// </summary>
		public IScriptSequenceBlock Created(params IScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnCreate);

		/// <summary>
		/// Runs every time the object's state changes to 'enabled' (visible and participating).
		/// Runs directly after 'Created' if the object was just instantiated.
		/// </summary>
		public IScriptSequenceBlock Enabled(params IScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnEnable);

		/// <summary>
		/// Runs once per lifetime just before the object starts processing frame/time-step events.
		/// </summary>
		public IScriptSequenceBlock Ready(params IScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnReady);

		/// <summary>
		/// Runs every time the object's state changes to 'disabled' (not visible, not participating).
		/// Runs directly before 'Destroyed' if the object was enabled as it got destroyed.
		/// </summary>
		public IScriptSequenceBlock Disabled(params IScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnDisable);

		/// <summary>
		/// Runs once when the object gets destroyed. The object is already disabled, the native engine instance still exists.
		/// </summary>
		public IScriptSequenceBlock Destroyed(params IScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnDestroy);

		/// <summary>
		/// Runs every frame while object is enabled.
		/// </summary>
		public IScriptSequenceBlock FrameUpdate(params IScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnFrameUpdate);

		/// <summary>
		/// Runs after frame update while object is enabled.
		/// </summary>
		public IScriptSequenceBlock FrameEnd(params IScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnFrameLateUpdate);

		/// <summary>
		/// Runs on fixed-rate stepping while object is enabled.
		/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
		/// May run multiple times per frame and may not run in every frame.
		/// </summary>
		public IScriptSequenceBlock Heartbeat(params IScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnHeartbeat);
	}
}
