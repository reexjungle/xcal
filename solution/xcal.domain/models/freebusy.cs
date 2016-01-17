using reexjungle.xcal.domain.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace reexjungle.xcal.domain.models
{
    [DataContract]
    public class VFREEBUSY : IFREEBUSY, IEquatable<VFREEBUSY>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the free/busy time.
        /// </summary>
        [DataMember]
        [Index(Unique = true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of a non-recurrent free/busy time.
        /// </summary>
        [DataMember]
        [Index(Unique = false)]
        public string Uid
        {
            get { return Id.ToString(); }
            set
            {
                var id = Guid.Empty;
                if (Guid.TryParse(value, out id))
                {
                    Id = id;
                }
            }
        }

        [DataMember]
        public DATE_TIME Datestamp { get; set; }

        [DataMember]
        public DATE_TIME Start { get; set; }

        [DataMember]
        [Ignore]
        public ORGANIZER Organizer { get; set; }

        [DataMember]
        public URL Url { get; set; }

        [DataMember]
        public DATE_TIME End { get; set; }

        [DataMember]
        [Ignore]
        public List<IATTACH> Attachments { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTENDEE> Attendees { get; set; }

        [DataMember]
        [Ignore]
        public List<COMMENT> Comments { get; set; }

        [DataMember]
        [Ignore]
        public List<FREEBUSY> FreeBusyProperties { get; set; }

        [DataMember]
        [Ignore]
        public List<REQUEST_STATUS> RequestStatuses { get; set; }

        [DataMember]
        [Ignore]
        public List<IANA_PROPERTY> IANA { get; set; }

        [DataMember]
        [Ignore]
        public List<X_PROPERTY> NonStandard { get; set; }

        public bool Equals(VFREEBUSY other)
        {
            //primary reference
            var equals = Id.Equals(other.Id);

            //tie-breaker
            if (equals)
                equals = Datestamp == other.Datestamp;

            return equals;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VFREEBUSY)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Datestamp.GetHashCode();
        }

        public static bool operator ==(VFREEBUSY left, VFREEBUSY right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VFREEBUSY left, VFREEBUSY right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("BEGIN:VFREEBUSY").AppendLine();

            sb.AppendFormat("DTSTAMP:{0}", Datestamp.ToString()).AppendLine();

            sb.AppendFormat("UID:{0}", Uid.ToString()).AppendLine();

            if (Start.TimeZoneId != null)
                sb.AppendFormat("DTSTART;{0}:{1}", Start.TimeZoneId.ToString(), Start.ToString()).AppendLine();
            else
                sb.AppendFormat("DTSTART:{0}", Start.ToString()).AppendLine();

            if (Organizer != null) sb.Append(Organizer.ToString()).AppendLine();

            if (Url != null) sb.AppendFormat("URL:{0}", Url.ToString()).AppendLine();

            if (End.TimeZoneId != null)
                sb.AppendFormat("DTEND;{0}:{1}", End.TimeZoneId.ToString(), End.ToString()).AppendLine();
            else
                sb.AppendFormat("DTEND:{0}", End.ToString()).AppendLine();

            if (!Attachments.NullOrEmpty())
            {
                foreach (var attachment in Attachments.Where(attachment => attachment != null))
                    sb.Append(attachment.ToString()).AppendLine();
            }

            if (!Attendees.NullOrEmpty())
            {
                foreach (var attendee in Attendees.Where(attendee => attendee != null))
                    sb.Append(attendee.ToString()).AppendLine();
            }

            if (!RequestStatuses.NullOrEmpty())
            {
                foreach (var reqstat in RequestStatuses.Where(reqstat => reqstat != null))
                    sb.Append(reqstat.ToString()).AppendLine();
            }

            if (!IANA.NullOrEmpty())
            {
                foreach (var iana in IANA.Where(iana => iana != null))
                    sb.Append(iana.ToString()).AppendLine();
            }

            if (!NonStandard.NullOrEmpty())
            {
                foreach (var xprop in NonStandard.Where(xprop => xprop != null))
                    sb.Append(xprop.ToString()).AppendLine();
            }

            sb.Append("END:VFREEBUSY");
            return sb.ToString();
        }
    }
}