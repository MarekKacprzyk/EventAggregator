using System;

namespace EventAggregator
{
    public interface IEventAggregator
    {
        IDisposable Subscribe(object subscriber);
        void Publish<TEventType>(TEventType eventType);
    }
}
