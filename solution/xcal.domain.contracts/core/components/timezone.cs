using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.properties;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.components
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
