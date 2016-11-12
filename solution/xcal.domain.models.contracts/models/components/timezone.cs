using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.properties;
using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.components
{
    public interface IOBSERVANCE
    {
        DATE_TIME Start { get; }

        UTC_OFFSET TimeZoneOffsetFrom { get; }

        UTC_OFFSET TimeZoneOffsetTo { get; }

        RECUR RecurrenceRule { get; }

        List<ICOMMENT> Comments { get; }

        List<IRDATE> RecurrenceDates { get; }

        List<ITZNAME> TimeZoneNames { get; }
    }

    public interface ITIMEZONE
    {
        TZID TimeZoneId { get; }

        IURL Url { get; }

        DATE_TIME LastModified { get; }

        List<IOBSERVANCE> Observances { get; }
    }
}
