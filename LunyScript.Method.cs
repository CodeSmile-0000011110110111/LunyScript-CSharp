using LunyScript.Blocks;
using LunyScript.Execution;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		public readonly struct MethodApi
		{
			private readonly ILunyScript _script;
			internal MethodApi(ILunyScript script) => _script = script;

			/// <summary>
			/// Run overload whose method/lambda receives the ILunyScriptContext instance.
			/// </summary>
			/// <param name="action"></param>
			/// <returns></returns>
			public ILunyScriptBlock Run(Action<ILunyScriptContext> action) => RunActionBlock.Create(action);

			/// <summary>
			/// Runs the contained method/lambda when this block executes.
			/// </summary>
			/// <remarks>
			/// Intended for quick prototyping and testing only: lambdas are not reusable building blocks.
			///
			/// Prefer to convert "Run" code into a custom IBlock class after its initial development and testing,
			/// If not that, use named methods rather than lambdas - this ensures the block-based code continues to read like intent.
			///
			///		// Even a single-line lambda adds notable 'syntax noise' (worse for multi-line lambdas):
			/// 	OnUpdate(Run(() => LunyLogger.LogInfo("custom log inline")));
			///
			///		// A named method is much cleaner, and re-usable in the same script:
			///		OnUpdate(Run(MyLog));
			///		private Action MyLog() => () => LunyLogger.LogInfo("custom log method");
			///
			///		// C# extension methods for LunyScript also work nice but require the 'this' prefix:
			///		OnUpdate(Run(this.MyLog()));
			///		public static MyLunyScriptExtensions
			///		{
			///			public static Action MyLog(this LunyScript ls) =>
			///				() => LunyLogger.LogInfo("custom log ext method");
			///		}
			///
			///		// Best: Create your own static factory class returning IBlock instances.
			///		// The block's Execute method has access to the context (object reference, variables, etc.).
			///		// The static Create() method helps to later adapt the creation code without having to modify callers.
			///		OnUpdate(MyBlocks.MyLog());
			///		internal static MyBlocks
			///		{
			///			public static ILunyScriptBlock MyLog() => MyLogBlock.Create();
			///			internal sealed class MyLogBlock : ILunyScriptBlock
			///			{
			///				public static ILunyScriptBlock Create() => new MyLogBlock();
			///				public void Execute(ILunyScriptContext context) => LunyLogger.LogInfo("custom log block");
			///			}
			///		}
			/// </remarks>
			/// <param name="action"></param>
			/// <returns></returns>
			public ILunyScriptBlock Run(Action action) => RunActionBlock.Create(_ => action());
		}
	}
}
