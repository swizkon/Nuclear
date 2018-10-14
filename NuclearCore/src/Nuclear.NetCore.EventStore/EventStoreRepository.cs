using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nuclear.NetCore.Aggregates;
using Nuclear.NetCore.Events;

namespace Nuclear.NetCore.EventStore
{

    internal static class Serializer
    {

        private const string EventClrTypeHeader = "EventClrTypeName";
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        public static object DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
        }
    }

    public class EventStoreRepository : IEventRepository
    {
        private const string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const string CommitIdHeader = "CommitId";
        
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        private readonly IEventStoreConnection connection;

        public EventStoreRepository(IEventStoreConnection connection)
        {
            this.connection = connection;
        }

        public ICollection<IDomainEvent> ReadEvents(IEventCategory category)
        {
            throw new NotImplementedException();
        }

        public ICollection<IDomainEvent> ReadEvents(IStreamIdentifier streamIdentifier)
        {
            return ReadEventsInternal(streamIdentifier);
        }

        public ICollection<IDomainEvent> ReadEvents(IStreamIdentifier streamIdentifier, long fromVersion)
        {
            throw new NotImplementedException();
        }

        public void WriteEvents(IStreamIdentifier streamIdentifier, ICollection<IDomainEvent> events)
        {
            WriteEvents(streamIdentifier, ExpectedVersion.Any, events);
        }

        public void WriteEvents(IStreamIdentifier streamIdentifier, long expectedVersion, ICollection<IDomainEvent> events)
        {
            var commitHeaders = new Dictionary<string, object>
            {
                {CommitIdHeader, Guid.NewGuid()},
                {AggregateClrTypeHeader, streamIdentifier.AggregateClrTypeName()}
            };

            var newEvents = events.Cast<object>().ToList();
            // var expectedVersion = expectedRevision <= 0 ? ExpectedVersion.NoStream : expectedRevision - 1;

            // expectedVersion = ExpectedVersion.Any;

            var preparedEvents = PrepareEvents(newEvents, commitHeaders).ToList();

            connection.AppendToStreamAsync(streamIdentifier.StreamIdentifier(), ExpectedVersion.Any, preparedEvents).Wait();
        }



        private static IEnumerable<EventData> PrepareEvents(IEnumerable<object> events, IDictionary<string, object> commitHeaders)
        {
            return events.Select(e => JsonEventData.Create(Guid.NewGuid(), e, commitHeaders));
        }




        private ICollection<IDomainEvent> ReadEventsInternal(IStreamIdentifier stream)
        {
            List<IDomainEvent> eventsForAggregate = new List<IDomainEvent>();

            StreamEventsSlice currentSlice;
            var nextSliceStart = 0L;
            do
            {
                currentSlice = connection
                                .ReadStreamEventsForwardAsync(stream.StreamIdentifier(), nextSliceStart, ReadPageSize, false)
                                .Result;
                nextSliceStart = currentSlice.NextEventNumber;

                foreach (var evnt in currentSlice.Events)
                {
                    IDomainEvent aggrEvent = (IDomainEvent)Serializer.DeserializeEvent(evnt.OriginalEvent.Metadata, evnt.OriginalEvent.Data);
                    eventsForAggregate.Add(aggrEvent);
                }

            } while (!currentSlice.IsEndOfStream);

            return eventsForAggregate;
        }


    }
}
