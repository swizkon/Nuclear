using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Domain
{
    public interface Aggregate
    {
        void LoadsFromHistory(IEnumerable<Event> history);
        IEnumerable<Event> GetUncommittedChanges();
        void ClearUncommittedEvents();

        Guid AggregateId { get; }
        int Version { get; }
    }
}
