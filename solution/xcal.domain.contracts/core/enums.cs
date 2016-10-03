namespace xcal.domain.contracts.core
{
    /// <summary>
    /// Represents the calendar type used top scale schedules of the iCalendar object.
    /// </summary>
    public enum CALSCALE
    {
        /// <summary>
        /// Not applicable marker to indicate no scale has been selected.
        /// </summary>
        NONE,

        /// <summary>
        /// Represents Gregorian (Western/Christian) calendar.
        /// </summary>
        GREGORIAN,

        /// <summary>
        /// Represents the Hebrew (Jewish) lunisolar calendar.
        /// </summary>
        HEBREW,

        /// <summary>
        /// Represents the Islamic (Persian/Hijri) lunar calendar.
        /// </summary>
        ISLAMIC,

        /// <summary>
        /// Represents the Hindu calendar.
        /// </summary>
        INDIAN,

        /// <summary>
        /// Represents the Chinese calendar.
        /// </summary>
        CHINESE,

        /// <summary>
        /// Represents the Julian calendar.
        /// </summary>
        JULIAN
    }

    /// <summary>
    /// Represents iCalendar object method associated with the calendar object.
    /// </summary>
    public enum METHOD
    {
        NONE,
        REQUEST,
        REPLY,
        REFRESH,
        PUBLISH,
        CANCEL,
        ADD,
        COUNTER,
        DECLINECOUNTER
    }

    /// <summary>
    /// Represents the type of iCalendar encodings
    /// </summary>
    public enum ENCODING
    {
        BIT8,
        BASE64
    }

    public enum TimeType
    {
        NONE,
        Local,
        Utc,
        LocalAndTimeZone
    }

    /// <summary>
    /// Represents the type of Free Busy Time
    /// </summary>
    public enum FBTYPE
    {
        NONE,
        FREE,
        BUSY,
        BUSY_UNAVAILABLE,
        BUSY_TENTATIVE
    }

    /// <summary>
    /// Represents an enumration of calendar user types
    /// </summary>
    public enum CUTYPE
    {
        NONE,
        UNKNOWN,
        INDIVIDUAL,
        GROUP,
        RESOURCE,
        ROOM
    }

    public enum BOOLEAN
    {
        TRUE,
        FALSE
    }

    /// <summary>
    /// Represents the icalendar role types
    /// </summary>
    public enum ROLE
    {
        NONE,
        CHAIR,
        REQ_PARTICIPANT,
        OPT_PARTICIPANT,
        NON_PARTICIPANT
    }

    /// <summary>
    /// Represents participation status of a calendar user
    /// </summary>
    public enum PARTSTAT
    {
        NONE,
        NEEDS_ACTION,
        ACCEPTED,
        DECLINED,
        TENTATIVE,
        DELEGATED,
        COMPLETED,
        IN_PROGRESS
    }

    /// <summary>
    /// Represents an enumeration for the range paramter of the icalendar
    /// </summary>
    public enum RANGE
    {
        NONE,
        THISANDFUTURE
    }

    /// <summary>
    /// Represents the frequency type of an icalendar component
    /// </summary>
    public enum FREQ
    {
        NONE,
        SECONDLY,
        MINUTELY,
        HOURLY,
        DAILY,
        WEEKLY,
        MONTHLY,
        YEARLY
    }

    /// <summary>
    /// Represents the type of day in the week
    /// </summary>
    public enum WEEKDAY
    {
        /// <summary>
        /// A not applicable marker to indicate non-selection of the week day.
        /// </summary>
        NONE,

        /// <summary>
        /// Represents the day of the week Sunday.
        /// </summary>
        SU,

        /// <summary>
        /// Represents the day of the week Monday.
        /// </summary>
        MO,

        /// <summary>
        /// Represents the day of the week Tuesday.
        /// </summary>
        TU,

        /// <summary>
        /// Represents the day of the week Wednesday.
        /// </summary>
        WE,

        /// <summary>
        /// Represents the day of the week Thursday.
        /// </summary>
        TH,

        /// <summary>
        /// Represents the day of the week Friday.
        /// </summary>
        FR,

        /// <summary>
        /// Represents the day of the week Saturday.
        /// </summary>
        SA
    }

    /// <summary>
    /// Represents the type of of access classification
    /// </summary>
    public enum CLASS { NONE, PUBLIC, PRIVATE, CONFIDENTIAL }

    /// <summary>
    /// Represents the type of hierachical relationship between components.
    /// </summary>
    public enum RELTYPE { NONE, PARENT, CHILD, SIBLING }

    /// <summary>
    /// Represents the type of alarm trigger relationship
    /// </summary>
    public enum RELATED { NONE, START, END }

    /// <summary>
    /// Represents the transparency of an event to busy time searches
    /// </summary>
    public enum TRANSP { NONE, OPAQUE, TRANSPARENT }

    /// <summary>
    /// Represents the overall status or confirmation for a calendar component
    /// </summary>
    public enum STATUS { NONE, TENTATIVE, CONFIRMED, CANCELLED, NEEDS_ACTION, COMPLETED, IN_PROCESS, DRAFT, FINAL }

    /// <summary>
    /// Represents the relative priority for a calendar component with a three-level priority scheme
    /// </summary>
    public enum PRIORITYLEVEL { NONE, LOW, MEDIUM, HIGH }

    /// <summary>
    /// Represents the relative priority for a calendar component with a priority Schema
    /// </summary>
    public enum PRIORITYSCHEMA { NONE, A1, A2, A3, B1, B2, B3, C1, C2, C3 }

    /// <summary>
    /// Represents the action to be invoked when an alarm is triggered
    /// </summary>
    public enum ACTION { AUDIO, DISPLAY, EMAIL }

    /// <summary>
    /// Represents explicitly  the value type format for a property value
    /// </summary>
    public enum VALUE
    {
        /// <summary>
        /// Unknown value type
        /// </summary>
        NONE,

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
        /// Text value type
        /// </summary>
        TEXT,

        /// <summary>
        /// Uniform Resource Identifier (URI) value type
        /// </summary>
        URI,

        /// <summary>
        /// Coordinated Universal Time (french - UTC) offset type
        /// </summary>
        UTC_OFFSET
    }

    public enum IANA_COMPONENT_TOKEN
    {
        NONE,
        VCALENDAR,
        VEVENT,
        VTODO,
        VJOURNAL,
        VFREEBUSY,
        VTIMEZONE,
        VALARM,
        STANDARD,
        DAYLIGHT,
    }

    public enum IANA_PROPERTY_TOKEN
    {
        NONE,
        CALSCALE,
        METHOD,
        PRODID,
        VERSION,
        ATTACH,
        CATEGORIES,
        CLASS,
        COMMENT,
        DESCRIPTION,
        GEO,
        LOCATION,
        PERCENT_COMPLETE,
        PRIORITY,
        RESOURCES,
        STATUS,
        SUMMARY,
        COMPLETED,
        DTEND,
        DUE,
        DTSTART,
        DURATION,
        FREEBUSY,
        TRANSP,
        TZID,
        TZNAME,
        TZOFFSETFROM,
        TZOFFSETTO,
        TZURL,
        ATTENDEE,
        CONTACT,
        ORGANIZER,
        RECURRENCE_ID,
        RELATED_TO,
        URL,
        UID,
        EXDATE,
        EXRULE,
        RDATE,
        RRULE,
        ACTION,
        REPEAT,
        TRIGGER,
        CREATED,
        DTSTAMP,
        LAST_MODIFIED,
        SEQUENCE,
        REQUEST_STATUS
    }

    public enum IANA_PARAMETER_TOKEN
    {
        NONE,
        ALTREP,
        CN,
        CUTYPE,
        DELEGATED_FROM,
        DELEGATED_TO,
        DIR,
        ENCODING,
        FMTTYPE,
        FBTYPE,
        LANGUAGE,
        MEMBER,
        PARTSTAT,
        RANGE,
        RELATED,
        RELTYPE,
        ROLE,
        RSVP,
        SENT_BY,
        TZID,
        VALUE
    }

    public enum IANA_VALUE_TOKEN
    {
        NONE,
        BINARY,
        BOOLEAN,
        CAL_ADDRESS,
        DATE,
        DATE_TIME,
        DURATION,
        FLOAT,
        INTEGER,
        PERIOD,
        RECUR,
        TEXT,
        TIME,
        URI,
        UTC_OFFSET
    }

    public enum IANA_CUTYPE_TOKEN
    {
        NONE,
        INDIVIDUAL,
        GROUP,
        RESOURCE,
        ROOM,
        UNKNOWN
    }

    public enum IANA_FREEBUSY_TOKEN
    {
        NONE,
        FREE,
        BUSY,
        BUSY_UNAVAILABLE,
        BUSY_TENTATIVE,
    }

    public enum IANA_PARTSTAT_TOKEN
    {
        NONE,
        NEEDS_ACTION,
        ACCEPTED,
        DECLINED,
        TENTATIVE,
        DELEGATED,
        IN_PROCESS
    }

    public enum IANA_RELTYPE_TOKEN
    {
        NONE,
        CHILD,
        PARENT,
        SIBLING
    }

    public enum IANA_ROLE_TOKEN
    {
        NONE,
        CHAIR,
        REQ_PARTICIPANT,
        OPT_PARTICIPANT,
        NON_PARTICIPANT,
    }

    public enum IANA_ACTION_TOKEN
    {
        AUDIO,
        DISPLAY,
        EMAIL,
        PROCEDURE
    }

    public enum IANA_CLASS_TOKEN
    {
        NONE,
        PUBLIC,
        PRIVATE,
        CONFIDENTIAL
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
    public enum PriorityType
    {
        Integral,
        Level,
        Schema
    }
}
