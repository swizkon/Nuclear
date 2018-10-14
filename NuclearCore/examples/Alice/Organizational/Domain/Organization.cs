namespace Alice.Organizational.Domain
{
    using System;
    using System.Collections.Generic;
    using Alice.Organizational.Events;
    using Nuclear.NetCore.Aggregates;
    using Nuclear.NetCore.Events;

    public class Organization : DomainAggregateBase
    {
        public Organization(Guid id, IEnumerable<IDomainEvent> events) : base(id: id, events: events)
        {
            
        }

        public void CreateUser(string userName, string shortName, string email, Role role)
        {
                var evt = new UserCreatedEvent(){
                    UserName = userName,
                    ShortName = shortName,
                    email = email,
                    Role = role
                };
                AcceptChange(domainEvent: evt);
        }

        public void PromoteUser(string user, Role role, string promotedBy)
        {

        }
    }
}
