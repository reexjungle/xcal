using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface IFREEBUSY_PROPERTY
    {
        FBTYPE Type { get; set; }

        List<IPERIOD> Periods { get; set; }
    }
}
