using System;

namespace EventAggregator
{
    class EventAggregator : IEventAggregator
    {

        public EventAggregator()
        {

        }

        public void Publish<TEventType>(TEventType eventType)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(object subscriber)
        {
            throw new NotImplementedException();
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
