using System;

namespace LunyScript
{
	/// <summary>
	/// Unique identifier for a runnable sequence/FSM/BT instance.
	/// Sequential integers for deterministic ordering and debugging.
	/// </summary>
	public readonly struct RunnableID : IEquatable<RunnableID>, IComparable<RunnableID>
	{
		private const Int32 StartID = 1;
		private static Int32 s_NextID = StartID;
		internal static void Reset() => s_NextID = StartID;

		public readonly Int32 Value;

		private RunnableID(Int32 value) => Value = value;

		/// <summary>
		/// Generates a new unique SequenceID.
		/// </summary>
		public static RunnableID Generate() => new(s_NextID++);

		public Boolean Equals(RunnableID other) => Value == other.Value;
		public override Boolean Equals(Object obj) => obj is RunnableID other && Equals(other);
		public override Int32 GetHashCode() => Value;
		public Int32 CompareTo(RunnableID other) => Value.CompareTo(other.Value);
		public override String ToString() => $"SequenceID:{Value}";

		public static Boolean operator ==(RunnableID left, RunnableID right) => left.Equals(right);
		public static Boolean operator !=(RunnableID left, RunnableID right) => !left.Equals(right);
	}
}
