
using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventstoreConsole.Events
{

    public class TaskAdded
    {
        public string Name { get; set; }
    }

    public static class EventDataBuilder
    {

        private const string EventClrTypeHeader = "EventClrTypeName";
        private const string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const string CommitIdHeader = "CommitId";

        public static EventData BuildTaskAddedEvent(string v)
        {
            var metadata = new Dictionary<string, string>();

            var t = new TaskAdded { Name = v };
            return Create(Guid.NewGuid(), t, metadata);
        }

        private static EventData Create(Guid eventId, object evnt, IDictionary<string, string> headers)
        {
            var SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));

            var metadata = AddEventClrTypeHeaderAndSerializeMetadata(evnt, headers);
            var typeName = evnt.GetType().Name;

            return new EventData(eventId, typeName, true, data, metadata);
        }

        private static byte[] AddEventClrTypeHeaderAndSerializeMetadata(object evnt, IDictionary<string, string> headers)
        {
            var SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            var eventHeaders = new Dictionary<string, string>(headers)
                {
                    {EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName}
                };

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
        }

    }

}