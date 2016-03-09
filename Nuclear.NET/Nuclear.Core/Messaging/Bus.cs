using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Messaging
{
    /// <summary>
    /// Interface for a Bus, including CommandDelegate and EventDispatcher.
    /// </summary>
    public interface Bus : CommandDelegate, EventDispatcher
    {
        /// <summary>
        /// Register a handle for a command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        void RegisterHandler<T>(Action<T> handler) where T : Command;

        // void RegisterHandler<THandler>(THandler handler) where THandler : CommandHandler;

        /// <summary>
        /// Register a subscriber for an event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subscriber"></param>
        void RegisterSubscriber<T>(Action<T> subscriber) where T : Event;
    }
}
