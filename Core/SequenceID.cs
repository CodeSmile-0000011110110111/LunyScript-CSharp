using System;

namespace Luny.Core
{
	/// <summary>
	/// Unique identifier for a sequence/FSM/BT instance.
	/// Sequential integers for deterministic ordering and debugging.
	/// </summary>
	public readonly struct SequenceID : IEquatable<SequenceID>, IComparable<SequenceID>
	{
		private static Int32 _nextID = 1;

		public readonly Int32 Value;

		private SequenceID(Int32 value) => Value = value;

		/// <summary>
		/// Generates a new unique SequenceID.
		/// </summary>
		public static SequenceID Generate() => new SequenceID(_nextID++);

		public Boolean Equals(SequenceID other) => Value == other.Value;
		public override Boolean Equals(Object obj) => obj is SequenceID other && Equals(other);
		public override Int32 GetHashCode() => Value;
		public Int32 CompareTo(SequenceID other) => Value.CompareTo(other.Value);
		public override String ToString() => $"SequenceID:{Value}";

		public static Boolean operator ==(SequenceID left, SequenceID right) => left.Equals(right);
		public static Boolean operator !=(SequenceID left, SequenceID right) => !left.Equals(right);
	}
}
