using Luny;
using Luny.Engine.Bridge;
using Luny.Engine.Bridge.Enums;
using LunyScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript.Execution
{
	internal interface ILunyScriptLifecycle {}

	/// <summary>
	/// Manages object lifecycle events by attaching hooks to LunyObjects and handling event dispatch.
	/// Coordinates enable/disable state changes and deferred object destruction.
	/// </summary>
	internal sealed class LunyScriptLifecycle : ILunyScriptLifecycle
	{
		[NotNull] private readonly LunyScriptContextRegistry _contexts;
		private readonly Dictionary<LunyScriptContext, LifecycleSubscriber> _subscribers = new();

		internal LunyScriptLifecycle(LunyScriptContextRegistry contextRegistry) =>
			_contexts = contextRegistry ?? throw new ArgumentNullException(nameof(contextRegistry));

		~LunyScriptLifecycle() => LunyLogger.LogInfo($"finalized {GetHashCode()}", this);

		/// <summary>
		/// Registers lifecycle hooks on a LunyObject for the given context.
		/// Called during ScriptContext construction.
		/// </summary>
		internal void Register(LunyScriptContext context)
		{
			var subscriber = new LifecycleSubscriber(this, context);
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
			var runnables = context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnFixedStep);
			if (runnables != null)
				LunyScriptRunner.Run(runnables, context);
		}

		public void OnUpdate(Double deltaTime, LunyScriptContext context)
		{
			var runnables = context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnUpdate);
			if (runnables != null)
				LunyScriptRunner.Run(runnables, context);
		}

		public void OnLateUpdate(Double deltaTime, LunyScriptContext context)
		{
			var runnables = context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnLateUpdate);
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

		private sealed class LifecycleSubscriber
		{
			private readonly LunyScriptLifecycle _lifecycle;
			private readonly LunyScriptContext _context;
			private Boolean _processingEnableDisable;

			public LifecycleSubscriber(LunyScriptLifecycle lifecycle, LunyScriptContext context)
			{
				_lifecycle = lifecycle;
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
				var runnables = _context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnCreate);
				if (runnables != null)
				{
					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnCreate)}: {_context} ...", _lifecycle);
					LunyScriptRunner.Run(runnables, _context);
				}

				_context.LunyObject.OnCreate -= OnCreate; // never fires again
			}

			private void OnDestroy()
			{
				var runnables = _context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnDestroy);
				if (runnables != null)
				{
					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnDestroy)}: {_context.LunyObject} ...", _lifecycle);
					LunyScriptRunner.Run(runnables, _context);
				}

				UnregisterAllCallbacks(); // no more events
				_lifecycle.UnregisterSubscriber(_context);
				_lifecycle.UnregisterContext(_context);
			}

			private void OnReady()
			{
				var runnables = _context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnReady);
				if (runnables != null)
				{
					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnReady)}: {_context.LunyObject} ...", _lifecycle);
					LunyScriptRunner.Run(runnables, _context);
					_context.Scheduler.Clear(LunyObjectLifecycleEvents.OnReady);
				}

				_context.LunyObject.OnReady -= OnReady; // never fires again
			}

			private void OnEnable()
			{
				if (_context.Scheduler.IsObserving(LunyObjectLifecycleEvents.OnEnable))
				{
					ThrowIfAlreadyProcessingEnableDisableEvent(_context);
					_processingEnableDisable = true;

					var runnables = _context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnEnable);
					if (runnables != null)
					{
						LunyLogger.LogInfo($"Running {nameof(LunyObject.OnEnable)}: {_context} ...", _lifecycle);
						LunyScriptRunner.Run(runnables, _context);
					}

					_processingEnableDisable = false;
				}
			}

			private void OnDisable()
			{
				if (_context.Scheduler.IsObserving(LunyObjectLifecycleEvents.OnDisable))
				{
					ThrowIfAlreadyProcessingEnableDisableEvent(_context);
					_processingEnableDisable = true;

					var runnables = _context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnDisable);
					if (runnables != null)
					{
						LunyLogger.LogInfo($"Running {nameof(LunyObject.OnDisable)}: {_context} ...", _lifecycle);
						LunyScriptRunner.Run(runnables, _context);
					}

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
