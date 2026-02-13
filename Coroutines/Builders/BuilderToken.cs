using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Coroutines.Builders
{
	internal sealed class BuilderToken
	{
		private readonly String _name;
		private readonly String _type;
		private readonly String _file;
		private readonly Int32 _line;
		private Boolean _isFinished;

		public BuilderToken(String name, String type, [CallerFilePath] String file = "", [CallerLineNumber] Int32 line = -1)
		{
			_name = name;
			_type = type;
			_file = file;
			_line = line;
		}

		public void MarkFinished()
		{
			_isFinished = true;
			GC.SuppressFinalize(this);
		}

		public static void LogWarning(BuilderToken token)
		{
			LunyLogger.LogWarning($"{token._file}({token._line}): Unfinished {token._type} builder '{token._name}' detected. " +
			                      "Did you forget to call a terminal method like .Do() or .WhenElapsed()?");
		}

		~BuilderToken()
		{
			if (!_isFinished)
			{
				LogWarning(this);
			}
		}
	}
}
