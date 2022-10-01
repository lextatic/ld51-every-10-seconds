using System;

namespace GameBase
{
	public abstract class BaseEventManager : IDisposable
	{
		public abstract void Subscribe<TEvent>(object subject, Action<TEvent> handler);

		public abstract void Unsubscribe<TEvent>(object subject, Action<TEvent> handler);

		public abstract void Dispatch<TEvent>(object subject, TEvent @event);

		public abstract void Dispose();
	}
}