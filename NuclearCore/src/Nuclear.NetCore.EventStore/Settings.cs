namespace Nuclear.NetCore.EventStore
{
    internal static class Settings
    {
        public const string AggregateClrTypeHeader = "AggregateClrTypeName";

        public const string EventClrTypeHeader = "EventClrTypeName";

        public const string CommitIdHeader = "CommitId";

        public const int WritePageSize = 500;

        public const int ReadPageSize = 500;

    }
}
