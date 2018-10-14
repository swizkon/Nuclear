namespace Nuclear.NetCore.Aggregates
{
    public interface ISaveEventsAggregate : IDomainAggregate
    {
        void Save(IEventRepository repository);
    }
}