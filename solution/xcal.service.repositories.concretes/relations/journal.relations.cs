using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;

namespace reexjungle.xcal.service.repositories.concretes.relations
{
    public class REL_JOURNALS_ATTACHBINS : IEquatable<REL_JOURNALS_ATTACHBINS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_JOURNALS_ATTACHBINS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_ATTACHBINS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_ATTACHBINS left, REL_JOURNALS_ATTACHBINS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_ATTACHBINS left, REL_JOURNALS_ATTACHBINS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_ATTACHURIS : IEquatable<REL_JOURNALS_ATTACHURIS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_JOURNALS_ATTACHURIS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_ATTACHURIS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_ATTACHURIS left, REL_JOURNALS_ATTACHURIS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_ATTACHURIS left, REL_JOURNALS_ATTACHURIS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_RECURS : IEquatable<REL_JOURNALS_RECURS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(RECUR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RecurId { get; set; }

        public bool Equals(REL_JOURNALS_RECURS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && RecurId == other.RecurId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_RECURS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ RecurId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_RECURS left, REL_JOURNALS_RECURS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_RECURS left, REL_JOURNALS_RECURS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_ORGANIZERS : IEquatable<REL_JOURNALS_ORGANIZERS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ORGANIZER), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid OrganizerId { get; set; }

        public bool Equals(REL_JOURNALS_ORGANIZERS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && OrganizerId == other.OrganizerId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_ORGANIZERS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ OrganizerId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_ORGANIZERS left, REL_JOURNALS_ORGANIZERS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_ORGANIZERS left, REL_JOURNALS_ORGANIZERS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_ATTENDEES : IEquatable<REL_JOURNALS_ATTENDEES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttendeeId { get; set; }

        public bool Equals(REL_JOURNALS_ATTENDEES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && AttendeeId == other.AttendeeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_ATTENDEES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ AttendeeId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_ATTENDEES left, REL_JOURNALS_ATTENDEES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_ATTENDEES left, REL_JOURNALS_ATTENDEES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_COMMENTS : IEquatable<REL_JOURNALS_COMMENTS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-comment relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related comment entity
        /// </summary>
        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CommentId { get; set; }

        public bool Equals(REL_JOURNALS_COMMENTS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && CommentId == other.CommentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_COMMENTS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ CommentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_COMMENTS left, REL_JOURNALS_COMMENTS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_COMMENTS left, REL_JOURNALS_COMMENTS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_CONTACTS : IEquatable<REL_JOURNALS_CONTACTS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-contact relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related contact entity
        /// </summary>
        [ForeignKey(typeof(CONTACT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ContactId { get; set; }

        public bool Equals(REL_JOURNALS_CONTACTS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && ContactId == other.ContactId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_CONTACTS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ ContactId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_CONTACTS left, REL_JOURNALS_CONTACTS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_CONTACTS left, REL_JOURNALS_CONTACTS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_RDATES : IEquatable<REL_JOURNALS_RDATES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence date relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence date entity
        /// </summary>
        [ForeignKey(typeof(RDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RecurrenceDateId { get; set; }

        public bool Equals(REL_JOURNALS_RDATES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && RecurrenceDateId == other.RecurrenceDateId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_RDATES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ RecurrenceDateId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_RDATES left, REL_JOURNALS_RDATES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_RDATES left, REL_JOURNALS_RDATES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_EXDATES : IEquatable<REL_JOURNALS_EXDATES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-exception date relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related exception date entity
        /// </summary>
        [ForeignKey(typeof(EXDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ExceptionDateId { get; set; }

        public bool Equals(REL_JOURNALS_EXDATES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && ExceptionDateId == other.ExceptionDateId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_EXDATES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ ExceptionDateId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_EXDATES left, REL_JOURNALS_EXDATES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_EXDATES left, REL_JOURNALS_EXDATES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_RELATEDTOS : IEquatable<REL_JOURNALS_RELATEDTOS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-related to relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the relation entity
        /// </summary>
        [ForeignKey(typeof(RELATEDTO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public int ExceptionDateId { get; set; }

        public bool Equals(REL_JOURNALS_RELATEDTOS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && ExceptionDateId == other.ExceptionDateId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_RELATEDTOS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ ExceptionDateId;
            }
        }

        public static bool operator ==(REL_JOURNALS_RELATEDTOS left, REL_JOURNALS_RELATEDTOS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_RELATEDTOS left, REL_JOURNALS_RELATEDTOS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_REQSTATS : IEquatable<REL_JOURNALS_REQSTATS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-request status relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related request status entity
        /// </summary>
        [ForeignKey(typeof(REQUEST_STATUS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ReqStatsId { get; set; }

        public bool Equals(REL_JOURNALS_REQSTATS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && ReqStatsId == other.ReqStatsId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_REQSTATS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ ReqStatsId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_REQSTATS left, REL_JOURNALS_REQSTATS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_REQSTATS left, REL_JOURNALS_REQSTATS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_JOURNALS_RESOURCES : IEquatable<REL_JOURNALS_RESOURCES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-resources relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid JournalId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related resources-entity
        /// </summary>
        [ForeignKey(typeof(RESOURCES), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ResourcesId { get; set; }

        public bool Equals(REL_JOURNALS_RESOURCES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JournalId == other.JournalId && ResourcesId == other.ResourcesId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_JOURNALS_RESOURCES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (JournalId.GetHashCode() * 397) ^ ResourcesId.GetHashCode();
            }
        }

        public static bool operator ==(REL_JOURNALS_RESOURCES left, REL_JOURNALS_RESOURCES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_JOURNALS_RESOURCES left, REL_JOURNALS_RESOURCES right)
        {
            return !Equals(left, right);
        }
    }
}