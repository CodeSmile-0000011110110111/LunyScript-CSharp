using System;

namespace LunyScript.Registries
{
	/// <summary>
	/// Metadata for a discovered LunyScript type.
	/// Represents the script definition/template, not an instance.
	/// </summary>
	public sealed class ScriptDefinition
	{
		/// <summary>
		/// Unique identifier for this script definition.
		/// </summary>
		public ScriptID ScriptID { get; }

		/// <summary>
		/// The C# Type of the LunyScript subclass.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// The name of the script (used for object binding).
		/// Derived from the Type name by default.
		/// </summary>
		public String Name { get; }

		public ScriptDefinition(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (!typeof(LunyScript).IsAssignableFrom(type))
				throw new ArgumentException($"Type {type.Name} does not inherit from LunyScript", nameof(type));

			ScriptID = ScriptID.Generate();
			Type = type;
			Name = type.Name;
		}

		public override String ToString() => $"ScriptDefinition: {Name} ({ScriptID}, {Type.FullName})";
	}
}
