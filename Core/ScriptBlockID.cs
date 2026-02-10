using System;

namespace LunyScript
{
	/// <summary>
	/// Unique identifier for a Sequence/FSM/BT instance.
	/// Sequential integers for deterministic ordering and debugging.
	/// </summary>
	public readonly struct ScriptBlockID : IEquatable<ScriptBlockID>, IComparable<ScriptBlockID>
	{
		private const Int32 StartID = 1;
		private static Int32 s_NextID = StartID;
		internal static void Reset() => s_NextID = StartID;

		public readonly Int32 Value;

		private ScriptBlockID(Int32 value) => Value = value;

		/// <summary>
		/// Generates a new unique SequenceID.
		/// </summary>
		public static ScriptBlockID Generate() => new(s_NextID++);

		public Boolean Equals(ScriptBlockID other) => Value == other.Value;
		public override Boolean Equals(Object obj) => obj is ScriptBlockID other && Equals(other);
		public override Int32 GetHashCode() => Value;
		public Int32 CompareTo(ScriptBlockID other) => Value.CompareTo(other.Value);
		public override String ToString() => $"SequenceID:{Value}";

		public static Boolean operator ==(ScriptBlockID left, ScriptBlockID right) => left.Equals(right);
		public static Boolean operator !=(ScriptBlockID left, ScriptBlockID right) => !left.Equals(right);
	}
}
