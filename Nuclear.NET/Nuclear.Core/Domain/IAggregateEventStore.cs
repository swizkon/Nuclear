using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Domain
{

    public interface IAggregateEventStore
    {
        void SaveEvents(Aggregate aggregate, Guid aggregateId, IEnumerable<Event> events);

        void SaveEvents(AggregateKey key, int expectedRevision, IEnumerable<Event> events);
        
        // List<Event> GetEventsForAggregate(Aggregate aggregate, Guid aggregateId);

        List<Event> EventsForAggregate(Aggregate aggregate);

        List<Event> EventsForAggregate(AggregateKey key);
    }

}
