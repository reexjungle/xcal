using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;

namespace reexjungle.xcal.service.repositories.concretes.relations
{
    public class REL_CALENDARS_EVENTS : IEquatable<REL_CALENDARS_EVENTS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-event relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        public bool Equals(REL_CALENDARS_EVENTS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CalendarId == other.CalendarId && EventId == other.EventId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_CALENDARS_EVENTS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CalendarId.GetHashCode() * 397) ^ EventId.GetHashCode();
            }
        }

        public static bool operator ==(REL_CALENDARS_EVENTS left, REL_CALENDARS_EVENTS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_CALENDARS_EVENTS left, REL_CALENDARS_EVENTS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_CALENDARS_TODOS : IEquatable<REL_CALENDARS_TODOS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-todo relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related todo entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        public bool Equals(REL_CALENDARS_TODOS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CalendarId == other.CalendarId && TodoId == other.TodoId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_CALENDARS_TODOS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CalendarId.GetHashCode() * 397) ^ TodoId.GetHashCode();
            }
        }

        public static bool operator ==(REL_CALENDARS_TODOS left, REL_CALENDARS_TODOS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_CALENDARS_TODOS left, REL_CALENDARS_TODOS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_CALENDARS_FREEBUSIES : IEquatable<REL_CALENDARS_FREEBUSIES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-free-busy relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related free-busy entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid FreeBusyId { get; set; }

        public bool Equals(REL_CALENDARS_FREEBUSIES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CalendarId == other.CalendarId && FreeBusyId == other.FreeBusyId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_CALENDARS_FREEBUSIES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CalendarId.GetHashCode() * 397) ^ FreeBusyId.GetHashCode();
            }
        }

        public static bool operator ==(REL_CALENDARS_FREEBUSIES left, REL_CALENDARS_FREEBUSIES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_CALENDARS_FREEBUSIES left, REL_CALENDARS_FREEBUSIES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_CALENDARS_JOURNALS : IEquatable<REL_CALENDARS_JOURNALS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-journal relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related journal entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        public bool Equals(REL_CALENDARS_JOURNALS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CalendarId == other.CalendarId && JournalId == other.JournalId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_CALENDARS_JOURNALS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CalendarId.GetHashCode() * 397) ^ JournalId.GetHashCode();
            }
        }

        public static bool operator ==(REL_CALENDARS_JOURNALS left, REL_CALENDARS_JOURNALS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_CALENDARS_JOURNALS left, REL_CALENDARS_JOURNALS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_CALENDARS_TIMEZONES : IEquatable<REL_CALENDARS_TIMEZONES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-time zone relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related time zone entity
        /// </summary>
        [ForeignKey(typeof(VTIMEZONE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TimeZoneId { get; set; }

        public bool Equals(REL_CALENDARS_TIMEZONES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CalendarId.Equals(other.CalendarId) && TimeZoneId.Equals(other.TimeZoneId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_CALENDARS_TIMEZONES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CalendarId.GetHashCode() * 397) ^ TimeZoneId.GetHashCode();
            }
        }

        public static bool operator ==(REL_CALENDARS_TIMEZONES left, REL_CALENDARS_TIMEZONES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_CALENDARS_TIMEZONES left, REL_CALENDARS_TIMEZONES right)
        {
            return !Equals(left, right);
        }
    }

}