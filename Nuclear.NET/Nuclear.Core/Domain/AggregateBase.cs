using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nuclear.Extensions;

namespace Nuclear.Domain
{
    public abstract class AggregateRoot : Aggregate
    {

        private readonly List<Event> _changes = new List<Event>();

        // public abstract Guid Id { get; }

        public Guid AggregateId
        {
            get { return this.Id; }
        }

        protected Guid Id; // { get; }
        public int Version { get; internal set; }

        protected AggregateRoot(Guid id)
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
