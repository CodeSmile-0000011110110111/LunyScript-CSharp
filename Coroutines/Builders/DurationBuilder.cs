using System;

namespace LunyScript.Coroutines.Builders
{
	internal readonly struct DurationBuilder<TFinal>
	{
		private readonly String _name;
		private readonly Double _amount;
		private readonly CoroutineContinuationMode _continuationMode;
		private readonly Func<CoroutineConfig, TFinal> _factory;

		internal DurationBuilder(String name, Double amount, Func<CoroutineConfig, TFinal> factory,
			CoroutineContinuationMode continuationMode = CoroutineContinuationMode.Finite)
		{
			_name = name;
			_amount = amount;
			_continuationMode = continuationMode;
			_factory = factory;
		}

		public TFinal Seconds() => _factory(CoroutineConfig.ForTimer(_name, _amount, _continuationMode));
		public TFinal Milliseconds() => _factory(CoroutineConfig.ForTimer(_name, _amount / 1000.0, _continuationMode));
		public TFinal Minutes() => _factory(CoroutineConfig.ForTimer(_name, _amount * 60.0, _continuationMode));
		public TFinal Heartbeats() => _factory(CoroutineConfig.ForCounter(_name, (Int32)_amount, _continuationMode));
		public TFinal Frames() => _factory(CoroutineConfig.ForCounter(_name, (Int32)_amount, _continuationMode));
	}
}
