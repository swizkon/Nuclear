namespace Alice.Domain
{
    using System;
    using System.Collections.Generic;
    using Nuclear.NetCore.Aggregates;
    using Nuclear.NetCore.Events;

    public class Bug : DomainAggregateBase
    {
        public Bug(Guid id, ICollection<IDomainEvent> events) : base(id: id, events: events)
        {
            
        }
    }
}
