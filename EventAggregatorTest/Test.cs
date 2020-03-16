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
        EventAggregator _sut;
        ISubscriber<int> _client;

        [SetUp]
        public void SetUp()
        {
            _sut = new EventAggregator();
            _client = Substitute.For<ISubscriber<int>>();
        }

        [Test]
        public void SubscribeTest()
        {
            _sut.Subscribe(_client);
            var dictionary = GetEventAggregatorDictionary();
            var subscribers = dictionary
                .SelectMany(d => d.Value)
                .Select(c => c.Target).ToArray();

            Assert.That(subscribers, Has.Member(_client));
        }

        [Test]
        public void PublishTest()
        {
            Assert.Fail();
        }

        [Test]
        public void UnsubscribeTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PublishManyCallbackTest()
        {
            Assert.Fail();
        }

        private Dictionary<Type, List<WeakReference>> GetEventAggregatorDictionary()
        {
            var dictionary = _sut.GetType().GetField("_eventDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Dictionary<Type, List<WeakReference>>)dictionary.GetValue(_sut);
        }
    }
}
