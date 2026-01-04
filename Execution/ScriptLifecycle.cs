using Luny;
using Luny.Diagnostics;
using Luny.Proxies;
using LunyScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript.Execution
{
	/// <summary>
	/// Manages object lifecycle events by attaching hooks to LunyObjects and handling event dispatch.
	/// Coordinates enable/disable state changes and deferred object destruction.
	/// </summary>
	internal sealed class ScriptLifecycle
	{
		[NotNull] private readonly ScriptContextRegistry _contexts;
		private Boolean _processingEnableDisable;

		internal ScriptLifecycle(ScriptContextRegistry contextRegistry) =>
			_contexts = contextRegistry ?? throw new ArgumentNullException(nameof(contextRegistry));

		~ScriptLifecycle() => LunyLogger.LogInfo($"finalized {GetHashCode()}", this);

		/// <summary>
		/// Registers lifecycle hooks on a LunyObject for the given context.
		/// Called during ScriptContext construction.
		/// </summary>
		internal void RegisterCallbacks(ScriptContext context)
		{
			if (context.LunyObject is LunyObject lunyObjectImpl)
			{
				// setup receiving the lifecycle events for which the script has scheduled runnables

				// OnCreate (always observed for internal processing)
				{
					lunyObjectImpl.OnCreate += OnCreate;

					void OnCreate()
					{
						var runnables = context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnCreate);
						if (runnables != null)
						{
							LunyLogger.LogInfo($"Running {nameof(LunyObject.OnCreate)}: {context} ...", this);
							LunyScriptRunner.Run(runnables, context);
						}

						LunyEngine.Instance.Lifecycle.EnqueueReady(context.LunyObject);
					}
				}

				// OnDestroy (always observed for internal processing)
				{
					lunyObjectImpl.OnDestroy += OnDestroy;

					void OnDestroy()
					{
						if (context.DidRunOnDestroy)
							throw new LunyScriptException($"{nameof(LunyObject.OnDestroy)} was invoked again: {context}");

						var runnables = context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnDestroy);
						if (runnables != null)
						{
							LunyLogger.LogInfo($"Running {nameof(LunyObject.OnDestroy)}: {context.LunyObject} ...", this);
							LunyScriptRunner.Run(runnables, context);
						}

						_contexts.Unregister(context);
					}
				}

				// OnEnable
				{
					lunyObjectImpl.OnEnable += OnEnable;

					void OnEnable()
					{
						// TODO send this event down to children too ...

						if (context.Scheduler.IsObserving(ObjectLifecycleEvents.OnEnable))
						{
							SafeguardAgainstInfiniteEnableDisableCycle(context);
							_processingEnableDisable = true;
							var runnables = context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnEnable);
							if (runnables != null)
							{
								LunyLogger.LogInfo($"Running {nameof(LunyObject.OnEnable)}: {context} ...", this);
								LunyScriptRunner.Run(runnables, context);
							}

							_processingEnableDisable = false;
						}
					}
				}

				// OnDisable
				if (context.Scheduler.IsObserving(ObjectLifecycleEvents.OnDisable))
				{
					lunyObjectImpl.OnDisable += OnDisable;

					void OnDisable()
					{
						// TODO send this event down to children too ...

						SafeguardAgainstInfiniteEnableDisableCycle(context);

						_processingEnableDisable = true;
						var runnables = context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnDisable);
						if (runnables != null)
						{
							LunyLogger.LogInfo($"Running {nameof(LunyObject.OnDisable)}: {context} ...", this);
							LunyScriptRunner.Run(runnables, context);
						}
						_processingEnableDisable = false;
					}
				}

				// OnReady
				if (context.Scheduler.IsObserving(ObjectLifecycleEvents.OnReady))
				{
					lunyObjectImpl.OnReady += OnReady;

					void OnReady()
					{
						var runnables = context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnReady);
						if (runnables != null)
						{
							LunyLogger.LogInfo($"Running {nameof(LunyObject.OnReady)}: {context.LunyObject} ...", this);
							LunyScriptRunner.Run(runnables, context);
							context.Scheduler.Clear(ObjectLifecycleEvents.OnReady);
						}
					}
				}
			}
		}

		/// <summary>
		/// Unregisters lifecycle hooks from a LunyObject.
		/// Called during context cleanup or shutdown.
		/// </summary>
		internal void UnregisterCallbacks(ScriptContext context)
		{
			context.Scheduler.Clear();

			if (context.LunyObject is LunyObject lunyObjectImpl)
			{
				// Events cannot be cleared from outside the class.
				// This needs a proper subscription management in Phase 2.
				// For now, we just clear the scheduler.
			}
		}

		public void OnFixedStep(Double fixedDeltaTime, ScriptContext context)
		{
			var runnables = context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnFixedStep);
			if (runnables != null)
				LunyScriptRunner.Run(runnables, context);
		}

		public void OnUpdate(Double deltaTime, ScriptContext context)
		{
			var runnables = context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnUpdate);
			if (runnables != null)
				LunyScriptRunner.Run(runnables, context);
		}

		public void OnLateUpdate(Double deltaTime, ScriptContext context)
		{
			var runnables = context.Scheduler.GetScheduled(ObjectLifecycleEvents.OnLateUpdate);
			if (runnables != null)
				LunyScriptRunner.Run(runnables, context);
		}

		[Conditional("DEBUG")] [Conditional("LUNYSCRIPT_DEBUG")]
		private void SafeguardAgainstInfiniteEnableDisableCycle(ScriptContext context)
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
		}
	}
}
