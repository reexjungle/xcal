using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface IORGANIZER
    {
        ICAL_ADDRESS Address { get; set; }
        string CN { get; set; }
        IDIR Directory { get; set; }
        ISENT_BY SentBy { get; set; }
        ILANGUAGE Language { get; set; }
    }
}
