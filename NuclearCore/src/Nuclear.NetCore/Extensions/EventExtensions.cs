using System;
using Nuclear.NetCore.Events;

namespace Nuclear.NetCore.Extensions
{
    public static class EventExtensions
    {
        public static string UnifiedEventName(this IDomainEvent domainEvent)
        {
            return UnifiedEventName(domainEvent.GetType());
        }

        public static string UnifiedEventName(Type eventType)
        {
            var name = eventType.Name;
            return name; //char.ToLower(name[0]) + name.Substring(1);
        }
    }
}