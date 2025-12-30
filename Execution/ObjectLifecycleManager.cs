using Luny.Proxies;
using LunyScript.Exceptions;
using LunyScript.Registries;
using System;
using System.Collections.Generic;

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
		internal void RegisterObject(ILunyObject lunyObject, ScriptContext context)
		{
			if (lunyObject is LunyObject lunyObjectImpl)
			{
				// TODO: OnCreate
				lunyObjectImpl.OnEnable = () => HandleEnabledStateChanged(context, true);
				lunyObjectImpl.OnDisable = () => HandleEnabledStateChanged(context, false);
				lunyObjectImpl.OnDestroy = () => HandleDestroy(context);
			}
		}

		/// <summary>
		/// Unregisters lifecycle hooks from a LunyObject.
		/// Called during context cleanup or shutdown.
		/// </summary>
		internal void UnregisterObject(ILunyObject lunyObject)
		{
			if (lunyObject is LunyObject lunyObjectImpl)
			{
				lunyObjectImpl.OnEnable = null;
				lunyObjectImpl.OnDisable = null;
				lunyObjectImpl.OnDestroy = null;
			}
		}

		/// <summary>
		/// Queues a context for destruction at end of frame.
		/// Immediately removes context from registry (no longer findable).
		/// </summary>
		internal void QueueForDestruction(ScriptContext context)
		{
			_destroyQueue.Enqueue(context);
			_contextRegistry.Unregister(context.LunyObject.LunyID);
		}

		/// <summary>
		/// Processes queued destructions at end of frame.
		/// Destroys native engine objects and cleans up lifecycle hooks.
		/// </summary>
		internal void OnEndOfFrame()
		{
			while (_destroyQueue.Count > 0)
			{
				var lunyObject = (LunyObject)_destroyQueue.Dequeue().LunyObject;
				if (lunyObject != null)
				{
					if (lunyObject.IsValid)
						throw new InvalidOperationException($"{lunyObject} queued for destruction but still valid");

					lunyObject.DestroyNativeObject();
					UnregisterObject(lunyObject);
				}
			}
		}

		private void HandleEnabledStateChanged(ScriptContext context, Boolean enabled)
		{
			// TODO send this event down to children too ...

			// Safeguard against infinite loops (OnEnable toggles to disabled, which triggers OnDisable, etc.)
			if (_processingEnableDisable)
			{
				_processingEnableDisable = false;
				throw new LunyScriptException(
					$"Disabling in When.Enabled while ALSO enabling in When.Disabled is not allowed (infinite loop). Script: {context}");
			}

			_processingEnableDisable = true;
			try
			{
				if (enabled)
					LunyScriptRunner.RunObjectEnabled(context);
				else
					LunyScriptRunner.RunObjectDisabled(context);
			}
			finally
			{
				_processingEnableDisable = false;
			}
		}

		private void HandleDestroy(ScriptContext context)
		{
			LunyScriptRunner.RunObjectDestroyed(context);
			QueueForDestruction(context);
		}
	}
}
