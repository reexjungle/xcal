using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface IEXDATE
    {
        List<IDATE_TIME> DateTimes { get; }
        ITZID TimeZoneId { get; }
    }

}
