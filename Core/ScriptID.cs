using System;

namespace LunyScript
{
	/// <summary>
	/// Unique identifier for a LunyScript definition (type).
	/// Sequential integers for deterministic ordering and debugging.
	/// </summary>
	public readonly struct ScriptID : IEquatable<ScriptID>, IComparable<ScriptID>
	{
		private static Int32 _nextID = 1;

		public readonly Int32 Value;

		private ScriptID(Int32 value) => Value = value;

		/// <summary>
		/// Generates a new unique ScriptID.
		/// </summary>
		public static ScriptID Generate() => new ScriptID(_nextID++);

		public Boolean Equals(ScriptID other) => Value == other.Value;
		public override Boolean Equals(Object obj) => obj is ScriptID other && Equals(other);
		public override Int32 GetHashCode() => Value;
		public Int32 CompareTo(ScriptID other) => Value.CompareTo(other.Value);
		public override String ToString() => $"ScriptID:{Value}";

		public static Boolean operator ==(ScriptID left, ScriptID right) => left.Equals(right);
		public static Boolean operator !=(ScriptID left, ScriptID right) => !left.Equals(right);
	}
}
