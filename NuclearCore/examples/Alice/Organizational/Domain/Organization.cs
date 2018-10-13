namespace Alice.Organizational.Domain
{
    using System;
    using System.Collections.Generic;
    using Nuclear.NetCore.Aggregates;
    using Nuclear.NetCore.Events;

    public class Organization : DomainAggregateBase
    {
        public Organization(Guid id, IEnumerable<IDomainEvent> events) : base(id: id, events: events)
        {
            
        }

        public void CreateUser(string userName, string shortName, string email, Role role)
        {

        }

        public void PromoteUser(string user, Role role, string promotedBy)
        {

        }
    }
}
