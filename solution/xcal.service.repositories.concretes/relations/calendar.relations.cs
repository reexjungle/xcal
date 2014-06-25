using System;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;
using reexmonkey.xcal.domain.models;
using reexmonkey.foundation.essentials.contracts;

namespace reexmonkey.xcal.service.repositories.concretes.relations
{
    public class REL_CALENDARS_EVENTS : IEquatable<REL_CALENDARS_EVENTS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-event relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        public bool Equals(REL_CALENDARS_EVENTS other)
        {
            if (other == null) return false;
            return (this.CalendarId.Equals(other.CalendarId, StringComparison.OrdinalIgnoreCase) &&
                this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_CALENDARS_EVENTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.CalendarId.GetHashCode() ^ this.EventId.GetHashCode();
        }

        public static bool operator ==(REL_CALENDARS_EVENTS x, REL_CALENDARS_EVENTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_CALENDARS_EVENTS x, REL_CALENDARS_EVENTS y)
        {
            if ((object)x == null || (object)y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_CALENDARS_TODOS: IEquatable<REL_CALENDARS_TODOS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-todo relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related todo entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        public bool Equals(REL_CALENDARS_TODOS other)
        {
            if (other == null) return false;
            return (this.CalendarId.Equals(other.CalendarId, StringComparison.OrdinalIgnoreCase) &&
                this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_CALENDARS_TODOS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.CalendarId.GetHashCode() ^ this.TodoId.GetHashCode();
        }

        public static bool operator ==(REL_CALENDARS_TODOS x, REL_CALENDARS_TODOS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_CALENDARS_TODOS x, REL_CALENDARS_TODOS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_CALENDARS_FREEBUSIES : IEquatable<REL_CALENDARS_FREEBUSIES>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-free-busy relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related free-busy entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string FreeBusyId { get; set; }

        public bool Equals(REL_CALENDARS_FREEBUSIES other)
        {
            if (other == null) return false;
            return (this.CalendarId.Equals(other.CalendarId, StringComparison.OrdinalIgnoreCase) &&
                this.FreeBusyId.Equals(other.FreeBusyId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_CALENDARS_FREEBUSIES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.CalendarId.GetHashCode() ^ this.FreeBusyId.GetHashCode();
        }

        public static bool operator ==(REL_CALENDARS_FREEBUSIES x, REL_CALENDARS_FREEBUSIES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_CALENDARS_FREEBUSIES x, REL_CALENDARS_FREEBUSIES y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_CALENDARS_JOURNALS : IEquatable<REL_CALENDARS_JOURNALS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-journal relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related journal entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string JournalId { get; set; }

        public bool Equals(REL_CALENDARS_JOURNALS other)
        {
            if (other == null) return false;
            return (this.CalendarId.Equals(other.CalendarId, StringComparison.OrdinalIgnoreCase) &&
                this.JournalId.Equals(other.JournalId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_CALENDARS_JOURNALS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.CalendarId.GetHashCode() ^ this.JournalId.GetHashCode();
        }

        public static bool operator ==(REL_CALENDARS_JOURNALS x, REL_CALENDARS_JOURNALS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_CALENDARS_JOURNALS x, REL_CALENDARS_JOURNALS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_CALENDARS_TIMEZONES : IEquatable<REL_CALENDARS_TIMEZONES>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-time zone relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related time zone entity
        /// </summary>
        [ForeignKey(typeof(VTIMEZONE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TimeZoneId { get; set; }

        public bool Equals(REL_CALENDARS_TIMEZONES other)
        {
            if (other == null) return false;
            return (this.CalendarId.Equals(other.CalendarId, StringComparison.OrdinalIgnoreCase) &&
                this.TimeZoneId.Equals(other.TimeZoneId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_CALENDARS_TIMEZONES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.CalendarId.GetHashCode() ^ this.TimeZoneId.GetHashCode();
        }

        public static bool operator ==(REL_CALENDARS_TIMEZONES x, REL_CALENDARS_TIMEZONES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_CALENDARS_TIMEZONES x, REL_CALENDARS_TIMEZONES y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_CALENDARS_IANAC : IEquatable<REL_CALENDARS_IANAC>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-time zone relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related time zone entity
        /// </summary>
        [ForeignKey(typeof(IANA_COMPONENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string IanaId { get; set; }

        public bool Equals(REL_CALENDARS_IANAC other)
        {
            if (other == null) return false;
            return (this.CalendarId.Equals(other.CalendarId, StringComparison.OrdinalIgnoreCase) &&
                this.IanaId.Equals(other.IanaId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_CALENDARS_IANAC;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.CalendarId.GetHashCode() ^ this.IanaId.GetHashCode();
        }

        public static bool operator ==(REL_CALENDARS_IANAC x, REL_CALENDARS_IANAC y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_CALENDARS_IANAC x, REL_CALENDARS_IANAC y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_CALENDARS_XC : IEquatable<REL_CALENDARS_XC>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-x-component relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related x-component entity
        /// </summary>
        [ForeignKey(typeof(XCOMPONENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string XComponentId { get; set; }

        public bool Equals(REL_CALENDARS_XC other)
        {
            if (other == null) return false;
            return (this.CalendarId.Equals(other.CalendarId, StringComparison.OrdinalIgnoreCase) &&
                this.XComponentId.Equals(other.XComponentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_CALENDARS_XC;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.CalendarId.GetHashCode() ^ this.XComponentId.GetHashCode();
        }

        public static bool operator ==(REL_CALENDARS_XC x, REL_CALENDARS_XC y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_CALENDARS_XC x, REL_CALENDARS_XC y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }
}
