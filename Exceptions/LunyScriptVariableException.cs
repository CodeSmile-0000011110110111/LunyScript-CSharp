using System;

namespace LunyScript.Exceptions
{
	public sealed class LunyScriptVariableException : LunyScriptException
	{
		public LunyScriptVariableException(String message)
			: base(message) {}

		public LunyScriptVariableException(String message, Exception innerException)
			: base(message, innerException) {}
	}
}
