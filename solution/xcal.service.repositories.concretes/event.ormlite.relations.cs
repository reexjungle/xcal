using System;
using System.Data;
using System.Runtime.Serialization;
using ServiceStack.OrmLite;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.repositories.concretes
{
    [DataContract]
    public class REL_EVENTS_ORGANIZERS :IEquatable<REL_EVENTS_ORGANIZERS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(ORGANIZER), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string OrganizerId { get; set; }

        public bool Equals(REL_EVENTS_ORGANIZERS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.OrganizerId.Equals(other.OrganizerId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_ORGANIZERS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.OrganizerId.GetHashCode();
        }

        public static bool operator == (REL_EVENTS_ORGANIZERS x, REL_EVENTS_ORGANIZERS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_ORGANIZERS x, REL_EVENTS_ORGANIZERS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    [DataContract]
    public class REL_EVENTS_RECURRENCE_IDS: IEquatable<REL_EVENTS_RECURRENCE_IDS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence ID relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence identifier entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(RECURRENCE_ID), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceId_Id { get; set; }

        public bool Equals(REL_EVENTS_RECURRENCE_IDS other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceId_Id.Equals(other.RecurrenceId_Id, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_RECURRENCE_IDS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.RecurrenceId_Id.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_RECURRENCE_IDS x, REL_EVENTS_RECURRENCE_IDS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_RECURRENCE_IDS x, REL_EVENTS_RECURRENCE_IDS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    [DataContract]
    public class REL_EVENTS_RRULES: IEquatable<REL_EVENTS_RRULES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence identifier entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(RECUR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceRuleId { get; set; }

        public bool Equals(REL_EVENTS_RRULES other)
        {
            if (other == null) return false;
            return (this.EventId.Equals(other.EventId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceRuleId.Equals(other.RecurrenceRuleId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EVENTS_RRULES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.EventId.GetHashCode() ^ this.RecurrenceRuleId.GetHashCode();
        }

        public static bool operator ==(REL_EVENTS_RRULES x, REL_EVENTS_RRULES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EVENTS_RRULES x, REL_EVENTS_RRULES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    [DataContract]
    public class REL_EVENTS_ATTACHBINS: IEquatable<REL_EVENTS_ATTACHBINS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_ATTACHURIS : IEquatable<REL_EVENTS_ATTACHURIS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_ATTENDEES: IEquatable<REL_EVENTS_ATTENDEES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_COMMENTS: IEquatable<REL_EVENTS_COMMENTS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-comment relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related comment entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_CONTACTS: IEquatable<REL_EVENTS_CONTACTS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-contact relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related contact entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_RDATES: IEquatable<REL_EVENTS_RDATES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence date relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence date entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_EXDATES: IEquatable<REL_EVENTS_EXDATES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-exception date relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related exception date entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_RELATEDTOS: IEquatable<REL_EVENTS_RELATEDTOS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-related to relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the relation entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_REQSTATS: IEquatable<REL_EVENTS_REQSTATS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-request status relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related request status entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_RESOURCES: IEquatable<REL_EVENTS_RESOURCES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-resources relation
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_AUDIO_ALARMS: IEquatable<REL_EVENTS_AUDIO_ALARMS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_DISPLAY_ALARMS : IEquatable<REL_EVENTS_DISPLAY_ALARMS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [DataMember]
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

    [DataContract]
    public class REL_EVENTS_EMAIL_ALARMS : IEquatable<REL_EVENTS_EMAIL_ALARMS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [DataMember]
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

}
