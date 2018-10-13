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
    /// <summary>
    /// Simple RAM implementation of a Bus interface.
    /// </summary>
    public class Switchboard : Bus
    {
        // Change into string as key to easier switch between implementation namespaces etc...
        // SEE The JustGiving event store code on github for clarity...
        private readonly Dictionary<string, List<Action<Message>>> _routes
            = new Dictionary<string, List<Action<Message>>>();

        /*
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
        */

        /// <summary>
        /// Register a handle for a command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
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


        /// <summary>
        /// Register a subscriber for an event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subscriber"></param>
        public void RegisterSubscriber<T>(Action<T> subscriber) where T : Event
        {
            List<Action<Message>> handlers;

            if (!_routes.TryGetValue(typeof(T).Name, out handlers))
            {
                handlers = new List<Action<Message>>();
                _routes.Add(typeof(T).Name, handlers);
            }

            handlers.Add((x => subscriber((T)x)));
        }


        /// <summary>
        /// Send a command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
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
                string routes = "";
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

            /*
            if (commandHandlerFound && CommandSent != null)
            {
                CommandSent(this, command);
            }
            */
        }

        /// <summary>
        /// Publish an event to all subscribers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
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
            }
            /*
            if (EventPublished != null)
                EventPublished(this, @event);
            */
        }

    }
}
