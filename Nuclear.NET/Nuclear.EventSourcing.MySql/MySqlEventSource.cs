using Newtonsoft.Json;
using Nuclear.Domain;
using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.EventSourcing.MySql
{
    public class MySqlEventSource : IAggregateEventStore
    {
        private const string EventClrTypeHeader = "EventClrTypeName";
        private const string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const string CommitIdHeader = "CommitId";
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        public void SaveEvents(Aggregate aggregate, Guid aggregateId, IEnumerable<Event> events)
        {
            throw new NotImplementedException();
        }

        public List<Event> GetEventsForAggregate(Aggregate aggregate, Guid aggregateId)
        {
            throw new NotImplementedException();
        }


        private static class JsonEventData
        {
            public static EventData Create(Guid eventId, object evnt, IDictionary<string, object> headers)
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));
                var metadata = AddEventClrTypeHeaderAndSerializeMetadata(evnt, headers);
                var typeName = evnt.GetType().Name;

                return new EventData(eventId, typeName, true, data, metadata);
            }

            private static byte[] AddEventClrTypeHeaderAndSerializeMetadata(object evnt, IDictionary<string, object> headers)
            {
                var eventHeaders = new Dictionary<string, object>(headers)
                {
                    {EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName}
                };

                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
            }

            private static readonly JsonSerializerSettings SerializerSettings;

            static JsonEventData()
            {
                SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            }
        }

    }
}
