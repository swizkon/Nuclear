using Nuclear.NetCore.Events;

namespace Alice.Organizational.Domain
{
    internal class UserCreatedEvent : IDomainEvent
    {
        public UserCreatedEvent()
        {
        }

        public string UserName { get; set; }
        public string ShortName { get; set; }
        public string email { get; set; }
        public Role Role { get; set; }
    }
}