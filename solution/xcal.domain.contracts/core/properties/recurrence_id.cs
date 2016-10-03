using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface IRECURRENCE_ID
    {
        IDATE_TIME Value { get; set; }
        RANGE Range { get; set; }
        ITZID TimeZoneId { get; set; }
    }
}
