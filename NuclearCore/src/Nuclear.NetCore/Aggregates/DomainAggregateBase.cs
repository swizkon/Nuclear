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
    public abstract class DomainAggregateBase : ISaveEventsAggregate, IStreamIdentifier
    {
        private readonly List<IDomainEvent> _changes = new List<IDomainEvent>();

        public Guid AggregateId { get; protected set; }

        public int Version { get; internal set; } = -1;

        protected DomainAggregateBase(Guid id, ICollection<IDomainEvent> events)
        {
            this.AggregateId = id;
            foreach (var evt in events)
            {
                this.Version += 1;
                this.ApplyChange(evt);
            }
        }

        public string StreamIdentifier() => new AggregateKey(this).StreamIdentifier();

        public string AggregateClrTypeName() => new AggregateKey(this).AggregateClrTypeName();

        public string AggregateIdentifier() => new AggregateKey(this).AggregateIdentifier();


        // public IEnumerable<IDomainEvent> UncommittedChanges()
        // {
        //     return _changes;
        // }

        // public void ClearUncommittedEvents()
        // {
        //     _changes.Clear();
        // }

        protected void AcceptChange<TEvent>(TEvent domainEvent)
            where TEvent : IDomainEvent
        {
            System.Console.WriteLine("AcceptChange " + @domainEvent.GetType().Name);
            _changes.Add(domainEvent);
            this.ApplyChange(domainEvent);
        }

        private void ApplyChange<TEvent>(TEvent @event)
            where TEvent : IDomainEvent
        {
            System.Console.WriteLine("ApplyChange " + @event.GetType().Name);
            this.AsDynamic().Apply(@event);
        }

        public void Save(IEventRepository repository)
        {
            repository.WriteEvents(this, Version, _changes);

            _changes.Clear();
        }
    }
}