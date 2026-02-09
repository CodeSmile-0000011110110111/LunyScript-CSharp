using System;

namespace LunyScript.Api
{
	public readonly struct TimeApi
	{
		private readonly ILunyScript _script;
		internal TimeApi(ILunyScript script)
		{
			_script = script;
			ElapsedSeconds = Double.NaN;
		}

		public readonly Double ElapsedSeconds; // TODO: implementation
	}
}
