using Nuclear.NetCore.Events;

namespace Alice.Organizational.Events
{
    public class OrganizationCreated : IDomainEvent
    {
        public string Name { get; set; }
    }
}