using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IEXDATE
    {
        List<IDATE_TIME> DateTimes { get; }
        ITZID TimeZoneId { get; }
    }

}
