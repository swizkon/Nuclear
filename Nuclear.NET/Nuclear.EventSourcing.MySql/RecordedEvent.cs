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
        public virtual DateTime Created { get; set; }
        //
        // Summary:
        //     A long representing the milliseconds since the epoch when the was created
        //     in the system
        public virtual long CreatedEpoch { get; set; }
        //
        // Summary:
        //     A byte array representing the data of this event
        public virtual byte[] Data { get; set; }
        //
        // Summary:
        //     The Unique Identifier representing this event
        public virtual Guid EventId { get; set; }
        //
        // Summary:
        //     The number of this event in the stream
        public virtual int EventNumber { get; set; }
        //
        // Summary:
        //     The Event Stream that this event belongs to
        public virtual string EventStreamId { get; set; }
        //
        // Summary:
        //     The type of event this is
        public virtual string EventType { get; set; }
        //
        // Summary:
        //     Indicates whether the content is internally marked as json
        public virtual bool IsJson { get; set; }
        //
        // Summary:
        //     A byte array representing the metadata associated with this event
        public virtual byte[] Metadata { get; set; }
    }
}
