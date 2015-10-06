using Nuclear.Domain;
using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nuclear.Lazy
{
    public class Switchboard : Bus
    {
        /*
        private readonly Dictionary<Type, List<Action<Message>>> _routes
            = new Dictionary<Type, List<Action<Message>>>();
        */

        // Change into string as key to easier switch between implementation namespaces etc...
        // SEE The JustGiving event store code on github for clarity...
        private readonly Dictionary<string, List<Action<Message>>> _routes
            = new Dictionary<string, List<Action<Message>>>();


        public Action<object, Command> CommandSent
        {
            get;
            set;
        }

        public Action<object, Event> EventPublished
        {
            get;
            set;
        }

        public void RegisterHandler<T>(Action<T> handler) where T : Command
        {
            List<Action<Message>> handlers;

            if (!_routes.TryGetValue(typeof(T).Name, out handlers))
            {
                handlers = new List<Action<Message>>();
                _routes.Add(typeof(T).Name, handlers);
            }

            handlers.Add((x => handler((T)x)));
        }


        public void RegisterSubscriber<T>(Action<T> handler) where T : Event
        {
            List<Action<Message>> handlers;

            if (!_routes.TryGetValue(typeof(T).Name, out handlers))
            {
                handlers = new List<Action<Message>>();
                _routes.Add(typeof(T).Name, handlers);
            }

            handlers.Add((x => handler((T)x)));
        }


        public void Send<T>(T command) where T : Command
        {
            string commandTypeName = command.GetType().Name;
            List<Action<Message>> handlers;
            bool commandHandlerFound = false;

            commandHandlerFound = _routes.TryGetValue(commandTypeName, out handlers);

            if (commandHandlerFound)
            {
                if (handlers.Count != 1)
                {
                    throw new InvalidOperationException("cannot send to more than one command executor");
                }
                // handlers[0](command);
            }
            else if (_routes.ContainsKey(commandTypeName))
            {
                handlers = _routes[commandTypeName];
                commandHandlerFound = handlers != null && handlers.Count != 0;

                if (commandHandlerFound)
                {
                    handlers[0](command);
                }

                if (handlers != null && handlers.Count != 1)
                {
                    throw new InvalidOperationException("cannot send to more than one command executor");
                }
            }


            //
            // Exec command if handler was found...
            if (commandHandlerFound)
            {
                handlers[0](command);
            }
            else // Debug stuff...
            {
                String routes = "";
                foreach (var key in _routes.Keys)
                {
                    routes += " [" + key + ":";
                    var funcs = _routes[key];
                    foreach (var f in funcs)
                    {
                        routes += " . " + f.Method.Name;
                    }
                    routes += "] ";
                }

                throw new InvalidOperationException("No handler registered for " + commandTypeName + " (" + routes + ")");
            }


            if (commandHandlerFound && CommandSent != null)
            {
                CommandSent(this, command);
            }
        }

        public void Publish<T>(T @event) where T : Event
        {
            List<Action<Message>> handlers;

            if (!_routes.TryGetValue(@event.GetType().Name, out handlers))
            {
                return;
            }

            foreach (var handler in handlers)
            {
                //dispatch on thread pool for added awesomeness
                var handler1 = handler;
                ThreadPool.QueueUserWorkItem(x => handler1(@event));
                // handler1(@event);
            }

            if (EventPublished != null)
                EventPublished(this, @event);
        }

    }
}
