using Luny;
using System;
using System.Globalization;

namespace LunyScript
{
	public readonly struct Variable
	{
		public String Name { get; }
		public Object Value { get; }

		public Variable(String name, Object value)
		{
			Name = name;
			Value = value;
		}

		public static implicit operator Variable(Int32 v) => new(null, v);
		public static implicit operator Variable(Single v) => new(null, v);
		public static implicit operator Variable(Double v) => new(null, v);
		public static implicit operator Variable(Boolean v) => new(null, v);
		public static implicit operator Variable(String v) => new(null, v);
		public static implicit operator Variable(Number v) => new(null, (Double)v);

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

		public override Boolean Equals(Object obj) => obj is Variable other && Equals(other);

		public Boolean Equals(Variable other) => Name == other.Name && Equals(Value, other.Value);

		public override Int32 GetHashCode()
		{
			unchecked
			{
				return (Name != null ? Name.GetHashCode() : 0) * 397 ^ (Value != null ? Value.GetHashCode() : 0);
			}
		}

		public static Boolean operator ==(Variable left, Variable right) => left.Equals(right);
		public static Boolean operator !=(Variable left, Variable right) => !left.Equals(right);
	}
}
