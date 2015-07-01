using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;

namespace reexjungle.xcal.service.repositories.concretes.relations
{
    public class REL_FREEBUSIES_ATTACHBINS : IEquatable<REL_FREEBUSIES_ATTACHBINS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid FreeBusyId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_FREEBUSIES_ATTACHBINS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FreeBusyId == other.FreeBusyId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_FREEBUSIES_ATTACHBINS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FreeBusyId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_FREEBUSIES_ATTACHBINS left, REL_FREEBUSIES_ATTACHBINS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_FREEBUSIES_ATTACHBINS left, REL_FREEBUSIES_ATTACHBINS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_FREEBUSIES_ATTACHURIS : IEquatable<REL_FREEBUSIES_ATTACHURIS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid FreeBusyId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_FREEBUSIES_ATTACHURIS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FreeBusyId == other.FreeBusyId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_FREEBUSIES_ATTACHURIS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FreeBusyId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_FREEBUSIES_ATTACHURIS left, REL_FREEBUSIES_ATTACHURIS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_FREEBUSIES_ATTACHURIS left, REL_FREEBUSIES_ATTACHURIS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_FREEBUSIES_ATTENDEES : IEquatable<REL_FREEBUSIES_ATTENDEES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid FreeBusyId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttendeeId { get; set; }

        public bool Equals(REL_FREEBUSIES_ATTENDEES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FreeBusyId == other.FreeBusyId && AttendeeId == other.AttendeeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_FREEBUSIES_ATTENDEES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FreeBusyId.GetHashCode() * 397) ^ AttendeeId.GetHashCode();
            }
        }

        public static bool operator ==(REL_FREEBUSIES_ATTENDEES left, REL_FREEBUSIES_ATTENDEES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_FREEBUSIES_ATTENDEES left, REL_FREEBUSIES_ATTENDEES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_FREEBUSIES_COMMENTS : IEquatable<REL_FREEBUSIES_COMMENTS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-comment relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid FreeBusyId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related comment entity
        /// </summary>
        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttendeeId { get; set; }

        public bool Equals(REL_FREEBUSIES_COMMENTS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FreeBusyId == other.FreeBusyId && AttendeeId == other.AttendeeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_FREEBUSIES_COMMENTS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FreeBusyId.GetHashCode() * 397) ^ AttendeeId.GetHashCode();
            }
        }

        public static bool operator ==(REL_FREEBUSIES_COMMENTS left, REL_FREEBUSIES_COMMENTS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_FREEBUSIES_COMMENTS left, REL_FREEBUSIES_COMMENTS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_FREEBUSIES_INFOS : IEquatable<REL_FREEBUSIES_INFOS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-contact relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid FreeBusyId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related contact entity
        /// </summary>
        [ForeignKey(typeof(FREEBUSY_INFO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid InfoId { get; set; }

        public bool Equals(REL_FREEBUSIES_INFOS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FreeBusyId == other.FreeBusyId && string.Equals(InfoId, other.InfoId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_FREEBUSIES_INFOS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FreeBusyId.GetHashCode() * 397) ^ (InfoId != null ? InfoId.GetHashCode() : 0);
            }
        }

        public static bool operator ==(REL_FREEBUSIES_INFOS left, REL_FREEBUSIES_INFOS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_FREEBUSIES_INFOS left, REL_FREEBUSIES_INFOS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_FREEBUSIES_REQSTATS : IEquatable<REL_FREEBUSIES_REQSTATS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-request status relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid FreeBusyId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related request status entity
        /// </summary>
        [ForeignKey(typeof(REQUEST_STATUS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ReqStatsId { get; set; }

        public bool Equals(REL_FREEBUSIES_REQSTATS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FreeBusyId == other.FreeBusyId && ReqStatsId == other.ReqStatsId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_FREEBUSIES_REQSTATS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FreeBusyId.GetHashCode() * 397) ^ ReqStatsId.GetHashCode();
            }
        }

        public static bool operator ==(REL_FREEBUSIES_REQSTATS left, REL_FREEBUSIES_REQSTATS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_FREEBUSIES_REQSTATS left, REL_FREEBUSIES_REQSTATS right)
        {
            return !Equals(left, right);
        }
    }
}