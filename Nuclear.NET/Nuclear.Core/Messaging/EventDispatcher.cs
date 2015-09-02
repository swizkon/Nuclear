
namespace Nuclear.Messaging
{
    public interface EventDispatcher
    {
        void Publish<TEvent>(TEvent @event) where TEvent : Event;
    }
}
