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
		private readonly IScript _script;
		internal OnApi(IScript script) => _script = script;
		private ScriptEventScheduler Scheduler => ((ILunyScriptInternal)_script).Scheduler;

		/// <summary>
		/// Runs once the moment when the object is instantiated.
		/// </summary>
		public ScriptSequenceBlock Created(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnCreated);

		/// <summary>
		/// Runs every time the object's state changes to 'enabled' (visible and participating).
		/// Runs directly after 'Created' if the object was just instantiated.
		/// </summary>
		public ScriptSequenceBlock Enabled(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnEnabled);

		/// <summary>
		/// Runs once per lifetime just before the object starts processing frame/time-step events.
		/// </summary>
		public ScriptSequenceBlock Ready(params ScriptActionBlock[] blocks) => Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnReady);

		/// <summary>
		/// Runs every time the object's state changes to 'disabled' (not visible, not participating).
		/// Runs directly before 'Destroyed' if the object was enabled as it got destroyed.
		/// </summary>
		public ScriptSequenceBlock Disabled(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnDisabled);

		/// <summary>
		/// Runs once when the object gets destroyed. The object is already disabled, the native engine instance still exists.
		/// </summary>
		public ScriptSequenceBlock Destroyed(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnDestroyed);

		/// <summary>
		/// Runs every frame while object is enabled.
		/// </summary>
		public ScriptSequenceBlock FrameUpdate(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnFrameUpdate);

		/// <summary>
		/// Runs after frame update while object is enabled.
		/// </summary>
		public ScriptSequenceBlock AfterFrameUpdate(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnFrameLateUpdate);

		/// <summary>
		/// Runs on fixed-rate stepping while object is enabled.
		/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
		/// May run multiple times per frame and may not run in every frame.
		/// </summary>
		public ScriptSequenceBlock Heartbeat(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnHeartbeat);
	}
}
