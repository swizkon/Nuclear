
namespace Nuclear.Messaging
{
    public interface CommandDelegate
    {
        void Send<T>(T command) where T : Command;
    }
}
