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
		private void Unregister(LunyScriptContext context)
		{
			_subscribers.Remove(context);
			_contexts.Unregister(context);
		}

		public void OnFixedStep(Double fixedDeltaTime, LunyScriptContext context)
		{
			if (context.LunyObject.IsEnabled)
			{
				var sequences = context.Scheduler.GetSequences(LunyObjectEvent.OnFixedStep);
				if (sequences != null)
					LunyScriptRunner.Run(sequences, context);
			}
		}

		public void OnUpdate(Double deltaTime, LunyScriptContext context)
		{
			if (context.LunyObject.IsEnabled)
			{
				var sequences = context.Scheduler.GetSequences(LunyObjectEvent.OnUpdate);
				if (sequences != null)
					LunyScriptRunner.Run(sequences, context);

				OnIntervalUpdate(deltaTime, context);
			}
		}

		internal void OnIntervalUpdate(Double deltaTime, LunyScriptContext context)
		{
			var lunyObject = context.LunyObject;
			if (!lunyObject.IsEnabled)
				return;

			var intervals = context.Scheduler.IntervalSequences;
			if (intervals == null || intervals.Count == 0)
				return;

			// TODO: Junie implemented this without enough context, this needs a refactor
			throw new NotImplementedException(nameof(OnIntervalUpdate));

			/*
			_intervalTimers ??= new List<Double>();
			while (_intervalTimers.Count < intervals.Count)
				_intervalTimers.Add(0);

			for (int i = 0; i < intervals.Count; i++)
			{
				_intervalTimers[i] += deltaTime;
				var target = intervals[i].interval.TotalSeconds;
				if (_intervalTimers[i] >= target)
				{
					_intervalTimers[i] %= target;
					LunyScriptRunner.Run(new[] { intervals[i].runnable }, this);
				}
			}
		*/
		}

		public void OnLateUpdate(Double deltaTime, LunyScriptContext context)
		{
			if (context.LunyObject.IsEnabled)
			{
				var sequences = context.Scheduler.GetSequences(LunyObjectEvent.OnLateUpdate);
				if (sequences != null)
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

			private void RunScheduledForEvent(LunyObjectEvent objectEvent)
			{
				var sequences = _context.Scheduler.GetSequences(objectEvent);
				if (sequences != null)
				{
					LunyLogger.LogInfo($"Running {nameof(objectEvent)}: {_context} ...", _objectEventHandler);
					LunyScriptRunner.Run(sequences, _context);
				}
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
				_objectEventHandler.Unregister(_context);
			}

			private void OnReady()
			{
				RunScheduledForEvent(LunyObjectEvent.OnReady);
				UnscheduleOnceOnlyEvent(LunyObjectEvent.OnReady);
			}

			private void OnEnable()
			{
				ThrowIfAlreadyProcessingEnableDisableEvent(_context);
				_processingEnableDisable = true;
				RunScheduledForEvent(LunyObjectEvent.OnEnable);
				_processingEnableDisable = false;
			}

			private void OnDisable()
			{
				ThrowIfAlreadyProcessingEnableDisableEvent(_context);
				_processingEnableDisable = true;
				RunScheduledForEvent(LunyObjectEvent.OnDisable);
				_processingEnableDisable = false;
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
