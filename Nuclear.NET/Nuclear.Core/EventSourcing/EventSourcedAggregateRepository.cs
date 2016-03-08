﻿using Nuclear.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.EventSourcing
{
    public class EventSourcedAggregateRepository<TAggregate>
        : AggregateRepository<TAggregate> where TAggregate
        : class, Aggregate
    {
        private readonly IAggregateEventStore _storage;

        public EventSourcedAggregateRepository(IAggregateEventStore storage)
        {
            _storage = storage;
        }

        public void Save(Aggregate aggregate)
        {
            _storage.SaveEvents(aggregate, aggregate.AggregateId, aggregate.UncommittedChanges());
        }

        public TAggregate GetById(Guid id)
        {
            // var obj = new T(); // lots of ways to do this

            TAggregate obj = (TAggregate)Activator.CreateInstance(typeof(TAggregate), id);
            // Activator.CreateInstance(typeof(T), id);
            var e = _storage.EventsForAggregate(obj);
            obj.LoadsFromHistory(e);
            return obj;
        }
    }
}
