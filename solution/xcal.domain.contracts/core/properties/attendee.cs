using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface IATTENDEE
    {
        ICAL_ADDRESS Address { get; }
        CUTYPE CalendarUserType { get; }
        IMEMBER Member { get; }
        ROLE Role { get; }
        PARTSTAT Participation { get; }
        BOOLEAN Rsvp { get; }
        IDELEGATE_TO Delegatee { get; }
        IDELEGATE_FROM Delegator { get; }
        ISENT_BY SentBy { get; }
        string CN { get; }
        IURI Directory { get; }
        ILANGUAGE Language { get; }
    }
}
