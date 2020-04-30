namespace EventAggregatorTest
{
    using NUnit.Framework;
    using EventAggregator;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Reflection;
    using System;
    using System.Linq;

    [TestFixture]
    public class Test
    {
        private EventAggregator _sut;
        private ISubscriber<int> _client;

        [SetUp]
        public void SetUp()
        {
            _sut = new EventAggregator();
            _client = Substitute.For<ISubscriber<int>>();
        }

        [Test]
        [Timeout(5000)]
        public void SubscribeTest()
        {
            _sut.Subscribe(_client);
            var dictionary = GetEventAggregatorDictionary();
            var subscribers = dictionary
                .SelectMany(d => d.Value)
                .ToArray();

            Assert.That(subscribers, Has.Member(_client));
            Assert.AreEqual(1, subscribers.Length);
        }

        [Test]
        [Timeout(5000)]
        public void PublishTest()
        {
            var value = 0;
            _sut.Subscribe(_client);
            _client.When(c => c.OnHandle(Arg.Any<int>())).Do(d => value += d.Arg<int>());
            var resoult = _sut.Publish(15);
            resoult.Wait();
            Assert.AreEqual(15, value);
        }

        [Test]
        [Timeout(5000)]
        public void UnsubscribeTest()
        {
            var value = 0;
            var subscriber = _sut.Subscribe(_client);
            _client.When(c => c.OnHandle(Arg.Any<int>())).Do(d => value += d.Arg<int>());
            subscriber.Dispose();
            var resoult = _sut.Publish(15);
            resoult.Wait();

            Assert.AreNotEqual(15, value);
        }

        [Test]
        [Timeout(5000)]
        [Obsolete]
        public void PublishManyCallbackTest([Range(1, 5)]int pow)
        {
            var counter = 0;
            var expect = (int)Math.Pow(10, pow);
            var subscribers = Enumerable.Range(0, expect)
                .Select(e => Substitute.For<ISubscriber<int>>())
                .ToArray();

            foreach(var subscriber in subscribers)
            {
                subscriber.When(c => c.OnHandle(Arg.Any<int>())).Do(d => counter++);
                _sut.Subscribe(subscriber);
            }

            var publishTask = _sut.Publish(0);
            publishTask.Wait();

            Assert.AreEqual(expect, counter);
        }

        private Dictionary<Type, List<object>> GetEventAggregatorDictionary()
        {
            var dictionary = _sut.GetType().GetField("_eventDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Dictionary<Type, List<object>>)dictionary.GetValue(_sut);
        }
    }
}
