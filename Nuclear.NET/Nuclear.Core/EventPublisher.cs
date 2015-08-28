using Nuclear.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear
{
    public interface EventPublisher
    {
        void Publish<TEvent>(TEvent @event) where TEvent : Event;
    }
}
