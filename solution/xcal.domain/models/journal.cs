using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;
using reexjungle.xcal.domain.contracts;
using reexjungle.foundation.essentials.contracts;
using reexjungle.foundation.essentials.concretes;



namespace reexjungle.xcal.domain.models
{
    [DataContract]
    public class VJOURNAL: IJOURNAL, IEquatable<VJOURNAL>, IContainsKey<string>
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
        public CLASS Classification { get; set; }

        [DataMember]
        public DATE_TIME Created { get; set; }

        [DataMember]
        public DESCRIPTION Description { get; set; }

        [DataMember]
        public DATE_TIME LastModified { get; set; }

        [DataMember]
        [Ignore]
        public ORGANIZER Organizer { get; set; }

        [DataMember]
        public int Sequence { get; set; }

        [DataMember]
        public STATUS Status { get; set; }

        [DataMember]
        public SUMMARY Summary { get; set; }

        [DataMember]
        public URI Url { get; set; }

        [DataMember]

        public RECURRENCE_ID RecurrenceId { get; set; }

        [DataMember]
        [Ignore]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        [Ignore]
        public List<IATTACH> Attachments { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTENDEE> Attendees { get; set; }

        [DataMember]
        public CATEGORIES Categories { get; set; }

        [DataMember]
        [Ignore]
        public List<COMMENT> Comments { get; set; }

        [DataMember]
        [Ignore]
        public List<CONTACT> Contacts { get; set; }

        [DataMember]
        [Ignore]
        public List<EXDATE> ExceptionDates { get; set; }

        [DataMember]
        [Ignore]
        public List<REQUEST_STATUS> RequestStatuses { get; set; }

        [DataMember]
        [Ignore]
        public List<RESOURCES> Resources { get; set; }

        [DataMember]
        [Ignore]
        public List<RELATEDTO> RelatedTos { get; set; }

        [DataMember]
        [Ignore]
        public List<RDATE> RecurrenceDates { get; set; }

        [DataMember]
        [Ignore]
        public List<IANA_PROPERTY> IANA { get; set; }

        [DataMember]
        [Ignore]
        public List<X_PROPERTY> NonStandard { get; set; }

        public bool Equals(VJOURNAL other)
        {
            bool equals = false;

            //primary reference
            equals = this.Uid.Equals(other.Uid, StringComparison.OrdinalIgnoreCase);

            if (equals && this.RecurrenceId != null && other.RecurrenceId != null) equals = this.RecurrenceId == other.RecurrenceId;

            //secondary reference if both events are equal by Uid/Recurrence Id
            if (equals) equals = this.Sequence == other.Sequence;

            //tie-breaker
            if (equals) equals = this.Datestamp == other.Datestamp;

            return equals;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is VJOURNAL)) return false;
            return this.Equals(obj as VJOURNAL);
        }

        public override int GetHashCode()
        {
            var hash = this.Uid.GetHashCode();
            if (this.RecurrenceId != null) hash = hash ^ this.RecurrenceId.GetHashCode();
            hash = hash ^ this.Sequence.GetHashCode() ^ this.Datestamp.GetHashCode();
            return hash;
        }

        public static bool operator ==(VJOURNAL a, VJOURNAL b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(VJOURNAL a, VJOURNAL b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VJOURNAL").AppendLine();
            sb.AppendFormat("DTSTAMP:{0}", this.Datestamp).AppendLine();
            sb.AppendFormat("UID:{0}", this.Uid).AppendLine();
            if (this.Start.TimeZoneId != null)
                sb.AppendFormat("DTSTART;{0}:{1}", this.Start.TimeZoneId, this.Start).AppendLine();
            else
                sb.AppendFormat("DTSTART:{0}", this.Start).AppendLine();
            if (this.Classification != CLASS.UNKNOWN) sb.AppendFormat("CLASS:{0}", this.Classification).AppendLine();
            sb.AppendFormat("CREATED:{0}", this.Created).AppendLine();
            if (this.Description != null) sb.Append(this.Description).AppendLine();
            sb.AppendFormat("LAST-MODIFIED:{0}", this.LastModified).AppendLine();
            if (this.Organizer != null) sb.Append(this.Organizer).AppendLine();
            sb.AppendFormat("SEQUENCE:{0}", this.Sequence).AppendLine();
            if (this.Status != STATUS.UNKNOWN) sb.AppendFormat("STATUS:{0}", this.Status).AppendLine();
            if (this.Summary != null) sb.Append(this.Summary).AppendLine();
            if (this.Url != null) sb.AppendFormat("URL:{0}", this.Url).AppendLine();
            if (this.RecurrenceId != null) sb.Append(this.RecurrenceId).AppendLine();
            if (this.RecurrenceRule != null) sb.AppendFormat("RRULE:{0}", this.RecurrenceRule).AppendLine();

            if (!this.Attachments.NullOrEmpty())
            {
                foreach (var attachment in this.Attachments) if (attachment != null) sb.Append(attachment).AppendLine();
            }

            if (!this.Attendees.NullOrEmpty())
            {
                foreach (var attendee in this.Attendees) if (attendee != null) sb.Append(attendee).AppendLine();
            }

            if (this.Categories != null && !this.Categories.Values.NullOrEmpty()) sb.Append(this.Categories).AppendLine();

            if (!this.Comments.NullOrEmpty())
            {
                foreach (var comment in Comments) if (comment != null) sb.Append(comment).AppendLine();
            }

            if (!this.Contacts.NullOrEmpty())
            {
                foreach (var contact in this.Contacts) if (contact != null) sb.Append(contact).AppendLine();
            }

            if (!this.ExceptionDates.NullOrEmpty())
            {
                foreach (var exdate in this.ExceptionDates) if (exdate != null) sb.Append(exdate).AppendLine();
            }

            if (!this.RequestStatuses.NullOrEmpty())
            {
                foreach (var reqstat in this.RequestStatuses) if (reqstat != null) sb.Append(reqstat).AppendLine();
            }

            if (!this.RelatedTos.NullOrEmpty())
            {
                foreach (var relatedto in this.RelatedTos) if (relatedto != null) sb.Append(relatedto).AppendLine();
            }

            if (!this.Resources.NullOrEmpty())
            {
                foreach (var resource in this.Resources) if (resource != null) sb.Append(resource).AppendLine();
            }

            if (!this.Resources.NullOrEmpty())
            {
                foreach (var resource in this.Resources) if (resource != null) sb.Append(resource).AppendLine();
            }

            if (!this.RecurrenceDates.NullOrEmpty())
            {
                foreach (var rdate in this.RecurrenceDates) if (rdate != null) sb.Append(rdate).AppendLine();
            }

            if (!this.IANA.NullOrEmpty())
            {
                foreach (var iana in this.IANA) if (iana != null) sb.Append(iana).AppendLine();
            }

            if (!this.NonStandard.NullOrEmpty())
            {
                foreach (var xprop in this.NonStandard) if (xprop != null) sb.Append(xprop).AppendLine();
            }

            sb.Append("END:VJOURNAL");
            return sb.ToString();
        }
    }
}
