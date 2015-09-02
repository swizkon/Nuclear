using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Messaging
{
    public interface Bus : CommandDelegate, EventDispatcher
    {

        void RegisterHandler<T>(Action<T> handler) where T : Command;

        // void RegisterHandler<THandler>(THandler handler) where THandler : CommandHandler;

        void RegisterSubscriber<T>(Action<T> subscriber) where T : Event;
    }
}
