using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Messaging
{
    public interface Handles<T> where T : Message
    {
        void Handle(T message);
    }
}
