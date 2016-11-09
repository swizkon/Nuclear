using Nuclear.Domain;
using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Lazy
{
    /// <summary>
    /// In memory for read / write and publishing of domain events.
    /// </summary>
    public class InMemEventStore : IAggregateEventStore
    {
        private readonly EventDispatcher _publisher;

        private struct EventDescriptor
        {
            public readonly DomainEvent EventData;
            public readonly Guid Id;
            public readonly int Revision;

            public EventDescriptor(Guid id, DomainEvent eventData, int revision)
            {
                EventData = eventData;
                Revision = revision;
                Id = id;
            }
        }

        /// <summary>
        /// Ctor with EventDispatcher dependency.
        /// </summary>
        /// <param name="publisher"></param>
        public InMemEventStore(EventDispatcher publisher)
        {
            _publisher = publisher;
        }

        private readonly Dictionary<Guid, List<EventDescriptor>> _current = new Dictionary<Guid, List<EventDescriptor>>();

        /// <summary>
        /// Stores the aggregate changes.
        /// </summary>
        /// <param name="aggregate"></param>
        public void SaveChanges(IAggregate aggregate)
        {
            SaveEvents(new AggregateKey(aggregate), aggregate.Revision, aggregate.UncommittedChanges());
            aggregate.ClearUncommittedEvents();
        }

        /// <summary>
        /// Saves the events for an aggregate that might be unknown to the system.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expectedRevision"></param>
        /// <param name="events"></param>
        public void SaveEvents(AggregateKey key, int expectedRevision, IEnumerable<DomainEvent> events)
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
                _publisher.Publish(@event as Event);
            }
        }

        /// <summary>
        /// Collects all processed events for given aggregate and return them as a list
        /// used to build up an aggregate from its history (Domain.ReconstituteFromHistory)
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        public List<DomainEvent> EventsForAggregate(IAggregate aggregate)
        {
            return EventsForAggregate(new AggregateKey(aggregate));
        }

        /// <summary>
        /// Collects all processed events for given aggregate and return them as a list
        /// used to build up an aggregate from its history (Domain.ReconstituteFromHistory)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<DomainEvent> EventsForAggregate(AggregateKey key)
        {
            List<EventDescriptor> eventDescriptors;

            if (!_current.TryGetValue(key.AggregateId, out eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }

            return eventDescriptors.Select(desc => desc.EventData).ToList();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AggregateNotFoundException : Exception
    {
    }

    /// <summary>
    /// Indicates thsat the expected version did not match.
    /// </summary>
    public class ConcurrencyException : Exception
    {
    }
}
