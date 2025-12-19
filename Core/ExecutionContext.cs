using System;
using System.Text;
using Luny.Core;
using Luny.Proxies;

namespace LunyScript
{
	/// <summary>
	/// Execution context for a LunyScript instance operating on a specific object.
	/// Contains the script ID, object reference, variables, and transient state.
	/// </summary>
	public sealed class ExecutionContext
	{
		/// <summary>
		/// The ID of the script definition this context executes.
		/// </summary>
		public ScriptID ScriptID { get; }

		/// <summary>
		/// The engine object/node this script operates on.
		/// </summary>
		public LunyObject Object { get; }

		/// <summary>
		/// Per-object variables for this script instance.
		/// </summary>
		public Variables Variables { get; }

		/// <summary>
		/// Transient state storage for stateless sequences (future hot-reload optimization).
		/// Keys should be namespaced to avoid collisions (e.g., "CollisionBlock.didCollide").
		/// </summary>
		public Variables TransientState { get; }

		public ExecutionContext(ScriptID scriptID, LunyObject obj)
		{
			ScriptID = scriptID;
			Object = obj ?? throw new ArgumentNullException(nameof(obj));
			Variables = new Variables();
			TransientState = new Variables();
		}

		/// <summary>
		/// Whether the underlying object is still valid (not destroyed).
		/// </summary>
		public Boolean IsValid => Object.IsValid;

		public override String ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"ExecutionContext: {ScriptID} -> {Object}");
			sb.AppendLine($"  Valid: {IsValid}");
			
			if (Variables.Count > 0)
			{
				sb.AppendLine($"  {Variables}");
			}
			
			if (TransientState.Count > 0)
			{
				sb.AppendLine($"  Transient: {TransientState}");
			}

			return sb.ToString();
		}
	}
}
