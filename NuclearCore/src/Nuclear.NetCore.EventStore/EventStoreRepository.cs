using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using Nuclear.NetCore.Aggregates;
using Nuclear.NetCore.Events;
using Nuclear.NetCore.Extensions;

namespace Nuclear.NetCore.EventStore
{
    public class EventStoreRepository : IEventRepository
    {
        private readonly IEventStoreConnection connection;

        public EventStoreRepository(IEventStoreConnection connection)
        {
            this.connection = connection;
        }

        public ICollection<TDomainEvent> ReadEventsByType<TDomainEvent>() where TDomainEvent : IDomainEvent
        {
            var streamName = "$et-" + EventExtensions.UnifiedEventName(typeof(TDomainEvent));
            var eventsForAggregate = new List<TDomainEvent>();

            StreamEventsSlice currentSlice;
            var nextSliceStart = 0L;
            do
            {
                currentSlice = connection
                                .ReadStreamEventsForwardAsync(streamName, nextSliceStart, Settings.ReadPageSize, true)
                                .Result;
                nextSliceStart = currentSlice.NextEventNumber;

                foreach (var evnt in currentSlice.Events)
                {
                    var aggrEvent = (TDomainEvent)Serializer.DeserializeEvent(evnt.Event.Metadata, evnt.Event.Data);
                    eventsForAggregate.Add(aggrEvent);
                }

            } while (!currentSlice.IsEndOfStream);

            return eventsForAggregate;
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
                {Settings.CommitIdHeader, Guid.NewGuid()},
                {Settings.AggregateClrTypeHeader, streamIdentifier.AggregateClrTypeName()}
            };

            var newEvents = events.Cast<object>().ToList();
            
            var preparedEvents = PrepareEvents(newEvents, commitHeaders).ToList();

            connection.AppendToStreamAsync(streamIdentifier.StreamIdentifier(), expectedVersion, preparedEvents).Wait();
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
                                .ReadStreamEventsForwardAsync(stream.StreamIdentifier(), nextSliceStart, Settings.ReadPageSize, false)
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
