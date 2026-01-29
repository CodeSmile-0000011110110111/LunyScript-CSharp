using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public abstract partial class LunyScript
	{
		/// <summary>
		/// Conditional execution: If(conditions).Then(blocks).ElseIf(conditions).Then(blocks).Else(blocks);
		/// Multiple conditions are implicitly AND combined.
		/// </summary>
		public IfBlockBuilder If(params IScriptConditionBlock[] conditions) => new(conditions);

		/// <summary>
		/// Loop execution: While(conditions).Do(blocks);
		/// Multiple conditions are implicitly AND combined.
		/// </summary>
		public WhileBlockBuilder While(params IScriptConditionBlock[] conditions) => new(conditions);

		/// <summary>
		/// For loop (1-based index): For(limit).Do(blocks);
		/// Starts at 1 and increments by 1 until limit is reached (inclusive).
		/// </summary>
		public ForBlockBuilder For(Int32 limit) => new(limit);

		/// <summary>
		/// For loop (1-based index): For(limit, step).Do(blocks);
		/// If step > 0: starts at 1 and increments by step until limit is reached.
		/// If step < 0: starts at limit and decrements by step until 1 is reached.
		/// </summary>
		public ForBlockBuilder For(Int32 limit, Int32 step) => new(limit, step);

		/// <summary>
		/// Logical AND: Returns true if all conditions are true.
		/// </summary>
		public IScriptConditionBlock AND(params IScriptConditionBlock[] conditions) => AndBlock.Create(conditions);

		/// <summary>
		/// Logical OR: Returns true if at least one condition is true.
		/// </summary>
		public IScriptConditionBlock OR(params IScriptConditionBlock[] conditions) => OrBlock.Create(conditions);

		/// <summary>
		/// Logical NOT: Returns the inverse of the condition.
		/// </summary>
		public IScriptConditionBlock NOT(IScriptConditionBlock condition) => NotBlock.Create(condition);
	}
}
