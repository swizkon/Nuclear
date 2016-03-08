using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Domain
{
    public sealed class AggregateKey
    {
        public Type AggregateType { get; private set; }
        public Guid AggregateId { get; private set; }

        public AggregateKey(Type type, Guid id)
        {
            this.AggregateType = type;
            this.AggregateId = id;
        }
    }
}
