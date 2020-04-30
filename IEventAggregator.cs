namespace EventAggregator
{
    using System;
    using System.Threading.Tasks;
    using System.Threading;

    public interface IEventAggregator
    {
        IDisposable Subscribe(object subscriber);
        Task Publish<TEventType>(TEventType eventType, CancellationToken token = new CancellationToken());
    }
}
