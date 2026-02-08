using Luny;
using Luny.Engine.Bridge.Enums;
using LunyScript.Exceptions;
using LunyScript.Execution;
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
	internal sealed class LunyScriptObjectLifecycle : ILunyScriptLifecycleInternal
	{
		[NotNull] private readonly LunyScriptContextRegistry _contexts;
		private readonly Dictionary<LunyScriptContext, ObjectEventHandler> _subscribers = new();

		internal LunyScriptObjectLifecycle(LunyScriptContextRegistry contextRegistry) =>
			_contexts = contextRegistry ?? throw new ArgumentNullException(nameof(contextRegistry));

		~LunyScriptObjectLifecycle() => LunyTraceLogger.LogInfoFinalized(this);

		/// <summary>
		/// Registers lifecycle hooks on a LunyObject for the given context.
		/// Called during ScriptContext construction.
		/// </summary>
		internal void Register(LunyScriptContext context)
		{
			var subscriber = new ObjectEventHandler(this, context);
			_subscribers[context] = subscriber;
		}

		/// <summary>
		/// Unregisters lifecycle hooks from a LunyObject.
		/// Called during context cleanup or shutdown.
		/// </summary>
		private void Unregister(LunyScriptContext context)
		{
			_subscribers.Remove(context);
			_contexts.Unregister(context);
		}

		public void OnHeartbeat(Double fixedDeltaTime, LunyScriptContext context)
		{
			if (context.LunyObject.IsEnabled)
			{
				var sequences = context.Scheduler.GetSequences(LunyObjectEvent.OnHeartbeat);
				LunyScriptRunner.Run(sequences, context);

				context.Coroutines?.OnHeartbeat(fixedDeltaTime, context);
			}
		}

		public void OnFrameUpdate(Double deltaTime, LunyScriptContext context)
		{
			if (context.LunyObject.IsEnabled)
			{
				var sequences = context.Scheduler.GetSequences(LunyObjectEvent.OnFrameUpdate);
				LunyScriptRunner.Run(sequences, context);

				context.Coroutines?.OnFrameUpdate(deltaTime, context);
			}
		}

		public void OnFrameLateUpdate(Double deltaTime, LunyScriptContext context)
		{
			if (context.LunyObject.IsEnabled)
			{
				var sequences = context.Scheduler.GetSequences(LunyObjectEvent.OnFrameLateUpdate);
				LunyScriptRunner.Run(sequences, context);
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
			private readonly LunyScriptObjectLifecycle _objectLifecycle;
			private readonly LunyScriptContext _context;
#if DEBUG || LUNYSCRIPT_DEBUG
			private Boolean _processingEnableDisableReentryLock;
#endif

			public ObjectEventHandler(LunyScriptObjectLifecycle objectLifecycle, LunyScriptContext context)
			{
				_objectLifecycle = objectLifecycle;
				_context = context;
				RegisterAllCallbacks();
			}

			private void RegisterAllCallbacks()
			{
				var lunyObject = _context.LunyObject;
				lunyObject.OnCreate += OnCreate;
				lunyObject.OnDestroy += OnDestroy;
				lunyObject.OnReady += OnReady;
				lunyObject.OnEnable += OnEnable;
				lunyObject.OnDisable += OnDisable;
			}

			internal void UnregisterAllCallbacks()
			{
				var lunyObject = _context.LunyObject;
				lunyObject.OnCreate -= OnCreate;
				lunyObject.OnDestroy -= OnDestroy;
				lunyObject.OnReady -= OnReady;
				lunyObject.OnEnable -= OnEnable;
				lunyObject.OnDisable -= OnDisable;
			}

			private void RunScheduledForEvent(LunyObjectEvent objectEvent)
			{
				var sequences = _context.Scheduler.GetSequences(objectEvent);
				if (sequences != null)
					LunyLogger.LogInfo($"Running {nameof(objectEvent)}: {_context} ...", _objectLifecycle);

				LunyScriptRunner.Run(sequences, _context);
			}

			private void UnscheduleOnceOnlyEvent(LunyObjectEvent objectEvent)
			{
				if (objectEvent == LunyObjectEvent.OnCreate || objectEvent == LunyObjectEvent.OnReady)
				{
					// event never fires again for this object
					_context.Scheduler.Unschedule(objectEvent);

					if (objectEvent == LunyObjectEvent.OnCreate)
						_context.LunyObject.OnCreate -= OnCreate;
					else if (objectEvent == LunyObjectEvent.OnReady)
						_context.LunyObject.OnReady -= OnReady;
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
				_context.Coroutines.OnObjectDestroyed(_context);
				_objectLifecycle.Unregister(_context);
			}

			private void OnReady()
			{
				RunScheduledForEvent(LunyObjectEvent.OnReady);
				UnscheduleOnceOnlyEvent(LunyObjectEvent.OnReady);
			}

			private void OnEnable()
			{
				ThrowIfAlreadyProcessingEnableDisableEvent(_context);
				SetProcessingEnableDisableReentryLock(true);
				RunScheduledForEvent(LunyObjectEvent.OnEnable);
				SetProcessingEnableDisableReentryLock(false);
			}

			private void OnDisable()
			{
				ThrowIfAlreadyProcessingEnableDisableEvent(_context);
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
			private void ThrowIfAlreadyProcessingEnableDisableEvent(LunyScriptContext context)
			{
#if DEBUG || LUNYSCRIPT_DEBUG
				// Safeguard against infinite loops (OnEnable toggles to disabled, which triggers OnDisable, etc.)
				if (_processingEnableDisableReentryLock)
				{
					throw new LunyScriptException("Disabling in When.Enabled while ALSO enabling in When.Disabled is not allowed " +
					                              $"(would cause an infinite loop). Script: {context}");
				}
#endif
			}
		}
	}
}
