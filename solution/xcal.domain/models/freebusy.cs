using reexjungle.xcal.domain.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace reexjungle.xcal.domain.models
{
    [DataContract]
    public class VFREEBUSY : IFREEBUSY, IEquatable<VFREEBUSY>, IContainsKey<string>
    {
        [DataMember]
        public string Id
        {
            get { return this.Uid; }
            set { this.Uid = value; }
        }

        [DataMember]
        public string Uid { get; set; }

        [DataMember]
        public DATE_TIME Datestamp { get; set; }

        [DataMember]
        public DATE_TIME Start { get; set; }

        [DataMember]
        [Ignore]
        public ORGANIZER Organizer { get; set; }

        [DataMember]
        public URI Url { get; set; }

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
        public List<FREEBUSY_INFO> FreeBusyProperties { get; set; }

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
            return this.Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is VFREEBUSY)) return false;
            return this.Equals(obj as VFREEBUSY);
        }

        public override int GetHashCode()
        {
            return this.Uid.GetHashCode();
        }

        public static bool operator ==(VFREEBUSY a, VFREEBUSY b)
        {
            if ((object)a == null || (object)b == null) return Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(VFREEBUSY a, VFREEBUSY b)
        {
            if ((object)a == null || (object)b == null) return !Equals(a, b);
            return !a.Equals(b);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VFREEBUSY").AppendLine();
            sb.AppendFormat("DTSTAMP:{0}", this.Datestamp.ToString()).AppendLine();
            sb.AppendFormat("UID:{0}", this.Uid.ToString()).AppendLine();
            if (this.Start.TimeZoneId != null)
                sb.AppendFormat("DTSTART;{0}:{1}", this.Start.TimeZoneId.ToString(), this.Start.ToString()).AppendLine();
            else
                sb.AppendFormat("DTSTART:{0}", this.Start.ToString()).AppendLine();
            if (this.Organizer != null) sb.Append(this.Organizer.ToString()).AppendLine();
            if (this.Url != null) sb.AppendFormat("URL:{0}", this.Url.ToString()).AppendLine();

            if (this.End.TimeZoneId != null)
                sb.AppendFormat("DTEND;{0}:{1}", this.End.TimeZoneId.ToString(), this.End.ToString()).AppendLine();
            else
                sb.AppendFormat("DTEND:{0}", this.End.ToString()).AppendLine();

            if (!this.Attachments.NullOrEmpty())
            {
                foreach (var attachment in this.Attachments) if (attachment != null) sb.Append(attachment.ToString()).AppendLine();
            }

            if (!this.Attendees.NullOrEmpty())
            {
                foreach (var attendee in this.Attendees) if (attendee != null) sb.Append(attendee.ToString()).AppendLine();
            }

            if (!this.RequestStatuses.NullOrEmpty())
            {
                foreach (var reqstat in this.RequestStatuses) if (reqstat != null) sb.Append(reqstat.ToString()).AppendLine();
            }

            if (!this.IANA.NullOrEmpty())
            {
                foreach (var iana in this.IANA) if (iana != null) sb.Append(iana.ToString()).AppendLine();
            }

            if (!this.NonStandard.NullOrEmpty())
            {
                foreach (var xprop in this.NonStandard) if (xprop != null) sb.Append(xprop.ToString()).AppendLine();
            }

            sb.Append("END:VFREEBUSY");
            return sb.ToString();
        }
    }
}