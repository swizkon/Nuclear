using System.Collections.Generic;
using Nuclear.NetCore.Events;

namespace Nuclear.NetCore.Aggregates
{
    public interface IEventRepository
    {
        ICollection<TDomainEvent> ReadEventsByType<TDomainEvent>() where TDomainEvent : IDomainEvent;

        ICollection<IDomainEvent> ReadEvents(IStreamIdentifier streamIdentifier);

        ICollection<IDomainEvent> ReadEvents(IStreamIdentifier streamIdentifier, long fromVersion);

        void WriteEvents(IStreamIdentifier streamIdentifier, ICollection<IDomainEvent> events);

        void WriteEvents(IStreamIdentifier streamIdentifier, long expectedVersion, ICollection<IDomainEvent> events);
    }
}