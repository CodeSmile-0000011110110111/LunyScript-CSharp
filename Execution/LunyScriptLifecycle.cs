using Luny;
using Luny.Engine.Bridge;
using Luny.Engine.Events;
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
		private Boolean _processingEnableDisable;

		internal LunyScriptLifecycle(LunyScriptContextRegistry contextRegistry) =>
			_contexts = contextRegistry ?? throw new ArgumentNullException(nameof(contextRegistry));

		~LunyScriptLifecycle() => LunyLogger.LogInfo($"finalized {GetHashCode()}", this);

		/// <summary>
		/// Registers lifecycle hooks on a LunyObject for the given context.
		/// Called during ScriptContext construction.
		/// </summary>
		internal void Register(LunyScriptContext context)
		{
			if (context.LunyObject is LunyObject lunyObjectImpl)
			{
				var subscriber = new LifecycleSubscriber(this, context, lunyObjectImpl);
				_subscribers[context] = subscriber;
				subscriber.RegisterCallbacks();
			}
		}

		/// <summary>
		/// Unregisters lifecycle hooks from a LunyObject.
		/// Called during context cleanup or shutdown.
		/// </summary>
		internal void Unregister(LunyScriptContext context)
		{
			context.Scheduler.Clear();

			if (_subscribers.Remove(context, out var subscriber))
				subscriber.UnregisterCallbacks();
		}

		private void UnregisterAllSubscribers()
		{
			foreach (var pair in _subscribers)
			{
				var context = pair.Key;
				context.Scheduler.Clear();

				var subscriber = pair.Value;
				subscriber.UnregisterCallbacks();
			}

			_subscribers.Clear();
		}

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

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void SafeguardAgainstInfiniteEnableDisableCycle(LunyScriptContext context)
		{
#if DEBUG || LUNYSCRIPT_DEBUG
			// Safeguard against infinite loops (OnEnable toggles to disabled, which triggers OnDisable, etc.)
			if (_processingEnableDisable)
			{
				_processingEnableDisable = false;
				throw new LunyScriptException("Disabling in When.Enabled while ALSO enabling in When.Disabled is not allowed " +
				                              $"(would cause an infinite loop). Script: {context}");
			}
#endif
		}

		public void Shutdown()
		{
			UnregisterAllSubscribers();

			_subscribers.Clear();
		}

		private sealed class LifecycleSubscriber
		{
			private readonly LunyScriptLifecycle _parent;
			private readonly LunyScriptContext _context;
			private readonly LunyObject _lunyObject;

			public LifecycleSubscriber(LunyScriptLifecycle parent, LunyScriptContext context, LunyObject lunyObject)
			{
				_parent = parent;
				_context = context;
				_lunyObject = lunyObject;
			}

			public void RegisterCallbacks()
			{
				_lunyObject.OnCreate += OnCreate;
				_lunyObject.OnDestroy += OnDestroy;
				_lunyObject.OnEnable += OnEnable;
				_lunyObject.OnDisable += OnDisable;
				_lunyObject.OnReady += OnReady;
			}

			public void UnregisterCallbacks()
			{
				_lunyObject.OnCreate -= OnCreate;
				_lunyObject.OnDestroy -= OnDestroy;
				_lunyObject.OnEnable -= OnEnable;
				_lunyObject.OnDisable -= OnDisable;
				_lunyObject.OnReady -= OnReady;
			}

			private void OnCreate()
			{
				var runnables = _context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnCreate);
				if (runnables != null)
				{
					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnCreate)}: {_context} ...", _parent);
					LunyScriptRunner.Run(runnables, _context);
				}
			}

			private void OnDestroy()
			{
				if (_context.DidRunOnDestroy)
					throw new LunyScriptException($"{nameof(LunyObject.OnDestroy)} was invoked again: {_context}");

				var runnables = _context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnDestroy);
				if (runnables != null)
				{
					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnDestroy)}: {_context.LunyObject} ...", _parent);
					LunyScriptRunner.Run(runnables, _context);
				}

				_parent._contexts.Unregister(_context);
			}

			private void OnEnable()
			{
				if (_context.Scheduler.IsObserving(LunyObjectLifecycleEvents.OnEnable))
				{
					_parent.SafeguardAgainstInfiniteEnableDisableCycle(_context);
					_parent._processingEnableDisable = true;
					var runnables = _context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnEnable);
					if (runnables != null)
					{
						LunyLogger.LogInfo($"Running {nameof(LunyObject.OnEnable)}: {_context} ...", _parent);
						LunyScriptRunner.Run(runnables, _context);
					}

					_parent._processingEnableDisable = false;
				}
			}

			private void OnDisable()
			{
				if (_context.Scheduler.IsObserving(LunyObjectLifecycleEvents.OnDisable))
				{
					_parent.SafeguardAgainstInfiniteEnableDisableCycle(_context);

					_parent._processingEnableDisable = true;
					var runnables = _context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnDisable);
					if (runnables != null)
					{
						LunyLogger.LogInfo($"Running {nameof(LunyObject.OnDisable)}: {_context} ...", _parent);
						LunyScriptRunner.Run(runnables, _context);
					}
					_parent._processingEnableDisable = false;
				}
			}

			private void OnReady()
			{
				var runnables = _context.Scheduler.GetScheduled(LunyObjectLifecycleEvents.OnReady);
				if (runnables != null)
				{
					LunyLogger.LogInfo($"Running {nameof(LunyObject.OnReady)}: {_context.LunyObject} ...", _parent);
					LunyScriptRunner.Run(runnables, _context);
					_context.Scheduler.Clear(LunyObjectLifecycleEvents.OnReady);
				}
			}
		}
	}
}
