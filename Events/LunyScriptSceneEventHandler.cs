using Luny;
using Luny.Engine.Bridge;
using Luny.Engine.Bridge.Enums;
using Luny.Engine.Bridge.Identity;
using LunyScript.Execution;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript.Events
{
	/// <summary>
	/// Manages object lifecycle events by attaching hooks to LunyObjects and handling event dispatch.
	/// Coordinates enable/disable state changes and deferred object destruction.
	/// </summary>
	internal sealed class LunyScriptSceneEventHandler : ILunyScriptLifecycleInternal
	{
		[NotNull] private readonly LunyScriptContextRegistry _contexts;

		private readonly List<LunyObjectID> _subscriberObjectIDs = new();

		internal LunyScriptSceneEventHandler(LunyScriptContextRegistry contextRegistry) =>
			_contexts = contextRegistry ?? throw new ArgumentNullException(nameof(contextRegistry));

		~LunyScriptSceneEventHandler() => LunyTraceLogger.LogInfoFinalized(this);

		/// <summary>
		/// Registers lifecycle hooks on a LunyObject for the given context.
		/// Called during ScriptContext construction.
		/// </summary>
		internal void Register(LunyScriptContext context)
		{
			if (context.Scheduler.IsObservingAnyOf(typeof(LunySceneEvent)))
				_subscriberObjectIDs.Add(context.LunyObject.LunyObjectID);
		}

		public void OnSceneUnloaded(ILunyScene scene)
		{
			foreach (var subscriberID in _subscriberObjectIDs)
				TryRunForEvent(subscriberID, LunySceneEvent.OnSceneUnloaded);
		}

		public void OnSceneLoaded(ILunyScene scene)
		{
			foreach (var subscriberID in _subscriberObjectIDs)
				TryRunForEvent(subscriberID, LunySceneEvent.OnSceneLoaded);
		}

		private void TryRunForEvent(LunyObjectID subscriberID, LunySceneEvent sceneEvent)
		{
			var context = _contexts.GetByLunyObjectID(subscriberID);
			var sequences = context?.Scheduler?.GetSequences(sceneEvent);
			if (sequences != null)
			{
				LunyLogger.LogInfo($"Running {nameof(sceneEvent)} for {context}", this);
				LunyScriptRunner.Run(sequences, context);
			}
		}

		public void Shutdown() => _subscriberObjectIDs.Clear();
	}
}
