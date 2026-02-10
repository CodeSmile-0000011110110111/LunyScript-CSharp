using System;

namespace LunyScript
{
	/// <summary>
	/// Unique identifier for a LunyScript definition (type).
	/// Sequential integers for deterministic ordering and debugging.
	/// </summary>
	public readonly struct ScriptDefID : IEquatable<ScriptDefID>, IComparable<ScriptDefID>
	{
		private const Int32 StartID = 1;
		private static Int32 s_NextID = StartID;
		internal static void Reset() => s_NextID = StartID;

		public readonly Int32 Value;

		private ScriptDefID(Int32 value) => Value = value;

		/// <summary>
		/// Generates a new unique ScriptID.
		/// </summary>
		public static ScriptDefID Generate() => new(s_NextID++);

		public Boolean Equals(ScriptDefID other) => Value == other.Value;
		public override Boolean Equals(Object obj) => obj is ScriptDefID other && Equals(other);
		public override Int32 GetHashCode() => Value;
		public Int32 CompareTo(ScriptDefID other) => Value.CompareTo(other.Value);
		public override String ToString() => $"LunyScriptID:{Value}";

		public static Boolean operator ==(ScriptDefID left, ScriptDefID right) => left.Equals(right);
		public static Boolean operator !=(ScriptDefID left, ScriptDefID right) => !left.Equals(right);
	}
}
