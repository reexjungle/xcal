using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface IATTENDEE
    {
        ICAL_ADDRESS Address { get; set; }
        CUTYPE CalendarUserType { get; set; }
        IMEMBER Member { get; set; }
        ROLE Role { get; set; }
        PARTSTAT Participation { get; set; }
        BOOLEAN Rsvp { get; set; }
        IDELEGATE Delegatee { get; set; }
        IDELEGATE Delegator { get; set; }
        ISENT_BY SentBy { get; set; }
        string CN { get; set; }
        IDIR Directory { get; set; }
        ILANGUAGE Language { get; set; }
    }
}
