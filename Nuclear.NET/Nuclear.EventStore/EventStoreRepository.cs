using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nuclear.Domain;
using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclear.EventStore
{
    public class EventStoreRepository : IAggregateEventStore
    {

        private const string EventClrTypeHeader = "EventClrTypeName";
        private const string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const string CommitIdHeader = "CommitId";
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        private readonly Func<Type, Guid, string> _aggregateIdToStreamName;

        private readonly IEventStoreConnection _eventStoreConnection;

        private EventDispatcher _publisher;

        public EventStoreRepository(IEventStoreConnection eventStoreConnection, EventDispatcher publisher)
            : this(eventStoreConnection, (t, g) => string.Format("{0}-{1}", char.ToLower(t.Name[0]) + t.Name.Substring(1), g))
        {
            _publisher = publisher;
        }

        private EventStoreRepository(IEventStoreConnection eventStoreConnection, Func<Type, Guid, string> aggregateIdToStreamName)
        {
            _eventStoreConnection = eventStoreConnection;
            _aggregateIdToStreamName = aggregateIdToStreamName;
        }



        public object DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
        }

        private static IEnumerable<EventData> PrepareEvents(IEnumerable<object> events, IDictionary<string, object> commitHeaders)
        {
            return events.Select(e => JsonEventData.Create(Guid.NewGuid(), e, commitHeaders));
        }

        private static class JsonEventData
        {
            public static EventData Create(Guid eventId, object evnt, IDictionary<string, object> headers)
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));
                var metadata = AddEventClrTypeHeaderAndSerializeMetadata(evnt, headers);
                var typeName = evnt.GetType().Name;

                return new EventData(eventId, typeName, true, data, metadata);
            }

            private static byte[] AddEventClrTypeHeaderAndSerializeMetadata(object evnt, IDictionary<string, object> headers)
            {
                var eventHeaders = new Dictionary<string, object>(headers)
                {
                    {EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName}
                };

                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
            }

            private static readonly JsonSerializerSettings SerializerSettings;

            static JsonEventData()
            {
                SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            }
        }




        public List<DomainEvent> EventsForAggregate(Aggregate aggregate)
        {
            return EventsForAggregate(new AggregateKey(aggregate));
        }


        public List<DomainEvent> EventsForAggregate(AggregateKey key)
        {
            List<DomainEvent> eventsForAggregate = new List<DomainEvent>();

            var streamName = _aggregateIdToStreamName(key.AggregateType, key.AggregateId);

            StreamEventsSlice currentSlice;
            var nextSliceStart = 0;
            do
            {
                currentSlice = _eventStoreConnection
                                .ReadStreamEventsForwardAsync(streamName, nextSliceStart, ReadPageSize, false)
                                .Result;
                nextSliceStart = currentSlice.NextEventNumber;

                foreach (var evnt in currentSlice.Events)
                {
                    DomainEvent aggrEvent = (DomainEvent)DeserializeEvent(evnt.OriginalEvent.Metadata, evnt.OriginalEvent.Data);
                    eventsForAggregate.Add(aggrEvent);
                }

            } while (!currentSlice.IsEndOfStream);

            return eventsForAggregate;
        }



        public void SaveChanges(Aggregate aggregate)
        {
            SaveEvents(new AggregateKey(aggregate.GetType(), aggregate.AggregateId), aggregate.Revision, aggregate.UncommittedChanges());
            aggregate.ClearUncommittedEvents();
        }

        public void SaveEvents(AggregateKey key, int expectedRevision, IEnumerable<DomainEvent> uncommittedEvents)
        {
            var commitHeaders = new Dictionary<string, object>
            {
                {CommitIdHeader, Guid.NewGuid()},
                {AggregateClrTypeHeader, key.AggregateType.AssemblyQualifiedName}
            };

            var streamName = _aggregateIdToStreamName(key.AggregateType, key.AggregateId);
            var newEvents = uncommittedEvents.Cast<object>().ToList();
            var expectedVersion = expectedRevision <= 0 ? ExpectedVersion.NoStream : expectedRevision - 1;

            expectedVersion = ExpectedVersion.Any;

            var preparedEvents = PrepareEvents(newEvents, commitHeaders).ToList();

            if (preparedEvents.Count < WritePageSize)
            {
                _eventStoreConnection
                    .AppendToStreamAsync(streamName, expectedVersion, preparedEvents)
                    .Wait();
            }
            else
            {
                var transaction = _eventStoreConnection
                                        .StartTransactionAsync(streamName, expectedVersion)
                                        .Result;

                var position = 0;
                while (position < preparedEvents.Count)
                {
                    var pageEvents = preparedEvents.Skip(position).Take(WritePageSize);
                    transaction.WriteAsync(pageEvents).Wait();
                    position += WritePageSize;
                }

                transaction.CommitAsync().Wait();
            }

            foreach (var @event in uncommittedEvents)
            {
                _publisher.Publish(@event);
            }
        }

        public void RepublishAllEvents()
        {
            AllEventsSlice currentSlice;
            var nextPosition = Position.Start;
            do
            {
                currentSlice = _eventStoreConnection
                                .ReadAllEventsForwardAsync(nextPosition, ReadPageSize, false)
                                .Result;

                nextPosition = currentSlice.NextPosition;

                foreach (var evnt in currentSlice.Events)
                {
                    DomainEvent aggrEvent = (DomainEvent)DeserializeEvent(evnt.OriginalEvent.Metadata, evnt.OriginalEvent.Data);
                    this._publisher.Publish(aggrEvent);
                }

            } while (!currentSlice.IsEndOfStream);

        }

    }

}
