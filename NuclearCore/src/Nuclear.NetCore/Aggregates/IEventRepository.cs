using System.Collections.Generic;
using Nuclear.NetCore.Events;

namespace Nuclear.NetCore.Aggregates
{
    public interface IEventRepository
    {
        IEnumerable<IDomainEvent> ReadEvents(IEventCategory category);

        IEnumerable<IDomainEvent> ReadEvents(AggregateKey aggregateKey);

        void WriteEvents(IEnumerable<IDomainEvent> events);
    }
}