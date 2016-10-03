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
        IDELEGATE_TO Delegatee { get; set; }
        IDELEGATE_FROM Delegator { get; set; }
        ISENT_BY SentBy { get; set; }
        string CN { get; set; }
        IURI Directory { get; set; }
        ILANGUAGE Language { get; set; }
    }
}
