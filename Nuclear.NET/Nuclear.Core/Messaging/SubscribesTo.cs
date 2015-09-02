using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Messaging
{
    public interface SubscribesTo<T> where T : Event
    {
        void Handle(T @event);
    }
}
