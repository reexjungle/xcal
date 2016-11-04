using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IRDATE
    {
        List<IDATE_TIME> DateTimes { get; }
        List<IPERIOD> Periods { get; }
        ITZID TimeZoneId { get; }
        VALUE ValueType { get; }
    }

}
