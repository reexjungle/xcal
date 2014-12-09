namespace reexjungle.xcal.domain.contracts
{
    /// <summary>
    /// Represents the icalendar scale type e.g. GREGORIAN
    /// </summary>
    public enum CALSCALE { UNKNOWN, GREGORIAN, HEBREW, ISLAMIC, INDIAN, CHINESE, JULIAN }

    /// <summary>
    /// Represents iCalendar object method associated with the calendar object.
    /// </summary>
    public enum METHOD { UNKNOWN, REQUEST, REPLY, REFRESH, PUBLISH, CANCEL, ADD, COUNTER, DECLINECOUNTER }

    /// <summary>
    /// Represents the type of iCalendar encodings
    /// </summary>
    public enum ENCODING { UNKNOWN, BIT8, BASE64 }

    public enum TimeType { Unknown, Local, Utc, LocalAndTimeZone }

    /// <summary>
    /// Represents the type of Free Busy Time
    /// </summary>
    public enum FBTYPE { UNKNOWN, FREE, BUSY, BUSY_UNAVAILABLE, BUSY_TENTATIVE }

    /// <summary>
    /// Represents an enumration of calendar user types
    /// </summary>
    public enum CUTYPE { UNKNOWN, INDIVIDUAL, GROUP, RESOURCE, ROOM }

    public enum BOOLEAN { UNKNOWN, TRUE, FALSE }

    /// <summary>
    /// Represents the icalendar role types
    /// </summary>
    public enum ROLE { UNKNOWN, CHAIR, REQ_PARTICIPANT, OPT_PARTICIPANT, NON_PARTICIPANT }

    /// <summary>
    /// Represents participation status of a calendar user
    /// </summary>
    public enum PARTSTAT { UNKNOWN, NEEDS_ACTION, ACCEPTED, DECLINED, TENTATIVE, DELEGATED, COMPLETED, IN_PROGRESS }

    /// <summary>
    /// Represents an enumeration for the range paramter of the icalendar
    /// </summary>
    public enum RANGE { UNKNOWN, THIS, THISANDFUTURE, ALL }

    /// <summary>
    /// Represents the frequency type of an icalendar component
    /// </summary>
    public enum FREQ { UNKNOWN, SECONDLY, MINUTELY, HOURLY, DAILY, WEEKLY, MONTHLY, YEARLY }

    /// <summary>
    /// Represents the type of day in the week
    /// </summary>
    public enum WEEKDAY { UNKNOWN, SU, MO, TU, WE, TH, FR, SA }

    /// <summary>
    /// Represents the type of of access classification
    /// </summary>
    public enum CLASS { UNKNOWN, PUBLIC, PRIVATE, CONFIDENTIAL, }

    /// <summary>
    /// Represents the type of hierachical relationship between components.
    /// </summary>
    public enum RELTYPE { UNKNOWN, PARENT, CHILD, SIBLING }

    /// <summary>
    /// Represents the type of alarm trigger relationship
    /// </summary>
    public enum RELATED { UNKNOWN, START, END }

    /// <summary>
    /// Represents the transparency of an event to busy time searches
    /// </summary>
    public enum TRANSP { UNKNOWN, OPAQUE, TRANSPARENT }

    /// <summary>
    /// Represents the overall status or confirmation for a calendar component
    /// </summary>
    public enum STATUS { UNKNOWN, TENTATIVE, CONFIRMED, CANCELLED, NEEDS_ACTION, COMPLETED, IN_PROCESS, DRAFT, FINAL }

    /// <summary>
    /// Represents the relative priority for a calendar component with a three-level priority scheme
    /// </summary>
    public enum PRIORITYLEVEL { UNKNOWN, LOW, MEDIUM, HIGH }

    /// <summary>
    /// Represents the relative priority for a calendar component with a priority Schema
    /// </summary>
    public enum PRIORITYSCHEMA { UNKNOWN, A1, A2, A3, B1, B2, B3, C1, C2, C3 }

    /// <summary>
    /// Represents the action to be invoked when an alarm is triggered
    /// </summary>
    public enum ACTION { UNKNOWN, AUDIO, DISPLAY, EMAIL }

    /// <summary>
    /// Represents the type of sign
    /// </summary>
    public enum SignType { Neutral, Negative, Positive }

    /// <summary>
    /// Represents explicitly  the value type format for a property value
    /// </summary>
    public enum VALUE
    {
        /// <summary>
        /// Unknown value type
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// Binary value type
        /// </summary>
        BINARY,

        /// <summary>
        /// Boolean value type
        /// </summary>
        BOOLEAN,

        /// <summary>
        /// The calendar address value type
        /// </summary>
        CAL_ADDRESS,

        /// <summary>
        /// Date value type
        /// </summary>
        DATE,

        /// <summary>
        /// The date time value type
        /// </summary>
        DATE_TIME,

        /// <summary>
        /// Duration value type
        /// </summary>
        DURATION,

        /// <summary>
        /// Floating number value type
        /// </summary>
        FLOAT,

        /// <summary>
        /// Integer value type
        /// </summary>
        INTEGER,

        /// <summary>
        /// Period of time value type
        /// </summary>
        PERIOD,

        /// <summary>
        /// Recurrence value type
        /// </summary>
        RECUR,

        /// <summary>
        /// Time value type
        /// </summary>
        TIME,

        /// <summary>
        /// Uniform Resource Identifier (URI) value type
        /// </summary>
        URI,

        /// <summary>
        /// Coordinated Universal Time (french - UTC) offset type
        /// </summary>
        UTC_OFFSET
    }

    /// <summary>
    /// Represents the complete representation basic format for a period of time.
    /// </summary>
    public enum PeriodType
    {
        /// <summary>
        /// The complete representation basic format for a period of time consisting of a start and end.
        /// </summary>
        Explicit,

        /// <summary>
        /// The complete representation basic format for a period of time consisting of a start and positive duration of time.
        /// </summary>
        Start
    }

    /// <summary>
    /// Represents the trigger edge, prior to the start of an event or to-do.
    /// </summary>
    public enum TriggerType
    {
        /// <summary>
        /// The trigger edge is explicitly set to be relative to the Start or End of the event or to do with the Related parameter of the Trigger property
        /// </summary>
        Related,

        /// <summary>
        /// The trigger edge is explicitly set to an absolute calendar time with UTC time.
        /// </summary>
        Absolute
    }

    /// <summary>
    /// Represents the type of scale used to define the relative priority for a calendar component
    /// </summary>
    public enum PriorityType { Integral, Level, Schema }
}