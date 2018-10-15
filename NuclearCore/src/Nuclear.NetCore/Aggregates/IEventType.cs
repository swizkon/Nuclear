using Nuclear.NetCore.Events;

namespace Nuclear.NetCore.Aggregates
{
    public interface IEventType
    {
        string StreamIdentifier();
    }

    public class EventTypeDescriptor : IEventType
    {
        private readonly IDomainEvent eventType;

        public EventTypeDescriptor(IDomainEvent eventType)
        {
            this.eventType = eventType;
        }
        
        public string StreamIdentifier() => eventType.GetType().Name;
    }
}