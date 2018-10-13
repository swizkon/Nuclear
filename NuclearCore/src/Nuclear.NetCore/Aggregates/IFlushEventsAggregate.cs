namespace Nuclear.NetCore.Aggregates
{
    public interface IFlushEventsAggregate : IDomainAggregate
    {
        void Flush(IEventRepository repository);
    }
}