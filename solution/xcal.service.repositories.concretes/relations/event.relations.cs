using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.Runtime.Serialization;

namespace reexjungle.xcal.service.repositories.concretes.relations
{
    public class REL_EVENTS_ATTACHBINS : IEquatable<REL_EVENTS_ATTACHBINS>, IContainsKey<Guid>
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
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_EVENTS_ATTACHBINS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_ATTACHBINS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_ATTACHBINS left, REL_EVENTS_ATTACHBINS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_ATTACHBINS left, REL_EVENTS_ATTACHBINS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_ATTACHURIS : IEquatable<REL_EVENTS_ATTACHURIS>, IContainsKey<Guid>
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
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_EVENTS_ATTACHURIS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_ATTACHURIS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_ATTACHURIS left, REL_EVENTS_ATTACHURIS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_ATTACHURIS left, REL_EVENTS_ATTACHURIS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_RECURS : IEquatable<REL_EVENTS_RECURS>, IContainsKey<Guid>
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
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(RECUR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RecurId { get; set; }

        public bool Equals(REL_EVENTS_RECURS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && RecurId == other.RecurId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_RECURS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ RecurId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_RECURS left, REL_EVENTS_RECURS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_RECURS left, REL_EVENTS_RECURS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_ORGANIZERS : IEquatable<REL_EVENTS_ORGANIZERS>, IContainsKey<Guid>
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
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ORGANIZER), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid OrganizerId { get; set; }

        public bool Equals(REL_EVENTS_ORGANIZERS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId.Equals(other.EventId) && OrganizerId.Equals(other.OrganizerId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_ORGANIZERS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ OrganizerId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_ORGANIZERS left, REL_EVENTS_ORGANIZERS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_ORGANIZERS left, REL_EVENTS_ORGANIZERS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_ATTENDEES : IEquatable<REL_EVENTS_ATTENDEES>, IContainsKey<Guid>
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
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttendeeId { get; set; }

        public bool Equals(REL_EVENTS_ATTENDEES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && AttendeeId == other.AttendeeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_ATTENDEES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ AttendeeId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_ATTENDEES left, REL_EVENTS_ATTENDEES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_ATTENDEES left, REL_EVENTS_ATTENDEES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_COMMENTS : IEquatable<REL_EVENTS_COMMENTS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-comment relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related comment entity
        /// </summary>
        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid CommentId { get; set; }

        public bool Equals(REL_EVENTS_COMMENTS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && CommentId == other.CommentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_COMMENTS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ CommentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_COMMENTS left, REL_EVENTS_COMMENTS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_COMMENTS left, REL_EVENTS_COMMENTS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_CONTACTS : IEquatable<REL_EVENTS_CONTACTS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-contact relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related contact entity
        /// </summary>
        [ForeignKey(typeof(CONTACT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ContactId { get; set; }

        public bool Equals(REL_EVENTS_CONTACTS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && ContactId == other.ContactId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_CONTACTS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ ContactId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_CONTACTS left, REL_EVENTS_CONTACTS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_CONTACTS left, REL_EVENTS_CONTACTS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_RDATES : IEquatable<REL_EVENTS_RDATES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence date relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence date entity
        /// </summary>
        [ForeignKey(typeof(RDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RecurrenceDateId { get; set; }

        public bool Equals(REL_EVENTS_RDATES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && RecurrenceDateId == other.RecurrenceDateId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_RDATES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ RecurrenceDateId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_RDATES left, REL_EVENTS_RDATES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_RDATES left, REL_EVENTS_RDATES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_EXDATES : IEquatable<REL_EVENTS_EXDATES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-exception date relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related exception date entity
        /// </summary>
        [ForeignKey(typeof(EXDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ExceptionDateId { get; set; }

        public bool Equals(REL_EVENTS_EXDATES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ExceptionDateId == other.ExceptionDateId && EventId == other.EventId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_EXDATES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ExceptionDateId.GetHashCode() * 397) ^ EventId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_EXDATES left, REL_EVENTS_EXDATES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_EXDATES left, REL_EVENTS_EXDATES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_RELATEDTOS : IEquatable<REL_EVENTS_RELATEDTOS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-related to relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the relation entity
        /// </summary>
        [ForeignKey(typeof(RELATEDTO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid RelatedToId { get; set; }

        public bool Equals(REL_EVENTS_RELATEDTOS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && RelatedToId == other.RelatedToId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_RELATEDTOS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ RelatedToId.GetHashCode() * 397;
            }
        }

        public static bool operator ==(REL_EVENTS_RELATEDTOS left, REL_EVENTS_RELATEDTOS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_RELATEDTOS left, REL_EVENTS_RELATEDTOS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_REQSTATS : IEquatable<REL_EVENTS_REQSTATS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-request status relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related request status entity
        /// </summary>
        [ForeignKey(typeof(REQUEST_STATUS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ReqStatsId { get; set; }

        public bool Equals(REL_EVENTS_REQSTATS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && ReqStatsId == other.ReqStatsId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_REQSTATS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ ReqStatsId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_REQSTATS left, REL_EVENTS_REQSTATS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_REQSTATS left, REL_EVENTS_REQSTATS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_RESOURCES : IEquatable<REL_EVENTS_RESOURCES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-resources relation
        /// </summary>
        [DataMember]
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related resources-entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(RESOURCES), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid ResourcesId { get; set; }

        public bool Equals(REL_EVENTS_RESOURCES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && ResourcesId == other.ResourcesId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_RESOURCES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ ResourcesId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_RESOURCES left, REL_EVENTS_RESOURCES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_RESOURCES left, REL_EVENTS_RESOURCES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_AUDIO_ALARMS : IEquatable<REL_EVENTS_AUDIO_ALARMS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(AUDIO_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        public bool Equals(REL_EVENTS_AUDIO_ALARMS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AlarmId == other.AlarmId && EventId == other.EventId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_AUDIO_ALARMS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AlarmId.GetHashCode() * 397) ^ EventId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_AUDIO_ALARMS left, REL_EVENTS_AUDIO_ALARMS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_AUDIO_ALARMS left, REL_EVENTS_AUDIO_ALARMS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_DISPLAY_ALARMS : IEquatable<REL_EVENTS_DISPLAY_ALARMS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(DISPLAY_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        public bool Equals(REL_EVENTS_DISPLAY_ALARMS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && AlarmId == other.AlarmId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_DISPLAY_ALARMS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ AlarmId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_DISPLAY_ALARMS left, REL_EVENTS_DISPLAY_ALARMS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_DISPLAY_ALARMS left, REL_EVENTS_DISPLAY_ALARMS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EVENTS_EMAIL_ALARMS : IEquatable<REL_EVENTS_EMAIL_ALARMS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(EMAIL_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        public bool Equals(REL_EVENTS_EMAIL_ALARMS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventId == other.EventId && AlarmId == other.AlarmId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EVENTS_EMAIL_ALARMS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventId.GetHashCode() * 397) ^ AlarmId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EVENTS_EMAIL_ALARMS left, REL_EVENTS_EMAIL_ALARMS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EVENTS_EMAIL_ALARMS left, REL_EVENTS_EMAIL_ALARMS right)
        {
            return !Equals(left, right);
        }
    }
}