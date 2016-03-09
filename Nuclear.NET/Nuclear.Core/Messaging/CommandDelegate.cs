
namespace Nuclear.Messaging
{
    /// <summary>
    /// CQRS command interface
    /// </summary>
    public interface CommandDelegate
    {
        /// <summary>
        /// Send a command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        void Send<T>(T command) where T : Command;
    }
}
