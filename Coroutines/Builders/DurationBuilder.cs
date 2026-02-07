using System;

namespace LunyScript.Coroutines.Builders
{
	internal readonly struct DurationBuilder<TFinal>
	{
		private readonly String _name;
		private readonly Double _amount;
		private readonly Boolean _isRepeating;
		private readonly Func<CoroutineConfig, TFinal> _factory;

		internal DurationBuilder(String name, Double amount, Func<CoroutineConfig, TFinal> factory,
			CoroutineContinuation continuation = CoroutineContinuation.Finite)
		{
			_name = name;
			_amount = amount;
			_isRepeating = continuation == CoroutineContinuation.Repeating;
			_factory = factory;
		}

		public TFinal Seconds() => _factory(CoroutineConfig.ForTimeInterval(_name, _amount, _isRepeating));
		public TFinal Milliseconds() => _factory(CoroutineConfig.ForTimeInterval(_name, _amount / 1000.0, _isRepeating));
		public TFinal Minutes() => _factory(CoroutineConfig.ForTimeInterval(_name, _amount * 60.0, _isRepeating));
		public TFinal Heartbeats() => _factory(CoroutineConfig.ForTargetCount(_name, (Int32)_amount, _isRepeating));
		public TFinal Frames() => _factory(CoroutineConfig.ForTargetCount(_name, (Int32)_amount, _isRepeating));
	}
}
