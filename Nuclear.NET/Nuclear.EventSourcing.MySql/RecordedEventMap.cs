using NHibernate.Mapping.ByCode.Conformist;

namespace Nuclear.EventSourcing.MySql
{
    public class RecordedEventMap : ClassMapping<RecordedEvent>
    {
        public RecordedEventMap()
        {
            this.Table("RecordedEvents");
            this.Id(p => p.EventId);

            this.Property(p => p.Created);
            this.Property(p => p.CreatedEpoch);
            this.Property(p => p.Data);
            this.Property(p => p.EventId);
            this.Property(p => p.EventNumber);
            this.Property(p => p.EventStreamId);
            this.Property(p => p.EventType);
            this.Property(p => p.IsJson);
            this.Property(p => p.Metadata);
        }
    }

}
