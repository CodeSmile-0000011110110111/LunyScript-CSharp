using System;

namespace LunyScript
{
	/// <summary>
	/// Unique identifier for a LunyScript definition (type).
	/// Sequential integers for deterministic ordering and debugging.
	/// </summary>
	public readonly struct LunyScriptID : IEquatable<LunyScriptID>, IComparable<LunyScriptID>
	{
		private const Int32 StartID = 1;
		private static Int32 s_NextID = StartID;
		internal static void Reset() => s_NextID = StartID;

		public readonly Int32 Value;

		private LunyScriptID(Int32 value) => Value = value;

		/// <summary>
		/// Generates a new unique ScriptID.
		/// </summary>
		public static LunyScriptID Generate() => new(s_NextID++);

		public Boolean Equals(LunyScriptID other) => Value == other.Value;
		public override Boolean Equals(Object obj) => obj is LunyScriptID other && Equals(other);
		public override Int32 GetHashCode() => Value;
		public Int32 CompareTo(LunyScriptID other) => Value.CompareTo(other.Value);
		public override String ToString() => $"LunyScriptID:{Value}";

		public static Boolean operator ==(LunyScriptID left, LunyScriptID right) => left.Equals(right);
		public static Boolean operator !=(LunyScriptID left, LunyScriptID right) => !left.Equals(right);
	}
}
