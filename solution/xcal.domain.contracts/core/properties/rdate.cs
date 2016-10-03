using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface IRDATE
    {
        List<IDATE_TIME> DateTimes { get; set; }
        List<IPERIOD> Periods { get; set; }
        ITZID TimeZoneId { get; set; }
        VALUE ValueType { get; set; }
    }

}
