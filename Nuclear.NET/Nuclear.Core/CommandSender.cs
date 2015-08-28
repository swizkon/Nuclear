using Nuclear.Domain;
using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclear
{
    public interface CommandSender
    {
        void Send<T>(T command) where T : Command;
    }
}
