using Nuclear.NetCore.Aggregates;

namespace Nuclear.NetCore.Commands
{
    public interface IAggregateCommand<T> where T : IDomainAggregate
    {

    }
}