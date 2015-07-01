using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;

namespace reexjungle.xcal.service.repositories.concretes.relations
{
    public class REL_AALARMS_ATTACHBINS : IEquatable<REL_AALARMS_ATTACHBINS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(AUDIO_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_AALARMS_ATTACHBINS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AlarmId == other.AlarmId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_AALARMS_ATTACHBINS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AlarmId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_AALARMS_ATTACHBINS left, REL_AALARMS_ATTACHBINS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_AALARMS_ATTACHBINS left, REL_AALARMS_ATTACHBINS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_AALARMS_ATTACHURIS : IEquatable<REL_AALARMS_ATTACHURIS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(AUDIO_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_AALARMS_ATTACHURIS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AlarmId == other.AlarmId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_AALARMS_ATTACHURIS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AlarmId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_AALARMS_ATTACHURIS left, REL_AALARMS_ATTACHURIS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_AALARMS_ATTACHURIS left, REL_AALARMS_ATTACHURIS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EALARMS_ATTENDEES : IEquatable<REL_EALARMS_ATTENDEES>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(EMAIL_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttendeeId { get; set; }

        public bool Equals(REL_EALARMS_ATTENDEES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AlarmId == other.AlarmId && AttendeeId == other.AttendeeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EALARMS_ATTENDEES)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AlarmId.GetHashCode() * 397) ^ AttendeeId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EALARMS_ATTENDEES left, REL_EALARMS_ATTENDEES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EALARMS_ATTENDEES left, REL_EALARMS_ATTENDEES right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EALARMS_ATTACHBINS : IEquatable<REL_EALARMS_ATTACHBINS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(EMAIL_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_EALARMS_ATTACHBINS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AlarmId == other.AlarmId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EALARMS_ATTACHBINS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AlarmId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EALARMS_ATTACHBINS left, REL_EALARMS_ATTACHBINS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EALARMS_ATTACHBINS left, REL_EALARMS_ATTACHBINS right)
        {
            return !Equals(left, right);
        }
    }

    public class REL_EALARMS_ATTACHURIS : IEquatable<REL_EALARMS_ATTACHURIS>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [Index(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(EMAIL_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public Guid AttachmentId { get; set; }

        public bool Equals(REL_EALARMS_ATTACHURIS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AlarmId == other.AlarmId && AttachmentId == other.AttachmentId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REL_EALARMS_ATTACHURIS)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AlarmId.GetHashCode() * 397) ^ AttachmentId.GetHashCode();
            }
        }

        public static bool operator ==(REL_EALARMS_ATTACHURIS left, REL_EALARMS_ATTACHURIS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REL_EALARMS_ATTACHURIS left, REL_EALARMS_ATTACHURIS right)
        {
            return !Equals(left, right);
        }
    }
}