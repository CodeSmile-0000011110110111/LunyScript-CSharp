using System;

namespace LunyScript
{
	/// <summary>
	/// Unique identifier for a runnable sequence/FSM/BT instance.
	/// Sequential integers for deterministic ordering and debugging.
	/// </summary>
	public readonly struct LunyScriptRunID : IEquatable<LunyScriptRunID>, IComparable<LunyScriptRunID>
	{
		private const Int32 StartID = 1;
		private static Int32 s_NextID = StartID;
		internal static void Reset() => s_NextID = StartID;

		public readonly Int32 Value;

		private LunyScriptRunID(Int32 value) => Value = value;

		/// <summary>
		/// Generates a new unique SequenceID.
		/// </summary>
		public static LunyScriptRunID Generate() => new(s_NextID++);

		public Boolean Equals(LunyScriptRunID other) => Value == other.Value;
		public override Boolean Equals(Object obj) => obj is LunyScriptRunID other && Equals(other);
		public override Int32 GetHashCode() => Value;
		public Int32 CompareTo(LunyScriptRunID other) => Value.CompareTo(other.Value);
		public override String ToString() => $"SequenceID:{Value}";

		public static Boolean operator ==(LunyScriptRunID left, LunyScriptRunID right) => left.Equals(right);
		public static Boolean operator !=(LunyScriptRunID left, LunyScriptRunID right) => !left.Equals(right);
	}
}
