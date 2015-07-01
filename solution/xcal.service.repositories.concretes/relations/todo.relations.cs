using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;

namespace reexjungle.xcal.service.repositories.concretes.relations
{
    public class REL_TODOS_ATTACHBINS : IEquatable<REL_TODOS_ATTACHBINS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_TODOS_ATTACHBINS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TodoId == other.TodoId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_ATTACHBINS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TodoId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_ATTACHBINS left, REL_TODOS_ATTACHBINS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_ATTACHBINS left, REL_TODOS_ATTACHBINS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_ATTACHURIS : IEquatable<REL_TODOS_ATTACHURIS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_TODOS_ATTACHURIS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(TodoId, other.TodoId) && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_ATTACHURIS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TodoId != null ? TodoId.GetHashCode() : 0) * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_ATTACHURIS left, REL_TODOS_ATTACHURIS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_ATTACHURIS left, REL_TODOS_ATTACHURIS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_RECURS : IEquatable<REL_TODOS_RECURS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(RECUR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RecurId { get; set; }

        public bool Equals(REL_TODOS_RECURS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(TodoId, other.TodoId) && RecurId == other.RecurId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_RECURS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TodoId != null ? TodoId.GetHashCode() : 0) * 397) ^ RecurId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_RECURS left, REL_TODOS_RECURS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_RECURS left, REL_TODOS_RECURS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_ORGANIZERS : IEquatable<REL_TODOS_ORGANIZERS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ORGANIZER), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid OrganizerId { get; set; }

        public bool Equals(REL_TODOS_ORGANIZERS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(TodoId, other.TodoId) && OrganizerId == other.OrganizerId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_ORGANIZERS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TodoId != null ? TodoId.GetHashCode() : 0) * 397) ^ OrganizerId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_ORGANIZERS left, REL_TODOS_ORGANIZERS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_ORGANIZERS left, REL_TODOS_ORGANIZERS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_ATTENDEES : IEquatable<REL_TODOS_ATTENDEES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttendeeId { get; set; }

        public bool Equals(REL_TODOS_ATTENDEES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(TodoId, other.TodoId) && AttendeeId == other.AttendeeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_ATTENDEES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TodoId != null ? TodoId.GetHashCode() : 0) * 397) ^ AttendeeId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_ATTENDEES left, REL_TODOS_ATTENDEES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_ATTENDEES left, REL_TODOS_ATTENDEES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_COMMENTS : IEquatable<REL_TODOS_COMMENTS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-comment relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related comment entity
        /// </summary>
        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CommentId { get; set; }

        public bool Equals(REL_TODOS_COMMENTS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TodoId.Equals(other.TodoId) && CommentId.Equals(other.CommentId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_COMMENTS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TodoId.GetHashCode() * 397) ^ CommentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_COMMENTS left, REL_TODOS_COMMENTS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_COMMENTS left, REL_TODOS_COMMENTS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_CONTACTS : IEquatable<REL_TODOS_CONTACTS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-contact relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related contact entity
        /// </summary>
        [ForeignKey(typeof(CONTACT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ContactId { get; set; }

        public bool Equals(REL_TODOS_CONTACTS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(TodoId, other.TodoId) && ContactId == other.ContactId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_CONTACTS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TodoId != null ? TodoId.GetHashCode() : 0) * 397) ^ ContactId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_CONTACTS left, REL_TODOS_CONTACTS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_CONTACTS left, REL_TODOS_CONTACTS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_RDATES : IEquatable<REL_TODOS_RDATES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence date relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence date entity
        /// </summary>
        [ForeignKey(typeof(RDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RecurrenceDateId { get; set; }

        public bool Equals(REL_TODOS_RDATES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TodoId.Equals(other.TodoId) && RecurrenceDateId.Equals(other.RecurrenceDateId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_RDATES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TodoId.GetHashCode() * 397) ^ RecurrenceDateId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_RDATES left, REL_TODOS_RDATES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_RDATES left, REL_TODOS_RDATES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_EXDATES : IEquatable<REL_TODOS_EXDATES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-exception date relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related exception date entity
        /// </summary>
        [ForeignKey(typeof(EXDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ExceptionDateId { get; set; }

        public bool Equals(REL_TODOS_EXDATES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(TodoId, other.TodoId) && ExceptionDateId == other.ExceptionDateId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_EXDATES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TodoId != null ? TodoId.GetHashCode() : 0) * 397) ^ ExceptionDateId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_EXDATES left, REL_TODOS_EXDATES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_EXDATES left, REL_TODOS_EXDATES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_RELATEDTOS : IEquatable<REL_TODOS_RELATEDTOS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-related to relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the relation entity
        /// </summary>
        [ForeignKey(typeof(RELATEDTO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ExceptionDateId { get; set; }

        public bool Equals(REL_TODOS_RELATEDTOS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TodoId.Equals(other.TodoId) && ExceptionDateId.Equals(other.ExceptionDateId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_RELATEDTOS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TodoId.GetHashCode() * 397) ^ ExceptionDateId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_RELATEDTOS left, REL_TODOS_RELATEDTOS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_RELATEDTOS left, REL_TODOS_RELATEDTOS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_REQSTATS : IEquatable<REL_TODOS_REQSTATS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-request status relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related request status entity
        /// </summary>
        [ForeignKey(typeof(REQUEST_STATUS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ReqStatsId { get; set; }

        public bool Equals(REL_TODOS_REQSTATS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TodoId.Equals(other.TodoId) && ReqStatsId.Equals(other.ReqStatsId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_REQSTATS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TodoId.GetHashCode() * 397) ^ ReqStatsId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_REQSTATS left, REL_TODOS_REQSTATS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_REQSTATS left, REL_TODOS_REQSTATS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_RESOURCES : IEquatable<REL_TODOS_RESOURCES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-resources relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related resources-entity
        /// </summary>
        [ForeignKey(typeof(RESOURCES), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ResourcesId { get; set; }

        public bool Equals(REL_TODOS_RESOURCES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TodoId.Equals(other.TodoId) && ResourcesId.Equals(other.ResourcesId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_RESOURCES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TodoId.GetHashCode() * 397) ^ ResourcesId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_RESOURCES left, REL_TODOS_RESOURCES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_RESOURCES left, REL_TODOS_RESOURCES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_AUDIO_ALARMS : IEquatable<REL_TODOS_AUDIO_ALARMS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(AUDIO_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        public bool Equals(REL_TODOS_AUDIO_ALARMS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TodoId.Equals(other.TodoId) && AlarmId.Equals(other.AlarmId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_AUDIO_ALARMS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TodoId.GetHashCode() * 397) ^ AlarmId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_AUDIO_ALARMS left, REL_TODOS_AUDIO_ALARMS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_AUDIO_ALARMS left, REL_TODOS_AUDIO_ALARMS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_DISPLAY_ALARMS : IEquatable<REL_TODOS_DISPLAY_ALARMS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(DISPLAY_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        public bool Equals(REL_TODOS_DISPLAY_ALARMS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TodoId.Equals(other.TodoId) && AlarmId.Equals(other.AlarmId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_DISPLAY_ALARMS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TodoId.GetHashCode() * 397) ^ AlarmId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_DISPLAY_ALARMS left, REL_TODOS_DISPLAY_ALARMS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_DISPLAY_ALARMS left, REL_TODOS_DISPLAY_ALARMS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_TODOS_EMAIL_ALARMS : IEquatable<REL_TODOS_EMAIL_ALARMS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(EMAIL_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        public bool Equals(REL_TODOS_EMAIL_ALARMS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TodoId.Equals(other.TodoId) && AlarmId.Equals(other.AlarmId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_TODOS_EMAIL_ALARMS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TodoId.GetHashCode() * 397) ^ AlarmId.GetHashCode();
            }
        }

        public static bool operator ==(REL_TODOS_EMAIL_ALARMS left, REL_TODOS_EMAIL_ALARMS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_TODOS_EMAIL_ALARMS left, REL_TODOS_EMAIL_ALARMS right)
        {
            return !Equals(left, right);
        }
    }
}