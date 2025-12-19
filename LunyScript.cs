using System;

namespace LunyScript
{
	/// <summary>
	/// Abstract base class for all LunyScripts.
	/// Provides the API interface for beginner-friendly visual scripting in C#.
	/// Users inherit from this class and implement OnStartup() to construct their script logic.
	/// </summary>
	public abstract class LunyScript
	{
		// NOTE: Internal properties are hidden from users but accessible to framework.
		// ExecutionContext and ScriptID should NOT be exposed to script authors.
		// Keep this class minimal - it's just the API shell for users.

		/// <summary>
		/// Called once when the script is initialized.
		/// Users build their sequences, state machines, and behavior trees here.
		/// </summary>
		public abstract void OnStartup();

		// Future API surface will be added here as protected properties:
		// protected When When { get; }
		// protected Audio Audio { get; }
		// protected Input Input { get; }
		// etc.
	}
}
