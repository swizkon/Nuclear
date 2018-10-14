using Nuclear.NetCore.Events;

namespace Alice.Organizational.Events
{
    public class OrganizationRenamed : IDomainEvent
    {
        public string NewName { get; set; }
    }
}