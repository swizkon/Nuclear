using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclear.EventSourcing.MySql
{
    public sealed class EventData
    {
        // public readonly int position;

        public readonly byte[] Data;

        public readonly Guid EventId;

        public readonly bool IsJson;

        public readonly byte[] Metadata;

        public readonly string Type;

        public EventData(Guid eventId, string type, bool isJson, byte[] data, byte[] metadata)
        {
            this.EventId = eventId;
            this.Type = type;
            this.IsJson = isJson;
            this.Data = data;
            this.Metadata = metadata;
        }
    }
}
