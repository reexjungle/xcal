using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.test.units.contracts
{
    public interface IPropertiesUnitTest
    {
        IEnumerable<ATTENDEE> GenerateAttendeesOfSize(int n);
    }
}