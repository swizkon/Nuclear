using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Domain
{
    /// <summary>
    /// Identity for an aggregate.
    /// </summary>
    public sealed class AggregateKey
    {
        /// <summary>
        /// The type of the aggreagate.
        /// </summary>
        public Type AggregateType { get; private set; }

        /// <summary>
        /// The id of the aggreagate.
        /// </summary>
        public Guid AggregateId { get; private set; }

        /// <summary>
        /// Open ctor.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        public AggregateKey(Type type, Guid id)
        {
            this.AggregateType = type;
            this.AggregateId = id;
        }

        /// <summary>
        /// Inverted factory method.
        /// </summary>
        /// <param name="obj"></param>
        public AggregateKey(Aggregate obj)
            : this(obj.GetType(), obj.AggregateId)
        {
            // this.AggregateType = obj.GetType();
            // this.AggregateId = obj.AggregateId;
        }
    }
}
