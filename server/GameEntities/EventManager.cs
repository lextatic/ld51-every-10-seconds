using GameBase;
using System;
using System.Collections.Generic;

namespace GameEntities
{
	public class EventManager : BaseEventManager
	{
		private Dictionary<object, Delegate> _handlers;

		public EventManager()
		{
			_handlers = new Dictionary<object, Delegate>();
		}

		public override void Subscribe<TEvent>(object subject, Action<TEvent> handler)
		{
			if (_handlers.TryGetValue(subject, out var handlers))
			{
				_handlers[subject] = Delegate.Combine(handlers, handler);
			}
			else
			{
				_handlers.Add(subject, handler);
			}
		}

		public override void Unsubscribe<TEvent>(object subject, Action<TEvent> handler)
		{
			if (_handlers.TryGetValue(subject, out var handlers))
			{
				_handlers[subject] = Delegate.Remove(handlers, handler);

				if (_handlers[subject] == null)
				{
					_handlers.Remove(subject);
				}
			}
		}

		public override void Dispatch<TEvent>(object subject, TEvent @event)
		{
			if (_handlers.TryGetValue(subject, out var handle))
			{
				handle.DynamicInvoke(@event);
			}
		}

		public override void Dispose()
		{
			foreach(var kv in _handlers)
			{
				Delegate.RemoveAll(kv.Value, kv.Value);
			}
			_handlers.Clear();
		}
	}
}
