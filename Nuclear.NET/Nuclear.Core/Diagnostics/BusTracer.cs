using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Diagnostics
{
    interface BusTracer
    {

        Action<object, Command> CommandSent
        {
            get;
        }

        Action<object, Event> EventPublished
        {
            get;
        }
    }
}
