using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexmonkey.xcal.domain.contracts
{
    /// <summary>
    /// Represents the icalendar scale type e.g. GREGORIAN 
    /// </summary>
    public enum CALSCALE { GREGORIAN, HEBREW, ISLAMIC, INDIAN, CHINESE, JULIAN, UNKNOWN }

    public enum METHOD { REQUEST, REPLY, REFRESH, PUBLISH, CANCEL, ADD, COUNTER, DECLINECOUNTER, UNKNOWN }

    /// <summary>
    /// Represents the type of iCalendar encodings
    /// </summary>
    public enum ENCODING {UNKNOWN, BIT8, BASE64 }


    public enum TimeFormat { Unknown, Local, Utc, LocalAndTimeZone  }

    /// <summary>
    /// Represents the type of Free Busy Time
    /// </summary>
    public enum FBTYPE {UNKNOWN, FREE, BUSY, BUSY_UNAVAILABLE, BUSY_TENTATIVE }

    /// <summary>
    /// Represents an enumration of calendar user types
    /// </summary>
    public enum CUTYPE { INDIVIDUAL, GROUP, RESOURCE, ROOM, UNKNOWN}

    public enum BOOLEAN { TRUE, FALSE, UNKNOWN}

    /// <summary>
    /// Represents the icalendar role types
    /// </summary>
    public enum ROLE { UNKNOWN, CHAIR, REQ_PARTICIPANT, OPT_PARTICIPANT, NON_PARTICIPANT }

    /// <summary>
    /// Represents an eneumration for an iCalendar participation status
    /// </summary>
    public enum PARTSTAT {UNKNOWN, NEEDS_ACTION, ACCEPTED, DECLINED, TENTATIVE, DELEGATED, COMPLETED, IN_PROGRESS }

    /// <summary>
    /// Represents an enumeration for the range paramter of the icalendar
    /// </summary>
    public enum RANGE {UNKNOWN, THIS, THISANDFUTURE, ALL}

    /// <summary>
    /// Represents the frequency type of an icalendar component
    /// </summary>
    public enum FREQ { UNKNOWN, SECONDLY, MINUTELY, HOURLY, DAILY, WEEKLY, MONTHLY, YEARLY  }

    /// <summary>
    /// Represents the type of day in the week
    /// </summary>
    public enum WEEKDAY {UNKNOWN, SU, MO, TU, WE, TH, FR, SA }

    /// <summary>
    /// Represents the type of of access classification
    /// </summary>
    public enum CLASS { UNKNOWN, PUBLIC, PRIVATE, CONFIDENTIAL, }

    /// <summary>
    /// Represents the type of hierachical relationship between components.
    /// </summary>
    public enum RELTYPE {UNKNOWN, PARENT, CHILD, SIBLING }

    /// <summary>
    /// Represents the type of alarm trigger relationship
    /// </summary>
    public enum RELATED {UNKNOWN, START, END }

    /// <summary>
    /// 
    /// </summary>
    public enum TRANSP { UNKNOWN, OPAQUE, TRANSPARENT}


    public enum STATUS {UNKNOWN, TENTATIVE, CONFIRMED, CANCELLED, NEEDS_ACTION, COMPLETED, IN_PROCESS, DRAFT, FINAL }

    public enum PRIORITYLEVEL{ UNKNOWN, LOW, MEDIUM, HIGH }

    public enum PRIORITYSCHEMA { UNKNOWN, A1, A2, A3, B1, B2, B3, C1, C2, C3 }

    public enum ACTION { UNKNOWN, AUDIO, DISPLAY, EMAIL}

    /// <summary>
    /// Represents the type of sign
    /// </summary>
    public enum SignType { Negative, Neutral, Positive }

    /// <summary>
    /// To explicitly specify the value type format for a property value
    /// </summary>
    public enum ValueFormat 
    {
        UNKNOWN,
        BINARY,
        CAL_ADDRESS,
        DATE, 
        DATE_TIME, 
        DURATION,
        FLOAT,
        INTEGER,
        PERIOD, 
        RECUR,
        TIME,
        URI,
        UTC_OFFSET
    }

    public enum PeriodFormat { Explicit, Start }

    public enum RecurFormat { DateTime, Range }

    public enum TriggerFormat { Related, Absolute }

    public enum PriorityFormat { Integral, Level, Schema }


}
