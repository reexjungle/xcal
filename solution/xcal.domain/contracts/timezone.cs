﻿using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.domain.contracts
{
    public interface IOBSERVANCE
    {
        DATE_TIME Start { get; set; }

        UTC_OFFSET TimeZoneOffsetFrom { get; set; }

        UTC_OFFSET TimeZoneOffsetTo { get; set; }

        RECUR RecurrenceRule { get; set; }

        List<COMMENT> Comments { get; set; }

        List<RDATE> RecurrenceDates { get; set; }

        List<TZNAME> TimeZoneNames { get; set; }
    }

    public interface ITIMEZONE
    {
        TZID TimeZoneId { get; set; }

        URL Url { get; set; }

        DATE_TIME LastModified { get; set; }

        List<STANDARD> StandardTimes { get; set; }

        List<DAYLIGHT> DaylightTimes { get; set; }
    }
}