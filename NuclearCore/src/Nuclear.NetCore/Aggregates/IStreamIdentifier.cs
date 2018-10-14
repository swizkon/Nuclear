namespace Nuclear.NetCore.Aggregates
{
    public interface IStreamIdentifier
    {
        string AggregateClrTypeName();

        string StreamIdentifier();
    }
}