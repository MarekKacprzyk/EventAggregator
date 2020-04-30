using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EventAggregator
{
    public class EventAggregator : IEventAggregator
    {
        private static readonly object SyncObject = new object();
        private readonly Dictionary<Type, List<object>> _eventDictionary;

        public EventAggregator()
        {
            _eventDictionary = new Dictionary<Type, List<object>>();
        }
        
        public Task Publish<TEventType>(TEventType eventType)
        {
            lock(SyncObject)
            {
                var dataType = typeof(TEventType);
                var list = GetSubscribersList(dataType).ToObservable();
                
                return list.ForEachAsync(p =>
                {
                    if(p is ISubscriber<TEventType> subscriber)
                    {
                        subscriber.OnHandle(eventType);
                    }
                });
            }
        }

        public IDisposable Subscribe(object subscriber)
        {
            lock(SyncObject)
            {
                var subscriberTypes = subscriber.GetType()
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscriber<>))
                    .SelectMany(t => t.GetGenericArguments());

                foreach(var subscriberType in subscriberTypes)
                {
                    var list = GetSubscribersList(subscriberType);
                    list.Add(subscriber);
                }

                return new Subscriber(subscriber, Unsubscribe);
            }
        }

        private void Unsubscribe(object subscriber)
        {
            
        }

        private IList<object> GetSubscribersList(Type subscriberType)
        {
            if (_eventDictionary.TryGetValue(subscriberType, out var subscriberList))
            {
                return subscriberList;
            }

            subscriberList = new List<object>();
            _eventDictionary.Add(subscriberType, subscriberList);
            return subscriberList;
        }

        private class Subscriber : IDisposable
        {
            private readonly object _item;
            private readonly Action<object> _unsubscribe;

            public Subscriber(object item, Action<object> unsubscribe)
            {
                _item = item;
                _unsubscribe = unsubscribe;
            }

            public void Dispose()
            {
                _unsubscribe.Invoke(_item);
            }
        }
    }
}