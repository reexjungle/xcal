using System;
using System.Data;
using System.Runtime.Serialization;
using ServiceStack.OrmLite;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.repositories.concretes
{
    [DataContract]
    public class RELS_EMAIL_ALARMS_ATTENDEES: IEquatable<RELS_EMAIL_ALARMS_ATTENDEES>
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
        [ForeignKey(typeof(RELS_EMAIL_ALARMS_ATTENDEES), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttendeeId { get; set; }

        public bool Equals(RELS_EMAIL_ALARMS_ATTENDEES other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttendeeId.Equals(other.AttendeeId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as RELS_EMAIL_ALARMS_ATTENDEES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttendeeId.GetHashCode();
        }

        public static bool operator ==(RELS_EMAIL_ALARMS_ATTENDEES x, RELS_EMAIL_ALARMS_ATTENDEES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(RELS_EMAIL_ALARMS_ATTENDEES x, RELS_EMAIL_ALARMS_ATTENDEES y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    [DataContract]
    public class RELS_EMAIL_ALARMS_ATTACHBINS : IEquatable<RELS_EMAIL_ALARMS_ATTACHBINS>
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
        [ForeignKey(typeof(RELS_EMAIL_ALARMS_ATTACHBINS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(RELS_EMAIL_ALARMS_ATTACHBINS other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as RELS_EMAIL_ALARMS_ATTACHBINS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(RELS_EMAIL_ALARMS_ATTACHBINS x, RELS_EMAIL_ALARMS_ATTACHBINS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(RELS_EMAIL_ALARMS_ATTACHBINS x, RELS_EMAIL_ALARMS_ATTACHBINS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    [DataContract]
    public class RELS_EMAIL_ALARMS_ATTACHURIS : IEquatable<RELS_EMAIL_ALARMS_ATTACHURIS>
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
        [ForeignKey(typeof(RELS_EMAIL_ALARMS_ATTACHURIS), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(RELS_EMAIL_ALARMS_ATTACHURIS other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as RELS_EMAIL_ALARMS_ATTACHURIS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(RELS_EMAIL_ALARMS_ATTACHURIS x, RELS_EMAIL_ALARMS_ATTACHURIS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(RELS_EMAIL_ALARMS_ATTACHURIS x, RELS_EMAIL_ALARMS_ATTACHURIS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }



}
