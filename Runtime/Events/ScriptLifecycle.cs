using Luny;
using Luny.Engine.Bridge.Enums;
using LunyScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript.Events
{
	internal interface ILunyScriptLifecycleInternal {}

	/// <summary>
	/// Manages object lifecycle events by attaching hooks to LunyObjects and handling event dispatch.
	/// Coordinates enable/disable state changes and deferred object destruction.
	/// </summary>
	internal sealed class ScriptLifecycle : ILunyScriptLifecycleInternal
	{
		[NotNull] private readonly ScriptRuntimeContextRegistry _contexts;
		private readonly Dictionary<ScriptRuntimeContext, ObjectEventHandler> _subscribers = new();

		internal ScriptLifecycle(ScriptRuntimeContextRegistry runtimeContextRegistry) =>
			_contexts = runtimeContextRegistry ?? throw new ArgumentNullException(nameof(runtimeContextRegistry));

		~ScriptLifecycle() => LunyTraceLogger.LogInfoFinalized(this);

		/// <summary>
		/// Registers lifecycle hooks on a LunyObject for the given context.
		/// Called during ScriptContext construction.
		/// </summary>
		internal void Register(ScriptRuntimeContext runtimeContext)
		{
			var subscriber = new ObjectEventHandler(this, _contexts, runtimeContext);
			_subscribers[runtimeContext] = subscriber;
		}

		/// <summary>
		/// Unregisters lifecycle hooks from a LunyObject.
		/// Called during context cleanup or shutdown.
		/// </summary>
		private void Unregister(ScriptRuntimeContext runtimeContext) => _subscribers.Remove(runtimeContext);

		public void OnHeartbeat(ScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext.LunyObject.IsEnabled)
			{
				var sequences = runtimeContext.Scheduler.GetSequences(LunyObjectEvent.OnHeartbeat);
				LunyScriptRunner.Run(sequences, runtimeContext);

				runtimeContext.Coroutines?.OnHeartbeat(runtimeContext);
			}
		}

		public void OnFrameUpdate(ScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext.LunyObject.IsEnabled)
			{
				var sequences = runtimeContext.Scheduler.GetSequences(LunyObjectEvent.OnFrameUpdate);
				LunyScriptRunner.Run(sequences, runtimeContext);

				runtimeContext.Coroutines?.OnFrameUpdate(runtimeContext);
			}
		}

		public void OnFrameLateUpdate(ScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext.LunyObject.IsEnabled)
			{
				var sequences = runtimeContext.Scheduler.GetSequences(LunyObjectEvent.OnFrameLateUpdate);
				LunyScriptRunner.Run(sequences, runtimeContext);
			}
		}

		public void Shutdown()
		{
			// remove all subscribers and their events
			foreach (var subscriber in _subscribers.Values)
				subscriber.UnregisterAllCallbacks();

			_subscribers.Clear();
		}

		private sealed class ObjectEventHandler
		{
			private readonly ScriptLifecycle _lifecycle;
			private readonly ScriptRuntimeContextRegistry _contexts;
			private readonly ScriptRuntimeContext _runtimeContext;
#if DEBUG || LUNYSCRIPT_DEBUG
			private Boolean _processingEnableDisableReentryLock;
#endif

			public ObjectEventHandler(ScriptLifecycle lifecycle, ScriptRuntimeContextRegistry contexts, ScriptRuntimeContext runtimeContext)
			{
				_lifecycle = lifecycle;
				_contexts = contexts;
				_runtimeContext = runtimeContext;
				RegisterAllCallbacks();
			}

			private void RegisterAllCallbacks()
			{
				var lunyObject = _runtimeContext.LunyObject;
				lunyObject.OnCreate += OnCreate;
				lunyObject.OnDestroy += OnDestroy;
				lunyObject.OnReady += OnReady;
				lunyObject.OnEnable += OnEnable;
				lunyObject.OnDisable += OnDisable;
			}

			internal void UnregisterAllCallbacks()
			{
				var lunyObject = _runtimeContext.LunyObject;
				lunyObject.OnCreate -= OnCreate;
				lunyObject.OnDestroy -= OnDestroy;
				lunyObject.OnReady -= OnReady;
				lunyObject.OnEnable -= OnEnable;
				lunyObject.OnDisable -= OnDisable;
			}

			private void RunScheduledForEvent(LunyObjectEvent objectEvent)
			{
				var sequences = _runtimeContext.Scheduler.GetSequences(objectEvent);
				if (sequences != null)
					LunyLogger.LogInfo($"<{objectEvent}> for {_runtimeContext}", _lifecycle);

				LunyScriptRunner.Run(sequences, _runtimeContext);
			}

			private void UnscheduleOnceOnlyEvent(LunyObjectEvent objectEvent)
			{
				// Note: during the event, the script may have run Object.Destroy() on its object, thus invalidating it
				if (!_runtimeContext.LunyObject.IsValid)
					return;

				if (objectEvent == LunyObjectEvent.OnCreate || objectEvent == LunyObjectEvent.OnReady)
				{
					// event never fires again for this object
					_runtimeContext.Scheduler.Unschedule(objectEvent);

					if (objectEvent == LunyObjectEvent.OnCreate)
						_runtimeContext.LunyObject.OnCreate -= OnCreate;
					else if (objectEvent == LunyObjectEvent.OnReady)
						_runtimeContext.LunyObject.OnReady -= OnReady;
				}
			}

			private void OnCreate()
			{
				RunScheduledForEvent(LunyObjectEvent.OnCreate);
				UnscheduleOnceOnlyEvent(LunyObjectEvent.OnCreate);
			}

			private void OnDestroy()
			{
				RunScheduledForEvent(LunyObjectEvent.OnDestroy);
				UnregisterAllCallbacks(); // no more events
				_runtimeContext.Coroutines.OnObjectDestroyed(_runtimeContext);
				_lifecycle.Unregister(_runtimeContext);
				_contexts.Unregister(_runtimeContext);
			}

			private void OnReady()
			{
				RunScheduledForEvent(LunyObjectEvent.OnReady);
				UnscheduleOnceOnlyEvent(LunyObjectEvent.OnReady);
			}

			private void OnEnable()
			{
				ThrowIfAlreadyProcessingEnableDisableEvent(_runtimeContext);
				SetProcessingEnableDisableReentryLock(true);
				RunScheduledForEvent(LunyObjectEvent.OnEnable);
				SetProcessingEnableDisableReentryLock(false);
			}

			private void OnDisable()
			{
				ThrowIfAlreadyProcessingEnableDisableEvent(_runtimeContext);
				SetProcessingEnableDisableReentryLock(true);
				RunScheduledForEvent(LunyObjectEvent.OnDisable);
				SetProcessingEnableDisableReentryLock(false);
			}

			[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
			private void SetProcessingEnableDisableReentryLock(Boolean locked)
			{
#if DEBUG || LUNYSCRIPT_DEBUG
				_processingEnableDisableReentryLock = locked;
#endif
			}

			[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
			private void ThrowIfAlreadyProcessingEnableDisableEvent(ScriptRuntimeContext runtimeContext)
			{
#if DEBUG || LUNYSCRIPT_DEBUG
				// Safeguard against infinite loops (OnEnable toggles to disabled, which triggers OnDisable, etc.)
				if (_processingEnableDisableReentryLock)
				{
					throw new LunyScriptException("Disabling in When.Enabled while ALSO enabling in When.Disabled is not allowed " +
					                              $"(would cause an infinite loop). Script: {runtimeContext}");
				}
#endif
			}
		}
	}
}
