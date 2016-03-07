using Nuclear.Extensions;
using Nuclear.Messaging;
using System;
using System.Collections.Generic;

namespace Nuclear.Domain
{
    public abstract class AggregateBase : Aggregate
    {
        private readonly List<Event> _changes = new List<Event>();

        protected Guid Id;

        public Guid AggregateId
        {
            get { return this.Id; }
        }

        public int Version { get; internal set; }

        protected AggregateBase(Guid id)
        {
            this.Id = id;
        }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void ClearUncommittedEvents()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                applyChange(e);
            }
        }

        /*
        protected void AcceptChange(Event @event)
        {
            ApplyChange(@event, true);
        }
        */

        protected void AcceptChange<TEvent>(TEvent domainEvent)
            where TEvent : Event
        {
            _changes.Add(domainEvent);

            applyChange(domainEvent);
        }

        /*
        protected void AcceptChange<TEvent>(TEvent domainEvent, Action<TEvent> apply)
            where TEvent : Event
        {
            Console.WriteLine("Accept: " + domainEvent.GetType().Name);
            apply(domainEvent);
        }*/

        private void applyChange(Event @event)
        {
            this.Version += 1;
            this.AsDynamic().Apply(@event);
        }

        /*
        // push atomic aggregate changes to local history for further processing (EventStore.SaveEvents)
        private void ApplyChange(Event @event, bool isNew)
        {
            this.Version += 1;

            this.AsDynamic().Apply(@event);

            if (isNew) _changes.Add(@event);
        }
        */
    }
}
