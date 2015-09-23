using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.tests.contracts.factories
{
    public interface IEventFactory
    {
        VEVENT Create();

        IEnumerable<VEVENT> Create(int quantity);
    }
}