using Nuclear.Extensions;
using Nuclear.Messaging;
using System;
using System.Collections.Generic;

namespace Nuclear.Domain
{
    /// <summary>
    /// The base class for a Event sourced aggregate
    /// </summary>
    public abstract class AggregateBase : IAggregate
    {
        private readonly List<DomainEvent> _changes = new List<DomainEvent>();

        /// <summary>
        /// Base ctor to ensure the Id to always be available. 
        /// </summary>
        /// <param name="id"></param>
        protected AggregateBase(Guid id)
        {
            this.AggregateId = id;
        }

        /// <summary>
        /// Gets or sets the Id of this aggregate.
        /// </summary>
        public Guid AggregateId { get; protected set; }

        /// <summary>
        /// The revision, ie number of domain events processed since constructed.
        /// </summary>
        public int Revision { get; internal set; }

        /// <summary>
        /// Returns the changes to an aggregate after its last reconstitution.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DomainEvent> UncommittedChanges()
        {
            return _changes;
        }

        /// <summary>
        /// Clears the changes.
        /// TODO Maybe merge this and UncommittedChanges() into COmmitChanges(Irepository).
        /// which should allow side effects on the aggregate...
        /// </summary>
        public void ClearUncommittedEvents()
        {
            _changes.Clear();
        }

        /// <summary>
        /// Takes domain events and applies them to the object graph.
        /// </summary>
        /// <param name="history"></param>
        public void ReconstituteFromHistory(IEnumerable<DomainEvent> history)
        {
            foreach (var e in history)
            {
                this.ApplyChange(e);
            }
        }

        /// <summary>
        /// Adds the domain event to the changes and applies it to the object
        /// using AsDynamic().
        /// The class MUST have a Apply(TEvent domainEvent)
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="domainEvent"></param>
        protected void AcceptChange<TEvent>(TEvent domainEvent)
            where TEvent : DomainEvent
        {
            /*
            AcceptChange(domainEvent,  (e) => {
                ApplyChange(e);
            });
            */
            _changes.Add(domainEvent);
            this.ApplyChange(domainEvent);
        }

        private void ApplyChange<TEvent>(TEvent @event)
            where TEvent : DomainEvent
        {
            this.bumpRevision();
            this.AsDynamic().Apply(@event);
        }

        private void bumpRevision()
        {
            this.Revision += 1;
        }

    }
}
