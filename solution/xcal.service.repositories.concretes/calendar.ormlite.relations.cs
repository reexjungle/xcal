using System;
using System.Data;
using System.Runtime.Serialization;
using ServiceStack.OrmLite;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.repositories.concretes
{
    [DataContract]
    public class REL_CALENDARS_EVENTS : IEquatable<REL_CALENDARS_EVENTS>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calendar-event relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related calendar entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VCALENDAR), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string ProdId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        [ForeignKey(typeof(VEVENT), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public string Uid { get; set; }

        public bool Equals(REL_CALENDARS_EVENTS other)
        {
            if (other == null) return false;
            return (this.ProdId.Equals(other.ProdId, StringComparison.OrdinalIgnoreCase) &&
                this.ProdId.Equals(other.ProdId, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rel = obj as REL_CALENDARS_EVENTS;
            if (rel == null) return false;
            return this.Equals(rel);
        }

        public override int GetHashCode()
        {
            return this.ProdId.GetHashCode() ^ this.Uid.GetHashCode();
        }

        public static bool operator ==(REL_CALENDARS_EVENTS x, REL_CALENDARS_EVENTS y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(REL_CALENDARS_EVENTS x, REL_CALENDARS_EVENTS y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }
    }
}
