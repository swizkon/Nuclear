
namespace Nuclear.Messaging
{
    /// <summary>
    /// Marker interface for an event that can be broadcasted with no
    /// additional changes to the corresponding aggregate.
    /// Compare to the DomainEvent for event sourced aggregates.
    /// </summary>
    public interface Event : Message
    {
    }
}
