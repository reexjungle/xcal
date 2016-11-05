namespace reexjungle.xcal.core.domain.contracts.io.readers
{
    /// <summary>
    /// Specifies the type of icalendar fragments.
    /// </summary>
    public enum NodeType
    {

        /// <summary>
        /// Represents an unknown calendar node type
        /// </summary>
        NONE,

        /// <summary>
        /// Represents the start of a <see cref="COMPONENT"/> or <see cref="CALENDAR"/> node.
        /// </summary>
        BEGIN,

        /// <summary>
        /// Represents the end of a <see cref="COMPONENT"/> or <see cref="CALENDAR"/> node.
        /// </summary>
        END,

        /// <summary>
        /// Represents a calendar object e.g. VCALENDAR
        /// </summary>
        CALENDAR,

        /// <summary>
        /// Represents an iCalendar component e.g. VEVENT
        /// </summary>
        COMPONENT,

        /// <summary>
        /// Represents an icalendar PROPERTY e.g. ATTENDEE.
        /// </summary>
        PROPERTY,

        /// <summary>
        /// Represents an iCalendar PARAMETER e.g. TZID.
        /// </summary>
        PARAMETER,

        /// <summary>
        /// Represents an iCalendar VALUE e.g. DATE-TIME.
        /// </summary>
        VALUE,

    }

    public enum ReadState
    {
        IDLE,
        READING,
        ERROR,
        EOF,
        CLOSED
    }
}
