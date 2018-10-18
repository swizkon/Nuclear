using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nuclear.NetCore.EventStore
{
    internal static class Serializer
    {
        private const string EventClrTypeHeader = "EventClrTypeName";

        private const string AggregateClrTypeHeader = "AggregateClrTypeName";

        private const string AggregateIdHeader = "AggregateId";

        private const int WritePageSize = 500;

        private const int ReadPageSize = 500;

        public static object DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
        }

        public static IDictionary<string, object> DeserializeMetadata(byte[] metadata)
        {
            var data = JObject.Parse(Encoding.UTF8.GetString(metadata));
            var eventClrTypeName = data.Property(EventClrTypeHeader)?.Value;
            
            return new Dictionary<string, object>()
            {
                {EventClrTypeHeader, eventClrTypeName},
                {AggregateClrTypeHeader, data.Property(AggregateClrTypeHeader)?.Value},
                {AggregateIdHeader, data.Property(AggregateIdHeader)?.Value}
            };
        }
    }
}
