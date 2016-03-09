using System.Collections.Generic;

namespace Nuclear.Domain
{
    /// <summary>
    /// Interface for read / write of domain events.
    /// </summary>
    public interface IAggregateEventStore
    {
        /// <summary>
        /// Stores the aggregate changes.
        /// </summary>
        /// <param name="aggregate"></param>
        void SaveChanges(Aggregate aggregate);

        /// <summary>
        /// Saves the events for an aggregate that might be unknown to the system.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expectedRevision"></param>
        /// <param name="events"></param>
        void SaveEvents(AggregateKey key, int expectedRevision, IEnumerable<DomainEvent> events);

        /// <summary>
        /// Returns the events for a known aggregate
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        List<DomainEvent> EventsForAggregate(Aggregate aggregate);

        /// <summary>
        /// Returns the domain events for a possible unknown aggregate.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<DomainEvent> EventsForAggregate(AggregateKey key);
    }
}
