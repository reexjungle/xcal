using System;
using System.Data;
using System.Runtime.Serialization;
using ServiceStack.OrmLite;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.repositories.concretes
{
    [DataContract]
    public class RELS_EALARMS_ATTENDEES: IEquatable<RELS_EALARMS_ATTENDEES>
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
        [ForeignKey(typeof(RELS_EALARMS_ATTENDEES), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttendeeId { get; set; }

        public bool Equals(RELS_EALARMS_ATTENDEES other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttendeeId.Equals(other.AttendeeId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as RELS_EALARMS_ATTENDEES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttendeeId.GetHashCode();
        }

        public static bool operator ==(RELS_EALARMS_ATTENDEES x, RELS_EALARMS_ATTENDEES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(RELS_EALARMS_ATTENDEES x, RELS_EALARMS_ATTENDEES y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    [DataContract]
    public class RELS_EALARMS_ATTACHBINS : IEquatable<RELS_EALARMS_ATTACHBINS>
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
        [ForeignKey(typeof(RELS_EALARMS_ATTACHBINS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(RELS_EALARMS_ATTACHBINS other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as RELS_EALARMS_ATTACHBINS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(RELS_EALARMS_ATTACHBINS x, RELS_EALARMS_ATTACHBINS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(RELS_EALARMS_ATTACHBINS x, RELS_EALARMS_ATTACHBINS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    [DataContract]
    public class RELS_EALARMS_ATTACHURIS : IEquatable<RELS_EALARMS_ATTACHURIS>
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
        [ForeignKey(typeof(RELS_EALARMS_ATTACHURIS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(RELS_EALARMS_ATTACHURIS other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as RELS_EALARMS_ATTACHURIS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(RELS_EALARMS_ATTACHURIS x, RELS_EALARMS_ATTACHURIS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(RELS_EALARMS_ATTACHURIS x, RELS_EALARMS_ATTACHURIS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }



}
