using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.properties;
using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.components
{
    public interface IOBSERVANCE
    {
        IDATE_TIME Start { get; }

        IUTC_OFFSET TimeZoneOffsetFrom { get; }

        IUTC_OFFSET TimeZoneOffsetTo { get; }

        IRECUR RecurrenceRule { get; }

        List<ICOMMENT> Comments { get; }

        List<IRDATE> RecurrenceDates { get; }

        List<ITZNAME> TimeZoneNames { get; }
    }

    public interface ITIMEZONE
    {
        ITZID TimeZoneId { get; }

        IURL Url { get; }

        IDATE_TIME LastModified { get; }

        List<IOBSERVANCE> Observances { get; }
    }
}
