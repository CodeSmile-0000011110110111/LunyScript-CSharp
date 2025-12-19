using System;
using LunyScript.Blocks;
using LunyScript.Runnables;

namespace LunyScript
{
	/// <summary>
	/// Abstract base class for all LunyScripts.
	/// Provides the API interface for beginner-friendly visual scripting in C#.
	/// Users inherit from this class and implement Build() to construct their script logic.
	/// </summary>
	public abstract class LunyScript
	{
		private RunContext _context;

		internal void Initialize(RunContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		// User-facing API: Variables
		protected Variables Variables => _context.Variables;
		protected Variables GlobalVariables => _context.GlobalVariables;
		protected Variables InspectorVariables => _context.InspectorVariables;

		// User-facing API: Block factory methods
		protected static LogMessageBlock Log(String message) => new LogMessageBlock(message);
		protected ActionBlock Do(Action action) => new ActionBlock(_ => action());

		// User-facing API: Runnable registration
		protected void OnUpdate(params IBlock[] blocks)
		{
			var runnable = new RunnableSequence(blocks);
			_context.UpdateRunnables.Add(runnable);
		}

		protected void OnFixedStep(params IBlock[] blocks)
		{
			var runnable = new RunnableSequence(blocks);
			_context.FixedStepRunnables.Add(runnable);
		}

		protected void OnLateUpdate(params IBlock[] blocks)
		{
			var runnable = new RunnableSequence(blocks);
			_context.LateUpdateRunnables.Add(runnable);
		}

		/// <summary>
		/// Called once when the script is initialized.
		/// Users construct their blocks and register them for execution here.
		/// </summary>
		public abstract void Build();
	}
}
