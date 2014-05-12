using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.repositories.concretes.relations
{
    public class REL_JOURNALS_ORGANIZERS : IEquatable<REL_JOURNALS_ORGANIZERS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ORGANIZER), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string OrganizerId { get; set; }

        public bool Equals(REL_JOURNALS_ORGANIZERS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.OrganizerId.Equals(other.OrganizerId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_ORGANIZERS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.OrganizerId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_ORGANIZERS x, REL_JOURNALS_ORGANIZERS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_ORGANIZERS x, REL_JOURNALS_ORGANIZERS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_RECURRENCE_IDS : IEquatable<REL_JOURNALS_RECURRENCE_IDS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence ID relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence identifier entity
        /// </summary>
        [ForeignKey(typeof(RECURRENCE_ID), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceId_Id { get; set; }

        public bool Equals(REL_JOURNALS_RECURRENCE_IDS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceId_Id.Equals(other.RecurrenceId_Id, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_RECURRENCE_IDS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.RecurrenceId_Id.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_RECURRENCE_IDS x, REL_JOURNALS_RECURRENCE_IDS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_RECURRENCE_IDS x, REL_JOURNALS_RECURRENCE_IDS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_RECURS : IEquatable<REL_JOURNALS_RECURS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence identifier entity
        /// </summary>
        [ForeignKey(typeof(RECUR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceRuleId { get; set; }

        public bool Equals(REL_JOURNALS_RECURS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceRuleId.Equals(other.RecurrenceRuleId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_RECURS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.RecurrenceRuleId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_RECURS x, REL_JOURNALS_RECURS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_RECURS x, REL_JOURNALS_RECURS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_ATTACHBINS : IEquatable<REL_JOURNALS_ATTACHBINS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_JOURNALS_ATTACHBINS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_ATTACHBINS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_ATTACHBINS x, REL_JOURNALS_ATTACHBINS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_ATTACHBINS x, REL_JOURNALS_ATTACHBINS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_ATTACHURIS : IEquatable<REL_JOURNALS_ATTACHURIS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_JOURNALS_ATTACHURIS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_ATTACHURIS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_ATTACHURIS x, REL_JOURNALS_ATTACHURIS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_ATTACHURIS x, REL_JOURNALS_ATTACHURIS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_ATTENDEES : IEquatable<REL_JOURNALS_ATTENDEES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttendeeId { get; set; }

        public bool Equals(REL_JOURNALS_ATTENDEES other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AttendeeId.Equals(other.AttendeeId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_ATTENDEES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AttendeeId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_ATTENDEES x, REL_JOURNALS_ATTENDEES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_ATTENDEES x, REL_JOURNALS_ATTENDEES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_COMMENTS : IEquatable<REL_JOURNALS_COMMENTS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-comment relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related comment entity
        /// </summary>
        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CommentId { get; set; }

        public bool Equals(REL_JOURNALS_COMMENTS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.CommentId.Equals(other.CommentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_COMMENTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.CommentId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_COMMENTS x, REL_JOURNALS_COMMENTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_COMMENTS x, REL_JOURNALS_COMMENTS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_CONTACTS : IEquatable<REL_JOURNALS_CONTACTS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-contact relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related contact entity
        /// </summary>
        [ForeignKey(typeof(CONTACT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ContactId { get; set; }

        public bool Equals(REL_JOURNALS_CONTACTS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.ContactId.Equals(other.ContactId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_CONTACTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.ContactId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_CONTACTS x, REL_JOURNALS_CONTACTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_CONTACTS x, REL_JOURNALS_CONTACTS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_RDATES : IEquatable<REL_JOURNALS_RDATES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence date relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence date entity
        /// </summary>
        [ForeignKey(typeof(RDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RecurrenceDateId { get; set; }

        public bool Equals(REL_JOURNALS_RDATES other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.RecurrenceDateId.Equals(other.RecurrenceDateId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_RDATES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.RecurrenceDateId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_RDATES x, REL_JOURNALS_RDATES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_RDATES x, REL_JOURNALS_RDATES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_EXDATES : IEquatable<REL_JOURNALS_EXDATES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-exception date relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related exception date entity
        /// </summary>
        [ForeignKey(typeof(EXDATE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ExceptionDateId { get; set; }

        public bool Equals(REL_JOURNALS_EXDATES other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.ExceptionDateId.Equals(other.ExceptionDateId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_EXDATES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.ExceptionDateId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_EXDATES x, REL_JOURNALS_EXDATES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_EXDATES x, REL_JOURNALS_EXDATES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_RELATEDTOS : IEquatable<REL_JOURNALS_RELATEDTOS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-related to relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the relation entity
        /// </summary>
        [ForeignKey(typeof(RELATEDTO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string RelatedToId { get; set; }

        public bool Equals(REL_JOURNALS_RELATEDTOS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.RelatedToId.Equals(other.RelatedToId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_RELATEDTOS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.RelatedToId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_RELATEDTOS x, REL_JOURNALS_RELATEDTOS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_RELATEDTOS x, REL_JOURNALS_RELATEDTOS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_REQSTATS : IEquatable<REL_JOURNALS_REQSTATS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-request status relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related request status entity
        /// </summary>
        [ForeignKey(typeof(REQUEST_STATUS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ReqStatsId { get; set; }

        public bool Equals(REL_JOURNALS_REQSTATS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.ReqStatsId.Equals(other.ReqStatsId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_REQSTATS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.ReqStatsId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_REQSTATS x, REL_JOURNALS_REQSTATS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_REQSTATS x, REL_JOURNALS_REQSTATS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_JOURNALS_RESOURCES : IEquatable<REL_JOURNALS_RESOURCES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-resources relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VJOURNAL), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related resources-entity
        /// </summary>
        [ForeignKey(typeof(RESOURCES), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ResourcesId { get; set; }

        public bool Equals(REL_JOURNALS_RESOURCES other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.ResourcesId.Equals(other.ResourcesId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_JOURNALS_RESOURCES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.ResourcesId.GetHashCode();
        }

        public static bool operator ==(REL_JOURNALS_RESOURCES x, REL_JOURNALS_RESOURCES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_JOURNALS_RESOURCES x, REL_JOURNALS_RESOURCES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

}
