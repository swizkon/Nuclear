using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Messaging
{
    /// <summary>
    /// Interface for command handlers.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface Handles<TCommand> where TCommand : Command
    {
        /// <summary>
        /// Place for real logic or piping to the domain model.
        /// </summary>
        /// <param name="command"></param>
        void Process(TCommand command);
    }
}
