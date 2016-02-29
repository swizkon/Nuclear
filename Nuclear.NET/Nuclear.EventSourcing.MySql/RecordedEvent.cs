using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.EventSourcing.MySql
{
    public class RecordedEvent
    {

        // Summary:
        //     A datetime representing when this event was created in the system
        public DateTime Created;
        //
        // Summary:
        //     A long representing the milliseconds since the epoch when the was created
        //     in the system
        public long CreatedEpoch;
        //
        // Summary:
        //     A byte array representing the data of this event
        public byte[] Data;
        //
        // Summary:
        //     The Unique Identifier representing this event
        public Guid EventId;
        //
        // Summary:
        //     The number of this event in the stream
        public int EventNumber;
        //
        // Summary:
        //     The Event Stream that this event belongs to
        public string EventStreamId;
        //
        // Summary:
        //     The type of event this is
        public string EventType;
        //
        // Summary:
        //     Indicates whether the content is internally marked as json
        public bool IsJson;
        //
        // Summary:
        //     A byte array representing the metadata associated with this event
        public byte[] Metadata;
    }
}
