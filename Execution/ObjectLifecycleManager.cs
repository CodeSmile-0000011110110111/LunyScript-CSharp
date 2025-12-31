using Luny.Proxies;
using LunyScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LunyScript.Execution
{
	/// <summary>
	/// Manages object lifecycle events by attaching hooks to LunyObjects and handling event dispatch.
	/// Coordinates enable/disable state changes and deferred object destruction.
	/// </summary>
	internal sealed class ObjectLifecycleManager
	{
		private readonly ScriptContextRegistry _contextRegistry;
		private readonly Queue<ScriptContext> _destroyQueue = new();
		private Boolean _processingEnableDisable;

		internal ObjectLifecycleManager(ScriptContextRegistry contextRegistry) =>
			_contextRegistry = contextRegistry ?? throw new ArgumentNullException(nameof(contextRegistry));

		/// <summary>
		/// Registers lifecycle hooks on a LunyObject for the given context.
		/// Called during ScriptContext construction.
		/// </summary>
		internal void Register(ScriptContext context, ILunyObject lunyObject)
		{
			if (lunyObject is LunyObject lunyObjectImpl)
			{
				// "update" events are handled by ScriptRunner
				lunyObjectImpl.OnCreate = () => HandleCreateEvent(context);
				lunyObjectImpl.OnDestroy = () => HandleDestroyEvent(context);
				lunyObjectImpl.OnReady = () => HandleReadyEvent(context);
				lunyObjectImpl.OnEnable = () => HandleOnEnableEvent(context);
				lunyObjectImpl.OnDisable = () => HandleOnDisableEvent(context);
			}
		}

		/// <summary>
		/// Unregisters lifecycle hooks from a LunyObject.
		/// Called during context cleanup or shutdown.
		/// </summary>
		internal void Unregister(ScriptContext context)
		{
			if (context.LunyObject is LunyObject lunyObjectImpl)
			{
				lunyObjectImpl.OnCreate = null;
				lunyObjectImpl.OnDestroy = null;
				lunyObjectImpl.OnReady = null;
				lunyObjectImpl.OnEnable = null;
				lunyObjectImpl.OnDisable = null;
			}
		}

		/// <summary>
		/// Queues a context for destruction at end of frame.
		/// Immediately removes context from registry (no longer findable).
		/// </summary>
		internal void QueueForDestruction(ScriptContext context)
		{
			_destroyQueue.Enqueue(context);
			_contextRegistry.Unregister(context);
		}

		/// <summary>
		/// Processes queued destructions at end of frame.
		/// Destroys native engine objects and cleans up lifecycle hooks.
		/// </summary>
		internal void OnEndOfFrame()
		{
			while (_destroyQueue.Count > 0)
			{
				var context = _destroyQueue.Dequeue();
				if (context != null)
				{
					var lunyObject = context.LunyObject;
					if (lunyObject.IsValid)
						throw new InvalidOperationException($"{context} queued for destruction but still valid");

					((LunyObject)lunyObject).DestroyNativeObject();
					Unregister(context);
				}
			}
		}

		private void HandleCreateEvent(ScriptContext context) => throw new NotImplementedException();

		private void HandleDestroyEvent(ScriptContext context)
		{
			ScriptRunner.RunObjectDestroyed(context);
			QueueForDestruction(context);
		}

		private void HandleReadyEvent(ScriptContext context) => throw new NotImplementedException();

		private void HandleOnEnableEvent(ScriptContext context)
		{
			// TODO send this event down to children too ...

			SafeguardAgainstInfiniteEnableDisableCycle(context);

			_processingEnableDisable = true;
			ScriptRunner.RunObjectEnabled(context);
			_processingEnableDisable = false;
		}

		private void HandleOnDisableEvent(ScriptContext context)
		{
			// TODO send this event down to children too ...

			SafeguardAgainstInfiniteEnableDisableCycle(context);

			_processingEnableDisable = true;
			ScriptRunner.RunObjectDisabled(context);
			_processingEnableDisable = false;
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
	}
}
