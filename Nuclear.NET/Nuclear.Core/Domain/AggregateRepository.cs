using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.Domain
{

    public interface AggregateRepository<T> where T
        : Aggregate
    {
        void Save(Aggregate aggregate);
        T GetById(Guid id);
    }
}
