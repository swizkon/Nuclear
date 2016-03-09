using Nuclear.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Domain
{
    /// <summary>
    /// The simple interface for a Event sourced aggregate
    /// </summary>
    public interface Aggregate
    {
        /// <summary>
        /// Takes domain events and applies them to the object graph.
        /// </summary>
        /// <param name="history"></param>
        void ReconstituteFromHistory(IEnumerable<DomainEvent> history);

        /// <summary>
        /// Returns the changes to an aggregate after its last reconstitution.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DomainEvent> UncommittedChanges();

        /// <summary>
        /// Clears the changes.
        /// TODO Maybe merge this and UncommittedChanges() into COmmitChanges(Irepository).
        /// which should allow side effects on the aggregate...
        /// </summary>
        void ClearUncommittedEvents();

        /// <summary>
        /// The Id of this aggregate.
        /// </summary>
        Guid AggregateId { get; }

        /// <summary>
        /// The revision, ie number of domain events processed since constructed.
        /// </summary>
        int Revision { get; }
    }
}
