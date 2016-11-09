using System;

namespace Nuclear.Domain
{
    /// <summary>
    /// Interface for handling re-constitution and persistence of aggregates.
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public interface AggregateRepository<TAggregate>
        where TAggregate : IAggregate
    {
        /// <summary>
        /// Saves the uncommitted changes to the underlying storage engine.
        /// </summary>
        /// <param name="aggregate"></param>
        void Save(TAggregate aggregate);

        /// <summary>
        /// Returns a reconstituted object graph for an aggregate.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TAggregate GetById(Guid id);
    }
}
