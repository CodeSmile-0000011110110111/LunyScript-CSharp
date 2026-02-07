using LunyScript.Coroutines;
using System;

namespace LunyScript.Api
{
	internal readonly struct TimeUnitBuilder<TFinal>
	{
		private readonly ILunyScript _script;
		private readonly String _name;
		private readonly Int32 _amount;
		private readonly Boolean _isRepeating;
		private readonly Boolean _isTimer;
		private readonly Func<CoroutineOptions, TFinal> _factory;

		internal TimeUnitBuilder(ILunyScript script, String name, Int32 amount, Boolean isRepeating, Boolean isTimer, Func<CoroutineOptions, TFinal> factory)
		{
			_script = script;
			_name = name;
			_amount = amount;
			_isRepeating = isRepeating;
			_isTimer = isTimer;
			_factory = factory;
		}

		public TFinal Seconds() => _factory(CoroutineOptions.ForDuration(_name, _amount, _isRepeating, false, _isTimer));
		public TFinal Milliseconds() => _factory(CoroutineOptions.ForDuration(_name, _amount / 1000.0, _isRepeating, false, _isTimer));
		public TFinal Minutes() => _factory(CoroutineOptions.ForDuration(_name, _amount * 60.0, _isRepeating, false, _isTimer));
		public TFinal Heartbeats() => _factory(CoroutineOptions.ForDuration(_name, _amount, _isRepeating, true, _isTimer));
	}
}
