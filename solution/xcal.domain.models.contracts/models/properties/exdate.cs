using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IEXDATE
    {
        List<DATE_TIME> DateTimes { get; }
        TZID TimeZoneId { get; }
    }

}
