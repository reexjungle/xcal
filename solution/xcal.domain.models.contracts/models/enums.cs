namespace reexjungle.xcal.core.domain.contracts.models
{

    /// <summary>
    ///Represents the calendar scale used for the calendar information.
    /// </summary>
    public enum CALSCALE
    {
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
    /// Represents the method associated with the calendar object.
    /// </summary>
    public enum METHOD
    {
        /// <summary>
        /// Make a request for a calendar component.
        /// </summary>
        REQUEST,

        /// <summary>
        /// Reply to a calendar component request.
        /// </summary>
        REPLY,

        /// <summary>
        /// A request is sent to a requestee by a requestor 
        /// asking for the latest version of a calendar component to be resent to the requester.
        /// </summary>
        REFRESH,

        /// <summary>
        /// Post notification of a calendar component.
        /// </summary>
        PUBLISH,

        /// <summary>
        /// Cancel one or more instances of an existing calendar component.
        /// </summary>
        CANCEL,

        /// <summary>
        /// Add one or more instances to an existing calendar component.
        /// </summary>
        ADD,

        /// <summary>
        /// Counter a <see cref="REQUEST"/> with an alternative proposal.
        /// </summary>
        COUNTER,

        /// <summary>
        /// Decline a counter proposal.
        /// </summary>
        DECLINECOUNTER
    }

    /// <summary>
    /// Represents the alternate inline encoding used in a textual value.
    /// </summary>
    public enum ENCODING
    {
        /// <summary>
        /// 8-Bit encoding format as specified by RFC2045 [<see cref="https://tools.ietf.org/pdf/rfc2045.pdf"/>] 
        /// </summary>
        BIT8,

        /// <summary>
        /// Base64 encoding format as specified by RFC2045 [<see cref="https://tools.ietf.org/pdf/rfc2045.pdf"/>]
        /// </summary>
        BASE64
    }

    /// <summary>
    /// Represents the form in which time values are expressed.
    /// </summary>
    public enum TIME_FORM
    {
        /// <summary>
        /// The time represented is specified as local time without a time zone reference.
        /// </summary>
        LOCAL,

        /// <summary>
        /// The time represented is specified as Coordinate Universal Time (UTC)
        /// </summary>
        UTC
    }

    /// <summary>
    /// Represents free or busy time.
    /// </summary>
    public enum FBTYPE
    {
        /// <summary>
        /// The time interval is free for scheduling. 
        /// </summary>
        FREE,

        /// <summary>
        /// The time interval is busy 
        /// because one or more activities or tasks have been scheduled for that interval.
        /// </summary>
        BUSY,

        /// <summary>
        /// The time interval is busy and that the interval can not be scheduled.
        /// </summary>
        BUSY_UNAVAILABLE,

        /// <summary>
        /// The time interval is busy 
        /// because one or more activities or tasks have been tentatively scheduled for that interval.
        /// </summary>
        BUSY_TENTATIVE
    }

    /// <summary>
    /// Identifies the type of calendar user.
    /// </summary>
    public enum CUTYPE
    {
        /// <summary>
        /// An individual.
        /// </summary>
        INDIVIDUAL,

        /// <summary>
        /// A group of individuals.
        /// </summary>
        GROUP,

        /// <summary>
        /// A physical resource.
        /// </summary>
        RESOURCE,

        /// <summary>
        /// A room resource.
        /// </summary>
        ROOM,

        /// <summary>
        /// Otherwise not known resource.
        /// </summary>
        UNKNOWN
    }

    /// <summary>
    /// Identifies properties that contain either a "TRUE" or "FALSE" Boolean value.
    /// </summary>
    public enum BOOLEAN
    {
        /// <summary>
        /// A "true" boolean value.
        /// </summary>
        TRUE,

        /// <summary>
        /// A "false" boolean value.
        /// </summary>
        FALSE
    }

    /// <summary>
    /// Represents the participation role for the calendar user.
    /// </summary>
    public enum ROLE
    {
        /// <summary>
        /// The chair of the calendar entity.
        /// </summary>
        CHAIR,

        /// <summary>
        /// A participant whose participation is required.
        /// </summary>
        REQ_PARTICIPANT,

        /// <summary>
        /// A participant whose participation is optional.
        /// </summary>
        OPT_PARTICIPANT,

        /// <summary>
        /// A participant who is copied for information purposes only.
        /// </summary>
        NON_PARTICIPANT
    }

    /// <summary>
    /// Represents the participation status for a calendar user.
    /// </summary>
    public enum PARTSTAT
    {
        /// <summary>
        /// Component activity or task needs action.
        /// </summary>
        NEEDS_ACTION,

        /// <summary>
        /// Component activity or task accepted.
        /// </summary>
        ACCEPTED,

        /// <summary>
        /// Component activity or task declined.
        /// </summary>
        DECLINED,

        /// <summary>
        /// Component activity or task tentatively accepted.
        /// </summary>
        TENTATIVE,

        /// <summary>
        /// Component activity or task delegated.
        /// </summary>
        DELEGATED,

        /// <summary>
        /// Component activity or task completed.
        /// </summary>
        COMPLETED,

        /// <summary>
        /// Component activity or task in the process of being completed.
        /// </summary>
        IN_PROCESS
    }

    /// <summary>
    /// Represents the effective range of recurrence instances from an instance 
    /// that is specified by a recurrence identifier.
    /// </summary>
    public enum RANGE
    {
        /// <summary>
        /// The instance specified by the recurrence identifier and all subsequent recurrence instances.
        /// </summary>
        THISANDFUTURE
    }

    /// <summary>
    /// Identifies the type of recurrence rule for an event.
    /// </summary>
    public enum FREQ
    {
        /// <summary>
        /// Specifies repeating events based on an interval of a second or more.
        /// </summary>
        SECONDLY,

        /// <summary>
        /// Specifies repeating events based on an interval of a minute or more.
        /// </summary>
        MINUTELY,

        /// <summary>
        /// Specifies repeating events based on an interval of an hour or more.
        /// </summary>
        HOURLY,

        /// <summary>
        /// Specifies repeating events based on an interval of a day or more.
        /// </summary>
        DAILY,

        /// <summary>
        /// Specifies repeating events based on an interval of a week or more.
        /// </summary>
        WEEKLY,

        /// <summary>
        /// Specifies repeating events based on an interval of a month or more.
        /// </summary>
        MONTHLY,

        /// <summary>
        /// Specifies repeating events based on an interval of a year or more.
        /// </summary>
        YEARLY
    }

    /// <summary>
    /// Represents the type of day in the week
    /// </summary>
    public enum WEEKDAY
    {

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
    /// Represents the the access classification for a calendar component.
    /// </summary>
    public enum CLASS
    {
        /// <summary>
        /// Public access to a calendar component.
        /// </summary>
        PUBLIC,

        /// <summary>
        /// Private access to a calendar component.
        /// </summary>
        PRIVATE,

        /// <summary>
        /// Secret access to a calendar component.
        /// </summary>
        CONFIDENTIAL
    }

    /// <summary>
    /// Represents the type of hierarchical relationship associated with the calendar component.
    /// </summary>
    public enum RELTYPE
    {
        /// <summary>
        /// Parent relationship.
        /// </summary>
        PARENT,

        /// <summary>
        /// Child relationship.
        /// </summary>
        CHILD,

        /// <summary>
        /// Sibling relationship.
        /// </summary>
        SIBLING
    }

    /// <summary>
    /// Represents the relationship of the alarm trigger with respect to the start or end of a calendar component.
    /// </summary>
    public enum RELATED
    {
        /// <summary>
        /// Trigger off of start.
        /// </summary>
        START,

        /// <summary>
        /// Trigger off of end.
        /// </summary>
        END
    }

    /// <summary>
    /// Represents whether or not an event is transparent to busy time searches.
    /// </summary>
    public enum TRANSP
    {
        /// <summary>
        /// The event is detectable by free/busy time searches.
        /// </summary>
        OPAQUE,

        /// <summary>
        /// The event is invisible to free/busy time searches.
        /// </summary>
        TRANSPARENT
    }

    /// <summary>
    /// Represents the overall status or confirmation for the calendar component.
    /// </summary>
    public enum STATUS
    {
        /// <summary>
        /// Indicates component activity or task is tentative.
        /// </summary>
        TENTATIVE,

        /// <summary>
        /// Indicates component activity or task is definite.
        /// </summary>
        CONFIRMED,

        /// <summary>
        /// Indicates component activity or task was cancelled.
        /// </summary>
        CANCELLED,

        /// <summary>
        /// Indicates component activity or task needs action.
        /// </summary>
        NEEDS_ACTION,

        /// <summary>
        /// Indicates component activity or task completed.
        /// </summary>
        COMPLETED,

        /// <summary>
        /// Indicates component activity or task in process of completion.
        /// </summary>
        IN_PROCESS,

        /// <summary>
        /// Indicates component is draft.
        /// </summary>
        DRAFT,

        /// <summary>
        /// Indicates journal is final.
        /// </summary>
        FINAL
    }

    /// <summary>
    /// Represents the relative priority for a calendar component in a three-level priority scheme.
    /// </summary>
    public enum PRIORITYLEVEL
    {
        /// <summary>
        /// No priority.
        /// </summary>
        NONE,

        /// <summary>
        /// Low priority.
        /// </summary>
        LOW,

        /// <summary>
        /// Medium priority.
        /// </summary>
        MEDIUM,

        /// <summary>
        /// Highest priority
        /// </summary>
        HIGH
    }

    /// <summary>
    /// Represents the relative priority for a calendar component in a nine-level priority Schema
    /// </summary>
    public enum PRIORITYSCHEMA
    {
        /// <summary>
        /// No priority.
        /// </summary>
        NONE,

        /// <summary>
        /// First priority level.
        /// </summary>
        A1,

        /// <summary>
        /// Second priority level.
        /// </summary>
        A2,

        /// <summary>
        /// Third priority level.
        /// </summary>
        A3,

        /// <summary>
        /// Fourth priority level.
        /// </summary>
        B1,

        /// <summary>
        /// Fifth priority level.
        /// </summary>
        B2,

        /// <summary>
        /// Sixth priority level.
        /// </summary>
        B3,

        /// <summary>
        /// Seventh priority level.
        /// </summary>
        C1,

        /// <summary>
        /// Eighth priority level.
        /// </summary>
        C2,

        /// <summary>
        /// Ninth priority level.
        /// </summary>
        C3
    }

    /// <summary>
    /// Represents the action to be invoked when an alarm is triggered
    /// </summary>
    public enum ACTION
    {
        /// <summary>
        /// Indicates an audio action e.g. a sound is played when an alarm is triggered.
        /// </summary>
        AUDIO,

        /// <summary>
        /// Indicates a display action e.g. a text to be displayed when an alarm is triggered.
        /// </summary>
        DISPLAY,

        /// <summary>
        /// Indicates an email action e.g. an email is sent when an alarm is triggered.
        /// </summary>
        EMAIL
    }

    /// <summary>
    /// Represents explicitly  the value type format for a property value.
    /// </summary>
    public enum VALUE
    {

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
