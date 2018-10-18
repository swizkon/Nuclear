using Nuclear.NetCore.Aggregates;

namespace Nuclear.NetCore.Events
{
    public sealed class AggregateEvent<TDomainEvent>
        where TDomainEvent: IDomainEvent
    {
        public AggregateEvent(AggregateKey aggregateKey, TDomainEvent evt)
        {
            AggregateKey = aggregateKey;
            this.Event = evt;
        }

        public AggregateKey AggregateKey { get; }

        public TDomainEvent Event { get; }
    }
}