using Luny;
using Luny.Engine.Bridge;
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
	internal sealed class LunyScriptObjectEventHandler : ILunyScriptLifecycleInternal
	{
		[NotNull] private readonly LunyScriptContextRegistry _contexts;
		private readonly Dictionary<LunyScriptContext, ObjectEventSubscriber> _subscribers = new();

		internal LunyScriptObjectEventHandler(LunyScriptContextRegistry contextRegistry) =>
			_contexts = contextRegistry ?? throw new ArgumentNullException(nameof(contextRegistry));

		~LunyScriptObjectEventHandler() => LunyTraceLogger.LogInfoFinalized(this);

		/// <summary>
		/// Registers lifecycle hooks on a LunyObject for the given context.
		/// Called during ScriptContext construction.
		/// </summary>
		internal void Register(LunyScriptContext context)
		{
			var subscriber = new ObjectEventSubscriber(this, context);
			_subscribers[context] = subscriber;
		}

		/// <summary>
		/// Unregisters lifecycle hooks from a LunyObject.
		/// Called during context cleanup or shutdown.
		/// </summary>
		private void UnregisterSubscriber(LunyScriptContext context) => _subscribers.Remove(context);

		private void UnregisterContext(LunyScriptContext context) => _contexts.Unregister(context);

		public void OnFixedStep(Double fixedDeltaTime, LunyScriptContext context)
		{
			var runnables = context.Scheduler.GetScheduled(LunyObjectEvent.OnFixedStep);
			if (runnables != null)
				LunyScriptRunner.Run(runnables, context);
		}

		public void OnUpdate(Double deltaTime, LunyScriptContext context)
		{
			var runnables = context.Scheduler.GetScheduled(LunyObjectEvent.OnUpdate);
			if (runnables != null)
				LunyScriptRunner.Run(runnables, context);
		}

		public void OnLateUpdate(Double deltaTime, LunyScriptContext context)
		{
			var runnables = context.Scheduler.GetScheduled(LunyObjectEvent.OnLateUpdate);
			if (runnables != null)
				LunyScriptRunner.Run(runnables, context);
		}

		public void Shutdown()
		{
			// remove all subscribers and their events
			foreach (var subscriber in _subscribers.Values)
				subscriber.UnregisterAllCallbacks();

			_subscribers.Clear();
		}

		private sealed class ObjectEventSubscriber
		{
			private readonly LunyScriptObjectEventHandler _objectEventHandler;
			private readonly LunyScriptContext _context;
			private Boolean _processingEnableDisable;

			public ObjectEventSubscriber(LunyScriptObjectEventHandler objectEventHandler, LunyScriptContext context)
			{
				_objectEventHandler = objectEventHandler;
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

			private void OnCreate()
			{
				var runnables = _context.Scheduler.GetScheduled(LunyObjectEvent.OnCreate);
				if (runnables != null)
				{
					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnCreate)}: {_context} ...", _objectEventHandler);
					LunyScriptRunner.Run(runnables, _context);
				}

				_context.LunyObject.OnCreate -= OnCreate; // never fires again
			}

			private void OnDestroy()
			{
				var runnables = _context.Scheduler.GetScheduled(LunyObjectEvent.OnDestroy);
				if (runnables != null)
				{
					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnDestroy)}: {_context.LunyObject} ...", _objectEventHandler);
					LunyScriptRunner.Run(runnables, _context);
				}

				UnregisterAllCallbacks(); // no more events
				_objectEventHandler.UnregisterSubscriber(_context);
				_objectEventHandler.UnregisterContext(_context);
			}

			private void OnReady()
			{
				var runnables = _context.Scheduler.GetScheduled(LunyObjectEvent.OnReady);
				if (runnables != null)
				{
					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnReady)}: {_context.LunyObject} ...", _objectEventHandler);
					LunyScriptRunner.Run(runnables, _context);
					_context.Scheduler.Unschedule(LunyObjectEvent.OnReady);
				}

				_context.LunyObject.OnReady -= OnReady; // never fires again
			}

			private void OnEnable()
			{
				var runnables = _context.Scheduler.GetScheduled(LunyObjectEvent.OnEnable);
				if (runnables != null)
				{
					ThrowIfAlreadyProcessingEnableDisableEvent(_context);
					_processingEnableDisable = true;

					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnEnable)}: {_context} ...", _objectEventHandler);
					LunyScriptRunner.Run(runnables, _context);

					_processingEnableDisable = false;
				}
			}

			private void OnDisable()
			{
				var runnables = _context.Scheduler.GetScheduled(LunyObjectEvent.OnDisable);
				if (runnables != null)
				{
					ThrowIfAlreadyProcessingEnableDisableEvent(_context);
					_processingEnableDisable = true;

					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnDisable)}: {_context} ...", _objectEventHandler);
					LunyScriptRunner.Run(runnables, _context);

					_processingEnableDisable = false;
				}
			}

			[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
			private void ThrowIfAlreadyProcessingEnableDisableEvent(LunyScriptContext context)
			{
#if DEBUG || LUNYSCRIPT_DEBUG
				// Safeguard against infinite loops (OnEnable toggles to disabled, which triggers OnDisable, etc.)
				if (_processingEnableDisable)
				{
					throw new LunyScriptException("Disabling in When.Enabled while ALSO enabling in When.Disabled is not allowed " +
					                              $"(would cause an infinite loop). Script: {context}");
				}
#endif
			}
		}
	}
}
