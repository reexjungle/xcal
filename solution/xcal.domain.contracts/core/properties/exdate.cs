using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface IEXDATE
    {
        List<IDATE_TIME> DateTimes { get; set; }
        ITZID TimeZoneId { get; set; }
    }

}
