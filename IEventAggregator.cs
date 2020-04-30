using System;
using System.Threading.Tasks;

namespace EventAggregator
{
    public interface IEventAggregator
    {
        IDisposable Subscribe(object subscriber);
        Task Publish<TEventType>(TEventType eventType);
    }
}
