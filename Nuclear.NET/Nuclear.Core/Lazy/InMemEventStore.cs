using Nuclear.Domain;
using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Lazy
{
    public class InMemEventStore : IAggregateEventStore
    {
        private readonly EventDispatcher _publisher;

        private struct EventDescriptor
        {
            public readonly Event EventData;
            public readonly Guid Id;
            public readonly int Revision;

            public EventDescriptor(Guid id, Event eventData, int revision)
            {
                EventData = eventData;
                Revision = revision;
                Id = id;
            }
        }

        public InMemEventStore(EventDispatcher publisher)
        {
            _publisher = publisher;
        }

        private readonly Dictionary<Guid, List<EventDescriptor>> _current = new Dictionary<Guid, List<EventDescriptor>>();

        public void SaveEvents(Aggregate aggregate, Guid aggregateId, IEnumerable<Event> events)
        {
            SaveEvents(new AggregateKey(aggregate.GetType(), aggregate.AggregateId), aggregate.Revision, events);
        }

        public void SaveEvents(AggregateKey key, int expectedRevision, IEnumerable<Event> events)
        {
            List<EventDescriptor> eventDescriptors;

            // try to get event descriptors list for given aggregate id
            // otherwise -> create empty dictionary
            if (!_current.TryGetValue(key.AggregateId, out eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                _current.Add(key.AggregateId, eventDescriptors);
            }
            // check whether latest event version matches current aggregate version
            // otherwise -> throw exception
            else if (eventDescriptors[eventDescriptors.Count - 1].Revision != expectedRevision && expectedRevision != -1)
            {
                throw new ConcurrencyException();
            }
            var i = expectedRevision;

            // iterate through current aggregate events increasing version with each processed event
            foreach (var @event in events)
            {
                i++;
                // @event.Version = i;

                // push event to the event descriptors list for current aggregate
                eventDescriptors.Add(new EventDescriptor(key.AggregateId, @event, i));
                // publish current event to the bus for further processing by subscribers
                _publisher.Publish(@event);
            }
        }

        // collect all processed events for given aggregate and return them as a list
        // used to build up an aggregate from its history (Domain.LoadsFromHistory)
        public List<Event> EventsForAggregate(Aggregate aggregate)
        {
            return EventsForAggregate(new AggregateKey(aggregate.GetType(), aggregate.AggregateId));
        }

        public List<Event> EventsForAggregate(AggregateKey key)
        {
            List<EventDescriptor> eventDescriptors;

            if (!_current.TryGetValue(key.AggregateId, out eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }

            return eventDescriptors.Select(desc => desc.EventData).ToList();
        }
    }


    public class AggregateNotFoundException : Exception
    {
    }

    public class ConcurrencyException : Exception
    {
    }
}
