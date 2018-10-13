using System.Collections.Generic;
using Nuclear.NetCore.Events;

namespace Nuclear.NetCore.Aggregates
{
    public interface IEventRepository
    {
        IEnumerable<IDomainEvent> ReadEvents(IEventCategory category);

        IEnumerable<IDomainEvent> ReadEvents(IStreamIdentifier streamIdentifier);

        void WriteEvents(IStreamIdentifier streamIdentifier, IEnumerable<IDomainEvent> events);
    }
}