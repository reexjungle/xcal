using reexjungle.xcal.domain.models;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.xcal.service.repositories.concretes.relations
{
    public class REL_FREEBUSIES_ATTACHBINS : IEquatable<REL_FREEBUSIES_ATTACHBINS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_FREEBUSIES_ATTACHBINS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_FREEBUSIES_ATTACHBINS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_FREEBUSIES_ATTACHBINS x, REL_FREEBUSIES_ATTACHBINS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_FREEBUSIES_ATTACHBINS x, REL_FREEBUSIES_ATTACHBINS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_FREEBUSIES_ATTACHURIS : IEquatable<REL_FREEBUSIES_ATTACHURIS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_FREEBUSIES_ATTACHURIS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_FREEBUSIES_ATTACHURIS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_FREEBUSIES_ATTACHURIS x, REL_FREEBUSIES_ATTACHURIS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_FREEBUSIES_ATTACHURIS x, REL_FREEBUSIES_ATTACHURIS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_FREEBUSIES_ATTENDEES : IEquatable<REL_FREEBUSIES_ATTENDEES>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttendeeId { get; set; }

        public bool Equals(REL_FREEBUSIES_ATTENDEES other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.AttendeeId.Equals(other.AttendeeId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_FREEBUSIES_ATTENDEES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.AttendeeId.GetHashCode();
        }

        public static bool operator ==(REL_FREEBUSIES_ATTENDEES x, REL_FREEBUSIES_ATTENDEES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_FREEBUSIES_ATTENDEES x, REL_FREEBUSIES_ATTENDEES y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_FREEBUSIES_COMMENTS : IEquatable<REL_FREEBUSIES_COMMENTS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-comment relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related comment entity
        /// </summary>
        [ForeignKey(typeof(COMMENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string CommentId { get; set; }

        public bool Equals(REL_FREEBUSIES_COMMENTS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.CommentId.Equals(other.CommentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_FREEBUSIES_COMMENTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.CommentId.GetHashCode();
        }

        public static bool operator ==(REL_FREEBUSIES_COMMENTS x, REL_FREEBUSIES_COMMENTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_FREEBUSIES_COMMENTS x, REL_FREEBUSIES_COMMENTS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_FREEBUSIES_INFOS : IEquatable<REL_FREEBUSIES_INFOS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-contact relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string FreeBusyId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related contact entity
        /// </summary>
        [ForeignKey(typeof(FREEBUSY_INFO), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string InfoId { get; set; }

        public bool Equals(REL_FREEBUSIES_INFOS other)
        {
            if (other == null) return false;
            return (this.FreeBusyId.Equals(other.FreeBusyId, StringComparison.OrdinalIgnoreCase) &&
                this.InfoId.Equals(other.InfoId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_FREEBUSIES_INFOS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.FreeBusyId.GetHashCode() ^ this.InfoId.GetHashCode();
        }

        public static bool operator ==(REL_FREEBUSIES_INFOS x, REL_FREEBUSIES_INFOS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_FREEBUSIES_INFOS x, REL_FREEBUSIES_INFOS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_FREEBUSIES_REQSTATS : IEquatable<REL_FREEBUSIES_REQSTATS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-request status relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(VFREEBUSY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string TodoId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related request status entity
        /// </summary>
        [ForeignKey(typeof(REQUEST_STATUS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ReqStatsId { get; set; }

        public bool Equals(REL_FREEBUSIES_REQSTATS other)
        {
            if (other == null) return false;
            return (this.TodoId.Equals(other.TodoId, StringComparison.OrdinalIgnoreCase) &&
                this.ReqStatsId.Equals(other.ReqStatsId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_FREEBUSIES_REQSTATS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.TodoId.GetHashCode() ^ this.ReqStatsId.GetHashCode();
        }

        public static bool operator ==(REL_FREEBUSIES_REQSTATS x, REL_FREEBUSIES_REQSTATS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_FREEBUSIES_REQSTATS x, REL_FREEBUSIES_REQSTATS y)
        {
            if ((object)x == null || y == (object)null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }
}