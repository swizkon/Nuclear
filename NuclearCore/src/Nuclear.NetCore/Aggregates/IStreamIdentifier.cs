namespace Nuclear.NetCore.Aggregates
{
    public interface IStreamIdentifier
    {
        string AggregateClrTypeName();

        string AggregateIdentifier();

        string StreamIdentifier();
    }
}