using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IRDATE
    {
        List<DATE_TIME> DateTimes { get; }
        List<PERIOD> Periods { get; }
        TZID TimeZoneId { get; }
        VALUE ValueType { get; }
    }

}
