namespace EventAggregator
{
    public interface ISubscriber<T>
    {
        void OnHandle(T item);
    }
}
