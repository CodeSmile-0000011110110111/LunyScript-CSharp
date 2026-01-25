using Luny;
using System;
using System.Globalization;

namespace LunyScript
{
	public readonly struct LunyScriptVariable : IEquatable<LunyScriptVariable>, IEquatable<Boolean>, IEquatable<Double>, IEquatable<String>
	{
		public String Name { get; }
		public Object Value { get; }

		public LunyScriptVariable(String name, Object value)
		{
			Name = name;
			Value = value;
		}

		public Boolean AsBoolean() => Value is Boolean b ? b : false;
		public Number AsNumber() => Value is Number n ? n : default;
		public String AsString() => Value is String s ? s : default;

		public static implicit operator LunyScriptVariable(Int32 v) => new(null, v);
		public static implicit operator LunyScriptVariable(Single v) => new(null, v);
		public static implicit operator LunyScriptVariable(Double v) => new(null, v);
		public static implicit operator LunyScriptVariable(Boolean v) => new(null, v);
		public static implicit operator LunyScriptVariable(String v) => new(null, v);
		public static implicit operator LunyScriptVariable(Number v) => new(null, (Double)v);

		public override String ToString()
		{
			if (Value == null)
				return "<null>";

			var type = Value.GetType();
			if (type == typeof(Double) || type == typeof(Single) || type == typeof(Int32))
				return $"{Convert.ToDouble(Value, CultureInfo.InvariantCulture)} (Number)";
			if (type == typeof(Boolean))
				return $"{Value} (Boolean)";
			if (type == typeof(String))
				return $"{Value} (String)";

			return Value.ToString();
		}

		public Boolean Equals(Boolean b) => AsBoolean() == b;
		public Boolean Equals(Double d) => AsNumber() == d;
		public Boolean Equals(String s) => AsString() == s;
		public Boolean Equals(LunyScriptVariable other) => Name == other.Name && Equals(Value, other.Value);

		public override Boolean Equals(Object obj)
		{
			if (obj is LunyScriptVariable other)
				return Equals(other);
			if (obj is Boolean b)
				return Equals(b);
			if (obj is Double d)
				return Equals(d);
			if (obj is String s)
				return Equals(s);

			return false;
		}

		public override Int32 GetHashCode()
		{
			unchecked
			{
				return (Name != null ? Name.GetHashCode() : 0) * 397 ^ (Value != null ? Value.GetHashCode() : 0);
			}
		}

		public static Boolean operator ==(LunyScriptVariable left, LunyScriptVariable right) => left.Equals(right);
		public static Boolean operator !=(LunyScriptVariable left, LunyScriptVariable right) => !left.Equals(right);

		public static Boolean operator ==(LunyScriptVariable left, Boolean right) => left.Equals(right);
		public static Boolean operator !=(LunyScriptVariable left, Boolean right) => !left.Equals(right);
		public static Boolean operator ==(Boolean left, LunyScriptVariable right) => right.Equals(left);
		public static Boolean operator !=(Boolean left, LunyScriptVariable right) => !right.Equals(left);

		public static Boolean operator ==(LunyScriptVariable left, Double right) => left.Equals(right);
		public static Boolean operator !=(LunyScriptVariable left, Double right) => !left.Equals(right);
		public static Boolean operator ==(Double left, LunyScriptVariable right) => right.Equals(left);
		public static Boolean operator !=(Double left, LunyScriptVariable right) => !right.Equals(left);

		public static Boolean operator ==(LunyScriptVariable left, String right) => left.Equals(right);
		public static Boolean operator !=(LunyScriptVariable left, String right) => !left.Equals(right);
		public static Boolean operator ==(String left, LunyScriptVariable right) => right.Equals(left);
		public static Boolean operator !=(String left, LunyScriptVariable right) => !right.Equals(left);
	}
}
