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
    public class VJOURNAL : IJOURNAL, IEquatable<VJOURNAL>, IContainsKey<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the journal.
        /// </summary>
        [DataMember]
        [Index(Unique = true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of a non-recurrent journal.
        /// </summary>
        [DataMember]
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
            var equals = Id.Equals(other.Id);

            if (equals && RecurrenceId != null && other.RecurrenceId != null)
                equals = RecurrenceId == other.RecurrenceId;

            if (equals) equals = Sequence == other.Sequence;

            if (equals)
                equals = Datestamp == other.Datestamp;

            return equals;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VJOURNAL)obj);
        }

        public override int GetHashCode()
        {
            var hash = Id.GetHashCode();
            if (RecurrenceId != null) hash = hash ^ RecurrenceId.GetHashCode();
            hash = hash ^ Sequence.GetHashCode() ^ Datestamp.GetHashCode();
            return hash;
        }

        public static bool operator ==(VJOURNAL left, VJOURNAL right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VJOURNAL left, VJOURNAL right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VJOURNAL").AppendLine();
            sb.AppendFormat("DTSTAMP:{0}", Datestamp).AppendLine();
            sb.AppendFormat("UID:{0}", Uid).AppendLine();
            if (Start.TimeZoneId != null)
                sb.AppendFormat("DTSTART;{0}:{1}", Start.TimeZoneId, Start).AppendLine();
            else
                sb.AppendFormat("DTSTART:{0}", Start).AppendLine();
            if (Classification != CLASS.UNKNOWN) sb.AppendFormat("CLASS:{0}", Classification).AppendLine();
            sb.AppendFormat("CREATED:{0}", Created).AppendLine();
            if (Description != null) sb.Append(Description).AppendLine();
            sb.AppendFormat("LAST-MODIFIED:{0}", LastModified).AppendLine();
            if (Organizer != null) sb.Append(Organizer).AppendLine();
            sb.AppendFormat("SEQUENCE:{0}", Sequence).AppendLine();
            if (Status != STATUS.UNKNOWN) sb.AppendFormat("STATUS:{0}", Status).AppendLine();
            if (Summary != null) sb.Append(Summary).AppendLine();
            if (Url != null) sb.AppendFormat("URL:{0}", Url).AppendLine();
            if (RecurrenceId != null) sb.Append(RecurrenceId).AppendLine();
            if (RecurrenceRule != null) sb.AppendFormat("RRULE:{0}", RecurrenceRule).AppendLine();

            if (!Attachments.NullOrEmpty())
            {
                foreach (var attachment in Attachments) if (attachment != null) sb.Append(attachment).AppendLine();
            }

            if (!Attendees.NullOrEmpty())
            {
                foreach (var attendee in Attendees) if (attendee != null) sb.Append(attendee).AppendLine();
            }

            if (Categories != null && !Categories.Values.NullOrEmpty()) sb.Append(Categories).AppendLine();

            if (!Comments.NullOrEmpty())
            {
                foreach (var comment in Comments) if (comment != null) sb.Append(comment).AppendLine();
            }

            if (!Contacts.NullOrEmpty())
            {
                foreach (var contact in Contacts) if (contact != null) sb.Append(contact).AppendLine();
            }

            if (!ExceptionDates.NullOrEmpty())
            {
                foreach (var exdate in ExceptionDates) if (exdate != null) sb.Append(exdate).AppendLine();
            }

            if (!RequestStatuses.NullOrEmpty())
            {
                foreach (var reqstat in RequestStatuses) if (reqstat != null) sb.Append(reqstat).AppendLine();
            }

            if (!RelatedTos.NullOrEmpty())
            {
                foreach (var relatedto in RelatedTos) if (relatedto != null) sb.Append(relatedto).AppendLine();
            }

            if (!Resources.NullOrEmpty())
            {
                foreach (var resource in Resources) if (resource != null) sb.Append(resource).AppendLine();
            }

            if (!Resources.NullOrEmpty())
            {
                foreach (var resource in Resources) if (resource != null) sb.Append(resource).AppendLine();
            }

            if (!RecurrenceDates.NullOrEmpty())
            {
                foreach (var rdate in RecurrenceDates) if (rdate != null) sb.Append(rdate).AppendLine();
            }

            if (!IANA.NullOrEmpty())
            {
                foreach (var iana in IANA) if (iana != null) sb.Append(iana).AppendLine();
            }

            if (!NonStandard.NullOrEmpty())
            {
                foreach (var xprop in NonStandard) if (xprop != null) sb.Append(xprop).AppendLine();
            }

            sb.Append("END:VJOURNAL");
            return sb.ToString();
        }
    }
}