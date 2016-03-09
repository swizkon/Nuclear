using Nuclear.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.EventSourcing
{
    /// <summary>
    /// Generic implementation of event sourced aggregate
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate to act on</typeparam>
    public class EventSourcedAggregateRepository<TAggregate>
        : AggregateRepository<TAggregate> where TAggregate
        : class, Aggregate
    {
        private readonly IAggregateEventStore _storage;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="storage"></param>
        public EventSourcedAggregateRepository(IAggregateEventStore storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Saves the uncommitted changes to the storage engine.
        /// </summary>
        /// <param name="aggregate"></param>
        public void Save(TAggregate aggregate)
        {
            _storage.SaveChanges(aggregate);
        }

        /// <summary>
        /// Returns a reconstituted object graph for an aggregate.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TAggregate GetById(Guid id)
        {
            TAggregate obj = (TAggregate)Activator.CreateInstance(typeof(TAggregate), id);
            var e = _storage.EventsForAggregate(obj);
            obj.ReconstituteFromHistory(e);
            return obj;
        }
    }
}
