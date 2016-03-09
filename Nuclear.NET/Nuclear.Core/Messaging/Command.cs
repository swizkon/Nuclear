using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Messaging
{
    /// <summary>
    /// Marker interface for a class that is a command
    /// </summary>
    public interface Command : Message
    {
    }
}
