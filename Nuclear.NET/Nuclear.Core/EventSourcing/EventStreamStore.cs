using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.EventSourcing
{
    public interface StreamEventStore
    {
        void SaveEvents(string streamName, IEnumerable<Event> events, int expectedVersion);
        List<Event> GetEventsForStream(String streamName);

    }
}
