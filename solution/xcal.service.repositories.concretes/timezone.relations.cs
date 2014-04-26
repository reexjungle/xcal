using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.repositories.concretes
{
    #region TimeZone relations
    
    public class REL_TIMEZONES_STANDARDS : IEquatable<REL_TIMEZONES_STANDARDS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-standard time relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VTIMEZONE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related standard time entity
        /// </summary>
        [ForeignKey(typeof(STANDARD), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string StandardId { get; set; }

        public bool Equals(REL_TIMEZONES_STANDARDS other)
        {
            if (other == null) return false;
            return (this.TimeZoneId.Equals(other.TimeZoneId, StringComparison.OrdinalIgnoreCase) &&
                this.StandardId.Equals(other.StandardId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TIMEZONES_STANDARDS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TimeZoneId.GetHashCode() ^ this.StandardId.GetHashCode();
        }

        public static bool operator ==(REL_TIMEZONES_STANDARDS x, REL_TIMEZONES_STANDARDS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TIMEZONES_STANDARDS x, REL_TIMEZONES_STANDARDS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TIMEZONES_DAYLIGHT : IEquatable<REL_TIMEZONES_DAYLIGHT>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-daylight saving changes time relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [ForeignKey(typeof(VTIMEZONE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related daylight saving changes time entity
        /// </summary>
        [ForeignKey(typeof(DAYLIGHT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string DaylightId { get; set; }

        public bool Equals(REL_TIMEZONES_DAYLIGHT other)
        {
            if (other == null) return false;
            return (this.CalendarId.Equals(other.CalendarId, StringComparison.OrdinalIgnoreCase) &&
                this.DaylightId.Equals(other.DaylightId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TIMEZONES_DAYLIGHT;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.CalendarId.GetHashCode() ^ this.DaylightId.GetHashCode();
        }

        public static bool operator ==(REL_TIMEZONES_DAYLIGHT x, REL_TIMEZONES_DAYLIGHT y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TIMEZONES_DAYLIGHT x, REL_TIMEZONES_DAYLIGHT y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    } 
    
    #endregion

    #region Standard Time relations
    
    public class REL_STANDARDS_RECURS : IEquatable<REL_STANDARDS_RECURS>
    {
        [Index(true)]
        public string Id { get; set; }

        [ForeignKey(typeof(STANDARD), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string StandardId { get; set; }


        [ForeignKey(typeof(RECUR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceId { get; set; }

        public bool Equals(REL_STANDARDS_RECURS other)
        {
            if (other == null) return false;
            return (this.StandardId.Equals(other.StandardId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceId.Equals(other.RecurrenceId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_STANDARDS_RECURS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.StandardId.GetHashCode() ^ this.RecurrenceId.GetHashCode();
        }

        public static bool operator ==(REL_STANDARDS_RECURS x, REL_STANDARDS_RECURS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_STANDARDS_RECURS x, REL_STANDARDS_RECURS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_STANDARDS_COMMENTS : IEquatable<REL_STANDARDS_COMMENTS>
    {
        [Index(true)]
        public string Id { get; set; }

        [ForeignKey(typeof(STANDARD), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string StandardId { get; set; }


        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CommentId { get; set; }

        public bool Equals(REL_STANDARDS_COMMENTS other)
        {
            if (other == null) return false;
            return (this.StandardId.Equals(other.StandardId, StringComparison.OrdinalIgnoreCase) &&
                this.CommentId.Equals(other.CommentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_STANDARDS_COMMENTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.StandardId.GetHashCode() ^ this.CommentId.GetHashCode();
        }

        public static bool operator ==(REL_STANDARDS_COMMENTS x, REL_STANDARDS_COMMENTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_STANDARDS_COMMENTS x, REL_STANDARDS_COMMENTS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_STANDARDS_RDATES : IEquatable<REL_STANDARDS_RDATES>
    {
        [Index(true)]
        public string Id { get; set; }

        [ForeignKey(typeof(STANDARD), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string StandardId { get; set; }


        [ForeignKey(typeof(RDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceDateId { get; set; }

        public bool Equals(REL_STANDARDS_RDATES other)
        {
            if (other == null) return false;
            return (this.StandardId.Equals(other.StandardId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceDateId.Equals(other.RecurrenceDateId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_STANDARDS_RDATES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.StandardId.GetHashCode() ^ this.RecurrenceDateId.GetHashCode();
        }

        public static bool operator ==(REL_STANDARDS_RDATES x, REL_STANDARDS_RDATES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_STANDARDS_RDATES x, REL_STANDARDS_RDATES y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_STANDARDS_TZNAMES : IEquatable<REL_STANDARDS_TZNAMES>
    {
        [Index(true)]
        public string Id { get; set; }

        [ForeignKey(typeof(STANDARD), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string StandardId { get; set; }


        [ForeignKey(typeof(TZNAME), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TimeZoneName { get; set; }

        public bool Equals(REL_STANDARDS_TZNAMES other)
        {
            if (other == null) return false;
            return (this.StandardId.Equals(other.StandardId, StringComparison.OrdinalIgnoreCase) &&
                this.TimeZoneName.Equals(other.TimeZoneName, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_STANDARDS_TZNAMES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.StandardId.GetHashCode() ^ this.TimeZoneName.GetHashCode();
        }

        public static bool operator ==(REL_STANDARDS_TZNAMES x, REL_STANDARDS_TZNAMES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_STANDARDS_TZNAMES x, REL_STANDARDS_TZNAMES y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    } 
    
    #endregion

    #region Daylight Time relations

    public class REL_DAYLIGHTS_RECURS : IEquatable<REL_DAYLIGHTS_RECURS>
    {
        [Index(true)]
        public string Id { get; set; }

        [ForeignKey(typeof(DAYLIGHT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string DaylightId { get; set; }


        [ForeignKey(typeof(RECUR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceId { get; set; }

        public bool Equals(REL_DAYLIGHTS_RECURS other)
        {
            if (other == null) return false;
            return (this.DaylightId.Equals(other.DaylightId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceId.Equals(other.RecurrenceId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_DAYLIGHTS_RECURS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.DaylightId.GetHashCode() ^ this.RecurrenceId.GetHashCode();
        }

        public static bool operator ==(REL_DAYLIGHTS_RECURS x, REL_DAYLIGHTS_RECURS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_DAYLIGHTS_RECURS x, REL_DAYLIGHTS_RECURS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_DAYLIGHTS_COMMENTS : IEquatable<REL_DAYLIGHTS_COMMENTS>
    {
        [Index(true)]
        public string Id { get; set; }

        [ForeignKey(typeof(DAYLIGHT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string DaylightId { get; set; }

        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CommentId { get; set; }

        public bool Equals(REL_DAYLIGHTS_COMMENTS other)
        {
            if (other == null) return false;
            return (this.DaylightId.Equals(other.DaylightId, StringComparison.OrdinalIgnoreCase) &&
                this.CommentId.Equals(other.CommentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_DAYLIGHTS_COMMENTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.DaylightId.GetHashCode() ^ this.CommentId.GetHashCode();
        }

        public static bool operator ==(REL_DAYLIGHTS_COMMENTS x, REL_DAYLIGHTS_COMMENTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_DAYLIGHTS_COMMENTS x, REL_DAYLIGHTS_COMMENTS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_DAYLIGHTS_RDATES : IEquatable<REL_DAYLIGHTS_RDATES>
    {
        [Index(true)]
        public string Id { get; set; }

        [ForeignKey(typeof(DAYLIGHT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string DaylightId { get; set; }


        [ForeignKey(typeof(RDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceDateId { get; set; }

        public bool Equals(REL_DAYLIGHTS_RDATES other)
        {
            if (other == null) return false;
            return (this.DaylightId.Equals(other.DaylightId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceDateId.Equals(other.RecurrenceDateId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_DAYLIGHTS_RDATES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.DaylightId.GetHashCode() ^ this.RecurrenceDateId.GetHashCode();
        }

        public static bool operator ==(REL_DAYLIGHTS_RDATES x, REL_DAYLIGHTS_RDATES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_DAYLIGHTS_RDATES x, REL_DAYLIGHTS_RDATES y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_DAYLIGHTS_TZNAMES : IEquatable<REL_DAYLIGHTS_TZNAMES>
    {
        [Index(true)]
        public string Id { get; set; }

        [ForeignKey(typeof(DAYLIGHT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string DaylightId { get; set; }


        [ForeignKey(typeof(TZNAME), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TimeZoneName { get; set; }

        public bool Equals(REL_DAYLIGHTS_TZNAMES other)
        {
            if (other == null) return false;
            return (this.DaylightId.Equals(other.DaylightId, StringComparison.OrdinalIgnoreCase) &&
                this.TimeZoneName.Equals(other.TimeZoneName, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_DAYLIGHTS_TZNAMES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.DaylightId.GetHashCode() ^ this.TimeZoneName.GetHashCode();
        }

        public static bool operator ==(REL_DAYLIGHTS_TZNAMES x, REL_DAYLIGHTS_TZNAMES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_DAYLIGHTS_TZNAMES x, REL_DAYLIGHTS_TZNAMES y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    #endregion

}
