
namespace Nuclear.Messaging
{
    /// <summary>
    /// Subscriber pattern
    /// </summary>
    public interface EventDispatcher
    {
        /// <summary>
        /// Publish an event to all subscribers
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        void Publish<TEvent>(TEvent @event) where TEvent : Event;
    }
}
