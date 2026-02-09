using System;

namespace LunyScript.Coroutines.Builders
{
	internal static class UnitConverter
	{
		public static Double ToMilliseconds(Double amount) => amount / 1000.0;
		public static Double ToMinutes(Double amount) => amount * 60.0;
	}
}
