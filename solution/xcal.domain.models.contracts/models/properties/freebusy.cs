using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IFREEBUSY
    {
        FBTYPE Type { get; }

        List<IPERIOD> Periods { get; }
    }
}
