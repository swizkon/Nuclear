using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Nuclear.NetCore.Events;
using Nuclear.NetCore.Extensions;

namespace Nuclear.NetCore.Aggregates
{
    public abstract class DomainAggregateBase : IDomainAggregate, IStreamIdentifier
    {
        private readonly List<IDomainEvent> _changes = new List<IDomainEvent>();

        public Guid AggregateId { get; protected set; }

        public int Version { get; internal set; }

        protected DomainAggregateBase(Guid id, IEnumerable<IDomainEvent> events)
        {
            this.AggregateId = id;
            foreach (var evt in events)
            {
                this.ApplyChange(evt);
            }
        }

        public string StreamIdentifier() => new AggregateKey(this).StreamIdentifier();

        public IEnumerable<IDomainEvent> UncommittedChanges()
        {
            return _changes;
        }

        public void ClearUncommittedEvents()
        {
            _changes.Clear();
        }

        protected void AcceptChange<TEvent>(TEvent domainEvent)
            where TEvent : IDomainEvent
        {
            _changes.Add(domainEvent);
            this.ApplyChange(domainEvent);
        }

        private void ApplyChange<TEvent>(TEvent @event)
            where TEvent : IDomainEvent
        {
            this.bump();
            this.AsDynamic().Apply(@event);
        }

        private void bump()
        {
            this.Version += 1;
        }
    }
}