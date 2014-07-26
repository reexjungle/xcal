using System;
using System.Data;
using System.Runtime.Serialization;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;
using reexmonkey.xcal.domain.models;
using reexmonkey.foundation.essentials.contracts;

namespace reexmonkey.xcal.service.repositories.concretes.relations
{

    public class REL_AALARMS_ATTACHBINS : IEquatable<REL_AALARMS_ATTACHBINS>, IContainsKey<string>
    {

        [Index(true)]
        public string Id { get; set; }

        [ForeignKey(typeof(AUDIO_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_AALARMS_ATTACHBINS other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_AALARMS_ATTACHBINS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_AALARMS_ATTACHBINS x, REL_AALARMS_ATTACHBINS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_AALARMS_ATTACHBINS x, REL_AALARMS_ATTACHBINS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_AALARMS_ATTACHURIS : IEquatable<REL_AALARMS_ATTACHURIS>, IContainsKey<string>
    {

        [Index(true)]
        public string Id { get; set; }

        [ForeignKey(typeof(AUDIO_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }


        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_AALARMS_ATTACHURIS other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_AALARMS_ATTACHURIS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_AALARMS_ATTACHURIS x, REL_AALARMS_ATTACHURIS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_AALARMS_ATTACHURIS x, REL_AALARMS_ATTACHURIS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EALARMS_ATTENDEES: IEquatable<REL_EALARMS_ATTENDEES>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(EMAIL_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ATTENDEE), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttendeeId { get; set; }

        public bool Equals(REL_EALARMS_ATTENDEES other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttendeeId.Equals(other.AttendeeId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EALARMS_ATTENDEES;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttendeeId.GetHashCode();
        }

        public static bool operator ==(REL_EALARMS_ATTENDEES x, REL_EALARMS_ATTENDEES y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EALARMS_ATTENDEES x, REL_EALARMS_ATTENDEES y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EALARMS_ATTACHBINS : IEquatable<REL_EALARMS_ATTACHBINS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(EMAIL_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_BINARY), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_EALARMS_ATTACHBINS other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EALARMS_ATTACHBINS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_EALARMS_ATTACHBINS x, REL_EALARMS_ATTACHBINS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EALARMS_ATTACHBINS x, REL_EALARMS_ATTACHBINS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }

    public class REL_EALARMS_ATTACHURIS : IEquatable<REL_EALARMS_ATTACHURIS>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [Index(true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [ForeignKey(typeof(EMAIL_ALARM), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [ForeignKey(typeof(ATTACH_URI), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string AttachmentId { get; set; }

        public bool Equals(REL_EALARMS_ATTACHURIS other)
        {
            if (other == null) return false;
            return (this.AlarmId.Equals(other.AlarmId, StringComparison.OrdinalIgnoreCase) &&
                this.AttachmentId.Equals(other.AttachmentId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_EALARMS_ATTACHURIS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.AlarmId.GetHashCode() ^ this.AttachmentId.GetHashCode();
        }

        public static bool operator ==(REL_EALARMS_ATTACHURIS x, REL_EALARMS_ATTACHURIS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_EALARMS_ATTACHURIS x, REL_EALARMS_ATTACHURIS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }



}
