using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Nuclear.NetCore.Extensions;

namespace Nuclear.NetCore.EventStore
{
    internal static class JsonEventData
    {
        private static readonly JsonSerializerSettings SerializerSettings;

        static JsonEventData()
        {
            SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
        }

        public static EventData Create(Guid eventId, object evnt, IDictionary<string, object> headers)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));
            var metadata = AddEventClrTypeHeaderAndSerializeMetadata(evnt, headers);
            var typeName = EventExtensions.UnifiedEventName(evnt.GetType());
            return new EventData(eventId, typeName, true, data, metadata);
        }

        private static byte[] AddEventClrTypeHeaderAndSerializeMetadata(object evnt, IDictionary<string, object> headers)
        {
            var eventHeaders = new Dictionary<string, object>(headers)
                {
                    {Settings.EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName}
                };

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
        }
    }
}
