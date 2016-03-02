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
            // this.Version += 1;
            Console.WriteLine("Contructing " + this.GetType().Name + " with Id: " + id.ToString());
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
            Console.WriteLine("-".PadLeft(76, '-'));
            Console.WriteLine("LoadsFromHistory...");

            foreach (var e in history)
                ApplyChange(e, false);
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        // push atomic aggregate changes to local history for further processing (EventStore.SaveEvents)
        private void ApplyChange(Event @event, bool isNew)
        {
            this.Version += 1;
            /*
            if (!isNew)
            else
                this.Version += 1;
            */

            this.AsDynamic().Apply(@event);

            if (isNew) _changes.Add(@event);
        }
    }
}
