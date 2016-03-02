using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using Nuclear.Domain;
using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
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


        private readonly Func<Type, Guid, string> _aggregateIdToStreamName;

        private readonly ISessionFactory factory;

        private EventDispatcher _publisher;

        public MySqlEventSource(ISessionFactory factory, EventDispatcher publisher)
        {
            this._publisher = publisher;
            this.factory = factory;
            this._aggregateIdToStreamName = (t, g) => string.Format("{0}-{1}", char.ToLower(t.Name[0]) + t.Name.Substring(1), g);
        }

        public void SaveEvents(Aggregate aggregate, Guid aggregateId, IEnumerable<Event> events)
        {
            var commitHeaders = new Dictionary<string, object>
            {
                {CommitIdHeader, Guid.NewGuid()},
                {AggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName}
            };

            // updateHeaders(commitHeaders);

            var streamName = _aggregateIdToStreamName(aggregate.GetType(), aggregate.AggregateId);
            var newEvents = aggregate.GetUncommittedChanges().Cast<object>().ToList();


            var eventNumber = aggregate.Version;

            var preparedEvents = PrepareEvents(newEvents, commitHeaders).ToList();

            using (ISession session = this.factory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                foreach (var uncommittedEvent in preparedEvents)
                {
                    var entry = new RecordedEvent()
                    {
                        Created = DateTime.UtcNow,
                        CreatedEpoch = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds,
                        Data = uncommittedEvent.Data,
                        EventId = uncommittedEvent.EventId,
                        EventStreamId = streamName,
                        IsJson = uncommittedEvent.IsJson,
                        EventType = uncommittedEvent.Type,
                        Metadata = uncommittedEvent.Metadata,
                        EventNumber = eventNumber
                    };
                    session.Save(entry);
                    eventNumber += 1;
                }

                tx.Commit();
            }



            foreach (var @event in aggregate.GetUncommittedChanges())
            {
                _publisher.Publish(@event);
            }

            aggregate.ClearUncommittedEvents();

        }


        public List<Event> GetEventsForAggregate(Aggregate aggregate, Guid aggregateId)
        {
            List<Event> eventsForAggregate = new List<Event>();

            var streamName = _aggregateIdToStreamName(aggregate.GetType(), aggregateId);

            using (ISession session = this.factory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                var events = session
                            .QueryOver<RecordedEvent>()
                            .Where(e => e.EventStreamId == streamName)
                            .OrderBy(e => e.EventNumber).Asc
                            .Future();

                foreach (var evnt in events)
                {
                    Event aggrEvent = (Event)DeserializeEvent(evnt.Metadata, evnt.Data);
                    eventsForAggregate.Add(aggrEvent);
                }
            }

            return eventsForAggregate;
        }


        object DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
        }

        private static IEnumerable<EventData> PrepareEvents(IEnumerable<object> events, IDictionary<string, object> commitHeaders)
        {
            return events.Select(e => JsonEventData.Create(Guid.NewGuid(), e, commitHeaders));
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
