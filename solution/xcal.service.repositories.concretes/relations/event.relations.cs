using reexjungle.foundation.essentials.contracts;
using reexjungle.xcal.domain.models;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.Data;
using System.Runtime.Serialization;

namespace reexjungle.xcal.service.repositories.concretes.relations
{
    public class REL_EVENTS_ATTACHBINS : IEquatable<REL_EVENTS_ATTACHBINS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_EVENTS_ATTACHBINS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_ATTACHBINS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_ATTACHBINS x, REL_EVENTS_ATTACHBINS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_ATTACHBINS x, REL_EVENTS_ATTACHBINS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_ATTACHURIS : IEquatable<REL_EVENTS_ATTACHURIS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_EVENTS_ATTACHURIS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_ATTACHURIS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_ATTACHURIS x, REL_EVENTS_ATTACHURIS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_ATTACHURIS x, REL_EVENTS_ATTACHURIS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_ATTENDEES : IEquatable<REL_EVENTS_ATTENDEES>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttendeeId { get; set; }

        public bool Equals(REL_EVENTS_ATTENDEES other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.AttendeeId.Equals(other.AttendeeId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_ATTENDEES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.AttendeeId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_ATTENDEES x, REL_EVENTS_ATTENDEES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_ATTENDEES x, REL_EVENTS_ATTENDEES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_COMMENTS : IEquatable<REL_EVENTS_COMMENTS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-comment relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related comment entity
        /// </summary>
        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CommentId { get; set; }

        public bool Equals(REL_EVENTS_COMMENTS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.CommentId.Equals(other.CommentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_COMMENTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.CommentId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_COMMENTS x, REL_EVENTS_COMMENTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_COMMENTS x, REL_EVENTS_COMMENTS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_CONTACTS : IEquatable<REL_EVENTS_CONTACTS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-contact relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related contact entity
        /// </summary>
        [ForeignKey(typeof(CONTACT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ContactId { get; set; }

        public bool Equals(REL_EVENTS_CONTACTS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.ContactId.Equals(other.ContactId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_CONTACTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.ContactId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_CONTACTS x, REL_EVENTS_CONTACTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_CONTACTS x, REL_EVENTS_CONTACTS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_RDATES : IEquatable<REL_EVENTS_RDATES>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence date relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence date entity
        /// </summary>
        [ForeignKey(typeof(RDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceDateId { get; set; }

        public bool Equals(REL_EVENTS_RDATES other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceDateId.Equals(other.RecurrenceDateId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_RDATES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.RecurrenceDateId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_RDATES x, REL_EVENTS_RDATES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_RDATES x, REL_EVENTS_RDATES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_EXDATES : IEquatable<REL_EVENTS_EXDATES>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-exception date relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related exception date entity
        /// </summary>
        [ForeignKey(typeof(EXDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ExceptionDateId { get; set; }

        public bool Equals(REL_EVENTS_EXDATES other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.ExceptionDateId.Equals(other.ExceptionDateId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_EXDATES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.ExceptionDateId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_EXDATES x, REL_EVENTS_EXDATES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_EXDATES x, REL_EVENTS_EXDATES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_RELATEDTOS : IEquatable<REL_EVENTS_RELATEDTOS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-related to relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the relation entity
        /// </summary>
        [ForeignKey(typeof(RELATEDTO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RelatedToId { get; set; }

        public bool Equals(REL_EVENTS_RELATEDTOS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.RelatedToId.Equals(other.RelatedToId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_RELATEDTOS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.RelatedToId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_RELATEDTOS x, REL_EVENTS_RELATEDTOS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_RELATEDTOS x, REL_EVENTS_RELATEDTOS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_REQSTATS : IEquatable<REL_EVENTS_REQSTATS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-request status relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related request status entity
        /// </summary>
        [ForeignKey(typeof(REQUEST_STATUS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ReqStatsId { get; set; }

        public bool Equals(REL_EVENTS_REQSTATS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.ReqStatsId.Equals(other.ReqStatsId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_REQSTATS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.ReqStatsId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_REQSTATS x, REL_EVENTS_REQSTATS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_REQSTATS x, REL_EVENTS_REQSTATS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_RESOURCES : IEquatable<REL_EVENTS_RESOURCES>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-resources relation
        /// </summary>
        [DataMember]
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related resources-entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(RESOURCES), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ResourcesId { get; set; }

        public bool Equals(REL_EVENTS_RESOURCES other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.ResourcesId.Equals(other.ResourcesId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_RESOURCES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.ResourcesId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_RESOURCES x, REL_EVENTS_RESOURCES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_RESOURCES x, REL_EVENTS_RESOURCES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_AUDIO_ALARMS : IEquatable<REL_EVENTS_AUDIO_ALARMS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(AUDIO_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        public bool Equals(REL_EVENTS_AUDIO_ALARMS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_AUDIO_ALARMS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.AlarmId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_AUDIO_ALARMS x, REL_EVENTS_AUDIO_ALARMS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_AUDIO_ALARMS x, REL_EVENTS_AUDIO_ALARMS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_DISPLAY_ALARMS : IEquatable<REL_EVENTS_DISPLAY_ALARMS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(DISPLAY_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        public bool Equals(REL_EVENTS_DISPLAY_ALARMS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_DISPLAY_ALARMS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.AlarmId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_DISPLAY_ALARMS x, REL_EVENTS_DISPLAY_ALARMS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_DISPLAY_ALARMS x, REL_EVENTS_DISPLAY_ALARMS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_EMAIL_ALARMS : IEquatable<REL_EVENTS_EMAIL_ALARMS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(EMAIL_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        public bool Equals(REL_EVENTS_EMAIL_ALARMS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_EMAIL_ALARMS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.AlarmId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_EMAIL_ALARMS x, REL_EVENTS_EMAIL_ALARMS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_EMAIL_ALARMS x, REL_EVENTS_EMAIL_ALARMS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_IANA_PROPERTIES : IEquatable<REL_EVENTS_IANA_PROPERTIES>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-iana property relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(IANA_PROPERTY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string IanaPropertyId { get; set; }

        public bool Equals(REL_EVENTS_IANA_PROPERTIES other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.IanaPropertyId.Equals(other.IanaPropertyId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_IANA_PROPERTIES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.IanaPropertyId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_IANA_PROPERTIES x, REL_EVENTS_IANA_PROPERTIES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_IANA_PROPERTIES x, REL_EVENTS_IANA_PROPERTIES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EVENTS_X_PROPERTIES : IEquatable<REL_EVENTS_X_PROPERTIES>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-x property relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(X_PROPERTY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string XPropertyId { get; set; }

        public bool Equals(REL_EVENTS_X_PROPERTIES other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.XPropertyId.Equals(other.XPropertyId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_IANA_PROPERTIES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.XPropertyId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_X_PROPERTIES x, REL_EVENTS_X_PROPERTIES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_X_PROPERTIES x, REL_EVENTS_X_PROPERTIES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }
}