using System;

namespace LunyScript.Api
{
	public readonly struct TimeApi
	{
		private readonly IScript _script;

		internal TimeApi(IScript script)
		{
			_script = script;
			ElapsedSeconds = Double.NaN;
		}

		public readonly Double ElapsedSeconds; // TODO: implementation
	}
}
