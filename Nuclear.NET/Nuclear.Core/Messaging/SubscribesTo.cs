
namespace Nuclear.Messaging
{
    /// <summary>
    /// Interface for mapping subscribers to events.
    /// 
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface SubscribesTo<TEvent> where TEvent : Event
    {
        /// <summary>
        /// The metod that handles the event message.
        /// </summary>
        /// <param name="event"></param>
        void Consume(TEvent @event);
    }
}
