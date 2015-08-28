using Nuclear.Domain;
using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear
{
    public class Switchboard : 
        EventPublisher, 
        CommandSender
    {
        private readonly Dictionary<Type, List<Action<Message>>> _routes
            = new Dictionary<Type, List<Action<Message>>>();

        public void RegisterHandler<T>(Action<T> handler) where T : Message
        {
            List<Action<Message>> handlers;

            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<Message>>();
                _routes.Add(typeof(T), handlers);
            }

            handlers.Add((x => handler((T)x)));
        }


        public void Send<T>(T command) where T : Command
        {
            List<Action<Message>> handlers;

            if (_routes.TryGetValue(typeof(T), out handlers))
            {
                if (handlers.Count != 1) throw new InvalidOperationException("cannot send to more than one command executor");
                handlers[0](command);
            }
            else
            {
                throw new InvalidOperationException("no handler registered");
            }
        }

        public void Publish<T>(T @event) where T : Event
        {
            List<Action<Message>> handlers;

            if (!_routes.TryGetValue(@event.GetType(), out handlers))
            {

                return;
            }

            foreach (var handler in handlers)
            {
                //dispatch on thread pool for added awesomeness
                var handler1 = handler;
                // ThreadPool.QueueUserWorkItem(x => handler1(@event));
                handler1(@event);
            }
        }

    }
}
