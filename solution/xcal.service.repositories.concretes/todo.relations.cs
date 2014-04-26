using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.repositories.concretes
{
    public class REL_TODOS_ORGANIZERS : IEquatable<REL_TODOS_ORGANIZERS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ORGANIZER), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string OrganizerId { get; set; }

        public bool Equals(REL_TODOS_ORGANIZERS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.OrganizerId.Equals(other.OrganizerId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_ORGANIZERS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.OrganizerId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_ORGANIZERS x, REL_TODOS_ORGANIZERS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_ORGANIZERS x, REL_TODOS_ORGANIZERS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_RECURRENCE_IDS : IEquatable<REL_TODOS_RECURRENCE_IDS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence ID relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence identifier entity
        /// </summary>
        [ForeignKey(typeof(RECURRENCE_ID), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceId_Id { get; set; }

        public bool Equals(REL_TODOS_RECURRENCE_IDS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceId_Id.Equals(other.RecurrenceId_Id, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_RECURRENCE_IDS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.RecurrenceId_Id.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_RECURRENCE_IDS x, REL_TODOS_RECURRENCE_IDS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_RECURRENCE_IDS x, REL_TODOS_RECURRENCE_IDS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_RECURS : IEquatable<REL_TODOS_RECURS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence identifier entity
        /// </summary>
        [ForeignKey(typeof(RECUR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceRuleId { get; set; }

        public bool Equals(REL_TODOS_RECURS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceRuleId.Equals(other.RecurrenceRuleId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_RECURS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.RecurrenceRuleId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_RECURS x, REL_TODOS_RECURS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_RECURS x, REL_TODOS_RECURS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_ATTACHBINS : IEquatable<REL_TODOS_ATTACHBINS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_TODOS_ATTACHBINS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_ATTACHBINS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_ATTACHBINS x, REL_TODOS_ATTACHBINS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_ATTACHBINS x, REL_TODOS_ATTACHBINS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_ATTACHURIS : IEquatable<REL_TODOS_ATTACHURIS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_TODOS_ATTACHURIS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_ATTACHURIS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_ATTACHURIS x, REL_TODOS_ATTACHURIS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_ATTACHURIS x, REL_TODOS_ATTACHURIS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_ATTENDEES : IEquatable<REL_TODOS_ATTENDEES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttendeeId { get; set; }

        public bool Equals(REL_TODOS_ATTENDEES other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AttendeeId.Equals(other.AttendeeId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_ATTENDEES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AttendeeId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_ATTENDEES x, REL_TODOS_ATTENDEES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_ATTENDEES x, REL_TODOS_ATTENDEES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_COMMENTS : IEquatable<REL_TODOS_COMMENTS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-comment relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related comment entity
        /// </summary>
        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CommentId { get; set; }

        public bool Equals(REL_TODOS_COMMENTS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.CommentId.Equals(other.CommentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_COMMENTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.CommentId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_COMMENTS x, REL_TODOS_COMMENTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_COMMENTS x, REL_TODOS_COMMENTS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_CONTACTS : IEquatable<REL_TODOS_CONTACTS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-contact relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related contact entity
        /// </summary>
        [ForeignKey(typeof(CONTACT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ContactId { get; set; }

        public bool Equals(REL_TODOS_CONTACTS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.ContactId.Equals(other.ContactId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_CONTACTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.ContactId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_CONTACTS x, REL_TODOS_CONTACTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_CONTACTS x, REL_TODOS_CONTACTS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_RDATES : IEquatable<REL_TODOS_RDATES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence date relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence date entity
        /// </summary>
        [ForeignKey(typeof(RDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceDateId { get; set; }

        public bool Equals(REL_TODOS_RDATES other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceDateId.Equals(other.RecurrenceDateId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_RDATES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.RecurrenceDateId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_RDATES x, REL_TODOS_RDATES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_RDATES x, REL_TODOS_RDATES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_EXDATES : IEquatable<REL_TODOS_EXDATES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-exception date relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related exception date entity
        /// </summary>
        [ForeignKey(typeof(EXDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ExceptionDateId { get; set; }

        public bool Equals(REL_TODOS_EXDATES other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.ExceptionDateId.Equals(other.ExceptionDateId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_EXDATES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.ExceptionDateId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_EXDATES x, REL_TODOS_EXDATES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_EXDATES x, REL_TODOS_EXDATES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_RELATEDTOS : IEquatable<REL_TODOS_RELATEDTOS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-related to relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the relation entity
        /// </summary>
        [ForeignKey(typeof(RELATEDTO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RelatedToId { get; set; }

        public bool Equals(REL_TODOS_RELATEDTOS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.RelatedToId.Equals(other.RelatedToId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_RELATEDTOS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.RelatedToId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_RELATEDTOS x, REL_TODOS_RELATEDTOS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_RELATEDTOS x, REL_TODOS_RELATEDTOS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_REQSTATS : IEquatable<REL_TODOS_REQSTATS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-request status relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related request status entity
        /// </summary>
        [ForeignKey(typeof(REQUEST_STATUS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ReqStatsId { get; set; }

        public bool Equals(REL_TODOS_REQSTATS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.ReqStatsId.Equals(other.ReqStatsId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_REQSTATS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.ReqStatsId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_REQSTATS x, REL_TODOS_REQSTATS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_REQSTATS x, REL_TODOS_REQSTATS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_RESOURCES : IEquatable<REL_TODOS_RESOURCES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-resources relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related resources-entity
        /// </summary>
        [ForeignKey(typeof(RESOURCES), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ResourcesId { get; set; }

        public bool Equals(REL_TODOS_RESOURCES other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.ResourcesId.Equals(other.ResourcesId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_RESOURCES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.ResourcesId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_RESOURCES x, REL_TODOS_RESOURCES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_RESOURCES x, REL_TODOS_RESOURCES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_AUDIO_ALARMS : IEquatable<REL_TODOS_AUDIO_ALARMS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(AUDIO_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        public bool Equals(REL_TODOS_AUDIO_ALARMS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_AUDIO_ALARMS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AlarmId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_AUDIO_ALARMS x, REL_TODOS_AUDIO_ALARMS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_AUDIO_ALARMS x, REL_TODOS_AUDIO_ALARMS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_DISPLAY_ALARMS : IEquatable<REL_TODOS_DISPLAY_ALARMS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(DISPLAY_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        public bool Equals(REL_TODOS_DISPLAY_ALARMS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_DISPLAY_ALARMS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AlarmId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_DISPLAY_ALARMS x, REL_TODOS_DISPLAY_ALARMS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_DISPLAY_ALARMS x, REL_TODOS_DISPLAY_ALARMS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_TODOS_EMAIL_ALARMS : IEquatable<REL_TODOS_EMAIL_ALARMS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VTODO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [ForeignKey(typeof(EMAIL_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        public bool Equals(REL_TODOS_EMAIL_ALARMS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_TODOS_EMAIL_ALARMS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AlarmId.GetHashCode();
        }

        public static bool operator ==(REL_TODOS_EMAIL_ALARMS x, REL_TODOS_EMAIL_ALARMS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_TODOS_EMAIL_ALARMS x, REL_TODOS_EMAIL_ALARMS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

}
