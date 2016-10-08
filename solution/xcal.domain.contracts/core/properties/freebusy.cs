using System.Collections.Generic;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface IFREEBUSY
    {
        FBTYPE Type { get; }

        List<IPERIOD> Periods { get; }
    }
}
