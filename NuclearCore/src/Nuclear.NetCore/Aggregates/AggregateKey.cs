using System;

namespace Nuclear.NetCore.Aggregates
{
    public sealed class AggregateKey : IStreamIdentifier
    {
        public Type AggregateType { get; private set; }

        public Guid AggregateId { get; private set; }
        
        public AggregateKey(Type type, Guid id)
        {
            this.AggregateType = type;
            this.AggregateId = id;
        }

        public AggregateKey(IDomainAggregate domainAggregate)
            : this(domainAggregate.GetType(), domainAggregate.AggregateId)
        {
        }
        
        public string StreamIdentifier() => $"{AggregateName}-{AggregateId}";

        private string AggregateName => AggregateType.FullName.Replace('.', '_');

        public string AggregateClrTypeName() => AggregateType.AssemblyQualifiedName;

        public string AggregateIdentifier() => AggregateId.ToString();
    }
}