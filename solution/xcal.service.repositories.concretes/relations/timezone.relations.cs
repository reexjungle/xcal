using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;

namespace reexjungle.xcal.service.repositories.concretes.relations
{
    #region TimeZone relations

    public class REL_TIMEZONES_STANDARDS : IEquatable<REL_TIMEZONES_STANDARDS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-standard time relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VTIMEZONE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related standard time entity
        /// </summary>
        [ForeignKey(typeof(STANDARD), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid StandardId { get; set; }

        public bool Equals(REL_TIMEZONES_STANDARDS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TimeZoneId.Equals(other.TimeZoneId) && StandardId.Equals(other.StandardId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TIMEZONES_STANDARDS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TimeZoneId.GetHashCode() * 397) ^ StandardId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TIMEZONES_STANDARDS left, REL_TIMEZONES_STANDARDS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TIMEZONES_STANDARDS left, REL_TIMEZONES_STANDARDS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TIMEZONES_DAYLIGHT : IEquatable<REL_TIMEZONES_DAYLIGHT>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-daylight saving changes time relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VTIMEZONE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related daylight saving changes time entity
        /// </summary>
        [ForeignKey(typeof(DAYLIGHT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid DaylightId { get; set; }

        public bool Equals(REL_TIMEZONES_DAYLIGHT other)
        {
            if (other == null) return false;
            return (CalendarId.Equals(other.CalendarId) &&
                DaylightId.Equals(other.DaylightId));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TIMEZONES_DAYLIGHT;
            if (rel == null) return false;
            return Equals(rel);
        }

        public override int GetHashCode()
        {
            return CalendarId.GetHashCode() ^ DaylightId.GetHashCode();
        }

        public static bool operator ==(REL_TIMEZONES_DAYLIGHT x, REL_TIMEZONES_DAYLIGHT y)
        {
            if ((object)x == null || (object)y == null) return Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TIMEZONES_DAYLIGHT x, REL_TIMEZONES_DAYLIGHT y)
        {
            if (x == null || y == null) return !Equals(x, y);
            return !x.Equals(y);
        }
    }

    #endregion TimeZone relations

    #region Standard Time relations

    public class REL_STANDARDS_RECURS : IEquatable<REL_STANDARDS_RECURS>, IContainsKey<Guid>
    {
        [Index(true)]
        public Guid Id { get; set; }

        [ForeignKey(typeof(STANDARD), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid StandardId { get; set; }

        [ForeignKey(typeof(RECUR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RecurId { get; set; }

        public bool Equals(REL_STANDARDS_RECURS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StandardId == other.StandardId && RecurId == other.RecurId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_STANDARDS_RECURS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StandardId.GetHashCode() * 397) ^ RecurId.GetHashCode();
            }
        }

        public static bool operator ==(REL_STANDARDS_RECURS left, REL_STANDARDS_RECURS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_STANDARDS_RECURS left, REL_STANDARDS_RECURS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_STANDARDS_COMMENTS : IEquatable<REL_STANDARDS_COMMENTS>, IContainsKey<Guid>
    {
        [Index(true)]
        public Guid Id { get; set; }

        [ForeignKey(typeof(STANDARD), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid StandardId { get; set; }

        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CommentId { get; set; }

        public bool Equals(REL_STANDARDS_COMMENTS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(StandardId, other.StandardId) && CommentId.Equals(other.CommentId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_STANDARDS_COMMENTS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((StandardId != null ? StandardId.GetHashCode() : 0) * 397) ^ CommentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_STANDARDS_COMMENTS left, REL_STANDARDS_COMMENTS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_STANDARDS_COMMENTS left, REL_STANDARDS_COMMENTS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_STANDARDS_RDATES : IEquatable<REL_STANDARDS_RDATES>, IContainsKey<Guid>
    {
        [Index(true)]
        public Guid Id { get; set; }

        [ForeignKey(typeof(STANDARD), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid StandardId { get; set; }

        [ForeignKey(typeof(RDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RecurrenceDateId { get; set; }

        public bool Equals(REL_STANDARDS_RDATES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StandardId.Equals(other.StandardId) && RecurrenceDateId.Equals(other.RecurrenceDateId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_STANDARDS_RDATES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StandardId.GetHashCode() * 397) ^ RecurrenceDateId.GetHashCode();
            }
        }

        public static bool operator ==(REL_STANDARDS_RDATES left, REL_STANDARDS_RDATES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_STANDARDS_RDATES left, REL_STANDARDS_RDATES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_STANDARDS_TZNAMES : IEquatable<REL_STANDARDS_TZNAMES>, IContainsKey<Guid>
    {
        [Index(true)]
        public Guid Id { get; set; }

        [ForeignKey(typeof(STANDARD), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid StandardId { get; set; }

        [ForeignKey(typeof(TZNAME), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TimeZoneNameId { get; set; }

        public bool Equals(REL_STANDARDS_TZNAMES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StandardId.Equals(other.StandardId) && TimeZoneNameId.Equals(other.TimeZoneNameId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_STANDARDS_TZNAMES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StandardId.GetHashCode() * 397) ^ TimeZoneNameId.GetHashCode();
            }
        }

        public static bool operator ==(REL_STANDARDS_TZNAMES left, REL_STANDARDS_TZNAMES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_STANDARDS_TZNAMES left, REL_STANDARDS_TZNAMES right)
        {
            return !Equals(left, right);
        }
    }

    #endregion Standard Time relations

    #region Daylight Time relations

    public class REL_DAYLIGHT_RECURS : IEquatable<REL_DAYLIGHT_RECURS>, IContainsKey<Guid>
    {
        [Index(true)]
        public Guid Id { get; set; }

        [ForeignKey(typeof(DAYLIGHT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid DaylightId { get; set; }

        [ForeignKey(typeof(RECUR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RecurId { get; set; }

        public bool Equals(REL_DAYLIGHT_RECURS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DaylightId.Equals(other.DaylightId) && RecurId.Equals(other.RecurId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_DAYLIGHT_RECURS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DaylightId.GetHashCode() * 397) ^ RecurId.GetHashCode();
            }
        }

        public static bool operator ==(REL_DAYLIGHT_RECURS left, REL_DAYLIGHT_RECURS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_DAYLIGHT_RECURS left, REL_DAYLIGHT_RECURS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_DAYLIGHTS_COMMENTS : IEquatable<REL_DAYLIGHTS_COMMENTS>, IContainsKey<Guid>
    {
        [Index(true)]
        public Guid Id { get; set; }

        [ForeignKey(typeof(DAYLIGHT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid DaylightId { get; set; }

        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CommentId { get; set; }

        public bool Equals(REL_DAYLIGHTS_COMMENTS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DaylightId == other.DaylightId && CommentId == other.CommentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_DAYLIGHTS_COMMENTS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DaylightId.GetHashCode() * 397) ^ CommentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_DAYLIGHTS_COMMENTS left, REL_DAYLIGHTS_COMMENTS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_DAYLIGHTS_COMMENTS left, REL_DAYLIGHTS_COMMENTS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_DAYLIGHTS_RDATES : IEquatable<REL_DAYLIGHTS_RDATES>, IContainsKey<Guid>
    {
        [Index(true)]
        public Guid Id { get; set; }

        [ForeignKey(typeof(DAYLIGHT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid DaylightId { get; set; }

        [ForeignKey(typeof(RDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RecurrenceDateId { get; set; }

        public bool Equals(REL_DAYLIGHTS_RDATES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DaylightId == other.DaylightId && RecurrenceDateId == other.RecurrenceDateId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_DAYLIGHTS_RDATES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DaylightId.GetHashCode() * 397) ^ RecurrenceDateId.GetHashCode();
            }
        }

        public static bool operator ==(REL_DAYLIGHTS_RDATES left, REL_DAYLIGHTS_RDATES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_DAYLIGHTS_RDATES left, REL_DAYLIGHTS_RDATES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_DAYLIGHTS_TZNAMES : IEquatable<REL_DAYLIGHTS_TZNAMES>, IContainsKey<Guid>
    {
        [Index(true)]
        public Guid Id { get; set; }

        [ForeignKey(typeof(DAYLIGHT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid DaylightId { get; set; }

        [ForeignKey(typeof(TZNAME), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TimeZoneNameId { get; set; }

        public bool Equals(REL_DAYLIGHTS_TZNAMES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DaylightId.Equals(other.DaylightId) && TimeZoneNameId.Equals(other.TimeZoneNameId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_DAYLIGHTS_TZNAMES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DaylightId.GetHashCode() * 397) ^ TimeZoneNameId.GetHashCode();
            }
        }

        public static bool operator ==(REL_DAYLIGHTS_TZNAMES left, REL_DAYLIGHTS_TZNAMES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_DAYLIGHTS_TZNAMES left, REL_DAYLIGHTS_TZNAMES right)
        {
            return !Equals(left, right);
        }
    }

    #endregion Daylight Time relations
}