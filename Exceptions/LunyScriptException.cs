using Luny.Exceptions;
using System;

namespace LunyScript.Exceptions
{
	public class LunyScriptException : LunyException
	{
		public LunyScriptException(String message)
			: base(message) {}

		public LunyScriptException(String message, Exception innerException)
			: base(message, innerException) {}
	}
}
