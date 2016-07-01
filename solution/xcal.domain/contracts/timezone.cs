using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.domain.contracts
{
    public interface IOBSERVANCE
    {
        DATE_TIME Start { get; }

        UTC_OFFSET TimeZoneOffsetFrom { get; }

        UTC_OFFSET TimeZoneOffsetTo { get; }

        RECUR RecurrenceRule { get; }

        List<COMMENT> Comments { get; }

        List<RDATE> RecurrenceDates { get; }

        List<TZNAME> TimeZoneNames { get; }
    }

    public interface ITIMEZONE
    {
        TZID TimeZoneId { get; }

        URL Url { get; }

        DATE_TIME LastModified { get; }

        List<OBSERVANCE> Observances { get; }
    }
}