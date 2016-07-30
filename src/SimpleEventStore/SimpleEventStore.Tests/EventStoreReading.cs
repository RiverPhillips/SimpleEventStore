﻿using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleEventStore.Tests.Events;

namespace SimpleEventStore.Tests
{
    // TODO: Add the remaining features
    // 1. Allow the storage engine fake to be seeded with streams, better than using the actual methods that we are developing
    // 2. Allow a read from event version x to y
    // 3. Read an "$all" stream - requires a global checkpoint

    [TestFixture]
    public class EventStoreReading
    {
        private const string StreamId = "TEST-ORDER";
        private StorageEngineFake engine;
        private EventStore subject;

        [SetUp]
        public async Task SetUp()
        {
            engine = new StorageEngineFake();
            subject = new EventStore(engine);

            await subject.AppendToStream(StreamId, new OrderCreated(StreamId), 0);
            await subject.AppendToStream(StreamId, new OrderDispatched(StreamId), 1);
        }

        [Test]
        public async Task when_reading_a_stream_all_events_are_returned()
        {
            var events = await subject.ReadStreamForwards(StreamId);

            Assert.That(events.Count(), Is.EqualTo(2));
            Assert.That(events.First().EventBody, Is.TypeOf<OrderCreated>());
            Assert.That(events.Skip(1).Single().EventBody, Is.TypeOf<OrderDispatched>());
        }
    }
}
