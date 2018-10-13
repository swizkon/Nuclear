using System;

namespace Nuclear.NetCore.Aggregates
{
    public interface IDomainAggregate
    {
        Guid AggregateId { get; }

        int Version { get; }
    }
}