using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ServiceStack.DataAnnotations;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.domain.models
{

    /// <summary>
    /// Specifes a contract for the VEVENT component of the iCalendar Core Object
    /// </summary>
    [DataContract]
    public class VEVENT : IEVENT, IEquatable<VEVENT>, IComparable<VEVENT>, IContainsKey<string>
    {
        private string id;
        private DATE_TIME start;
        private DATE_TIME end;
        private DURATION duration;

        /// <summary>
        /// Gets or sets the unique identifier of the event.. 
        /// </summary>
        [DataMember]
        [Index(Unique = true)]
        public string Id 
        {
            get 
            {
                if (string.IsNullOrEmpty(this.id))
                {
                    return this.RecurrenceId != null
                        ? string.Format("{0}-{1}", this.Uid, this.RecurrenceId.Id)
                        : this.Uid;
                }
                else return this.id;
            }
            set { this.id = value; }
        }

        /// <summary>
        /// Gets or sets the unique identifier of a non-recurrent event.
        /// </summary>
        [DataMember]
        [Index(Unique = false)]
        public string Uid { get; set; }

        [DataMember]
        [Index(Unique = false)]
        public DATE_TIME Datestamp { get; set; }

        [DataMember]
        public DATE_TIME Start
        {
            get { return this.start; }
            set
            {
                this.start = value;
                if (this.end == default(DATE_TIME))
                    this.end = this.start;
            }
        }

        [DataMember]
        public CLASS Classification { get; set; }

        [DataMember]
        public DATE_TIME Created { get; set; }

        [DataMember]
        public DESCRIPTION Description { get; set; }

        [DataMember]
        public GEO Position { get; set; }

        [DataMember]
        public DATE_TIME LastModified { get; set; }

        [DataMember]
        public LOCATION Location { get; set; }

        [DataMember]
        [Ignore]
        public ORGANIZER Organizer { get; set; }

        [DataMember]
        public PRIORITY Priority { get; set; }

        [DataMember]
        public int Sequence { get; set; }

        [DataMember]
        public STATUS Status { get; set; }

        [DataMember]
        public SUMMARY Summary { get; set; }

        [DataMember]
        public TRANSP Transparency { get; set; }

        [DataMember]
        public URI Url { get; set; }

        [DataMember]
        [Ignore]
        public RECURRENCE_ID RecurrenceId { get; set; }

        [DataMember]
        [Ignore]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        public DATE_TIME End
        {
            get { return this.end; }
            set 
            { 
                this.end = value;
                if (duration == default(DURATION)) 
                    this.duration = this.end - this.start;
            }
        }

        [DataMember]
        public DURATION Duration 
        {
            get { return this.duration; } 
            set
            {
                this.duration = value;
                if(this.end == default(DATE_TIME)) this.end = start + duration;
            }
        }

        [DataMember]
        [Ignore]
        public List<ATTACH_BINARY> AttachmentBinaries { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTACH_URI> AttachmentUris { get; set; }

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
        public List<AUDIO_ALARM> AudioAlarms { get; set; }

        [DataMember]
        [Ignore]
        public List<DISPLAY_ALARM> DisplayAlarms { get; set; }

        [DataMember]
        [Ignore]
        public List<EMAIL_ALARM> EmailAlarms { get; set; }

        [DataMember]
        [Ignore]
        public Dictionary<string, IANA_PROPERTY> IANAProperties { get; set; }

        [DataMember]
        [Ignore]
        public Dictionary<string, X_PROPERTY> XProperties { get; set; }

        public VEVENT()
        {
            this.RecurrenceId = null;
            this.Datestamp = new DATE_TIME(DateTime.UtcNow);
            this.Created = this.Datestamp;
            this.LastModified = this.Datestamp;
            this.AttachmentBinaries = new List<ATTACH_BINARY>();
            this.AttachmentUris = new List<ATTACH_URI>();
            this.Attendees = new List<ATTENDEE>();
            this.Categories = new CATEGORIES();
            this.Contacts = new List<CONTACT>();
            this.Comments = new List<COMMENT>();
            this.ExceptionDates = new List<EXDATE>();
            this.RequestStatuses = new List<REQUEST_STATUS>();
            this.RelatedTos = new List<RELATEDTO>();
            this.RecurrenceDates = new List<RDATE>();
            this.AudioAlarms = new List<AUDIO_ALARM>();
            this.DisplayAlarms = new List<DISPLAY_ALARM>();
            this.EmailAlarms = new List<EMAIL_ALARM>();
            this.IANAProperties = new Dictionary<string, IANA_PROPERTY>();
            this.XProperties = new Dictionary<string, X_PROPERTY>();
        }

        public VEVENT(DATE_TIME dtstamp, string uid, DATE_TIME start, DATE_TIME end,  PRIORITY priority, ORGANIZER organizer = null, LOCATION location = null, 
            STATUS status = STATUS.NEEDS_ACTION, SUMMARY summary = null, TRANSP transparency = TRANSP.TRANSPARENT,
            RECURRENCE_ID recurid = null, RECUR rrule = null, List<ATTENDEE> attendees = null, CATEGORIES categories = null, List<RELATEDTO> relatedtos = null)
        {
            this.Datestamp = dtstamp;
            this.Uid = uid;
            this.Start = start;
            this.Organizer = organizer;
            this.Location = location;
            this.Priority = priority;
            this.Status = status;
            this.Summary = summary;
            this.Transparency = transparency;
            this.RecurrenceId = recurid;
            this.RecurrenceRule = rrule;
            this.end = end;
            this.Attendees = attendees;
            this.Categories = categories;
            this.RelatedTos = relatedtos;
        }


        public VEVENT(DATE_TIME dtstamp, string uid, DATE_TIME start, DURATION duration, PRIORITY priority, ORGANIZER organizer = null, LOCATION location = null,
             STATUS status = STATUS.NEEDS_ACTION, SUMMARY summary = null, TRANSP transparency = TRANSP.TRANSPARENT, RECURRENCE_ID recurid = null, RECUR rrule = null, List<ATTENDEE> attendees = null, CATEGORIES categories = null, List<RELATEDTO> relatedtos = null)
        {
            this.Datestamp = dtstamp;
            this.Uid = uid;
            this.Start = start;
            this.Organizer = organizer;
            this.Location = location;
            this.Priority = priority;
            this.Status = status;
            this.Summary = summary;
            this.Transparency = transparency;
            this.RecurrenceId = recurid;
            this.RecurrenceRule = rrule;
            this.Duration = duration;
            this.Attendees = attendees;
            this.Categories = categories;
            this.RelatedTos = relatedtos;
        }

        public VEVENT(IEVENT value)
        {
            if (value == null) throw new ArgumentNullException("value");
            this.Datestamp = value.Datestamp;
            this.Uid = value.Uid;
            this.RecurrenceId = value.RecurrenceId;
            this.Start = value.Start;
            this.Organizer = value.Organizer;
            this.Location = value.Location;
            this.Sequence = value.Sequence;
            this.Priority = value.Priority;
            this.Status = value.Status;
            this.Position = value.Position;
            this.Classification = value.Classification;
            this.Transparency = value.Transparency;
            this.Summary = value.Summary;
            this.Description = value.Description;
            this.Transparency = value.Transparency;
            this.RecurrenceId = value.RecurrenceId;
            this.RecurrenceRule = value.RecurrenceRule;
            this.end = value.End;
            this.duration = value.Duration;
            this.AttachmentBinaries = value.AttachmentBinaries;
            this.AttachmentUris = value.AttachmentUris;
            this.Comments = value.Comments;
            this.Contacts = value.Contacts;
            this.ExceptionDates = value.ExceptionDates;
            this.RecurrenceDates = value.RecurrenceDates;
            this.RequestStatuses = value.RequestStatuses;
            this.Attendees = value.Attendees;
            this.Categories = value.Categories;
            this.RelatedTos = value.RelatedTos;
            this.AudioAlarms = value.AudioAlarms;
            this.DisplayAlarms = value.DisplayAlarms;
            this.EmailAlarms = value.EmailAlarms;
            this.IANAProperties = value.IANAProperties;
            this.XProperties = value.XProperties;
        }

        public bool Equals(VEVENT other)
        {
            bool equals = false;

            //primary reference
            equals = this.Uid.Equals(other.Uid, StringComparison.OrdinalIgnoreCase);

            if(equals && this.RecurrenceId != null && other.RecurrenceId != null) equals = this.RecurrenceId == other.RecurrenceId;
            
            //secondary reference if both events are equal by Uid/Recurrence Id
            if (equals)  equals = this.Sequence == other.Sequence;

            //tie-breaker
            if (equals) equals = this.Datestamp == other.Datestamp;

            return equals;
        }

        public int CompareTo(VEVENT other)
        {
            var compare = this.Uid.CompareTo(other.Uid);
            if(compare == 0) compare = this.Sequence.CompareTo(other.Sequence);
            if(compare == 0) compare = this.Datestamp.CompareTo(other.Datestamp);
            return compare;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is VEVENT)) return false;
            return this.Equals(obj as VEVENT);
        }

        public override int GetHashCode()
        {
            var hash = this.Uid.GetHashCode();
            if (this.RecurrenceId != null) hash = hash ^ this.RecurrenceId.GetHashCode();
            hash = hash ^ this.Sequence.GetHashCode() ^ this.Datestamp.GetHashCode();
            return hash;
        }

        public static bool operator ==(VEVENT a, VEVENT b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(VEVENT a, VEVENT b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public static bool operator <(VEVENT a, VEVENT b)
        {
            if ((object)a == null || (object)b == null) return false;
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(VEVENT a, VEVENT b)
        {
            if ((object)a == null || (object)b == null) return false;
            return a.CompareTo(b) > 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VEVENT").AppendLine();
            sb.AppendFormat("DTSTAMP:{0}", this.Datestamp.ToString()).AppendLine();
            sb.AppendFormat("UID:{0}", this.Uid.ToString()).AppendLine();
            if (this.Start.TimeZoneId != null)
                sb.AppendFormat("DTSTART;{0}:{1}", this.Start.TimeZoneId.ToString(), this.Start.ToString()).AppendLine();
            else
                sb.AppendFormat("DTSTART:{0}", this.Start.ToString()).AppendLine();
            if (this.Classification != CLASS.UNKNOWN) sb.AppendFormat("CLASS:{0}", this.Classification.ToString()).AppendLine();
            sb.AppendFormat("CREATED:{0}", this.Created.ToString()).AppendLine();
            if (this.Description != null) sb.Append(this.Description.ToString()).AppendLine();
            if (this.Position != default(GEO)) sb.Append(this.Position.ToString()).AppendLine();
            sb.AppendFormat("LAST-MODIFIED:{0}", this.LastModified.ToString()).AppendLine();
            if (this.Location != null) sb.Append(this.Location.ToString()).AppendLine();
            if (this.Organizer != null) sb.Append(this.Organizer.ToString()).AppendLine();
            if (this.Priority != null) sb.Append(this.Priority.ToString()).AppendLine();
            sb.AppendFormat("SEQUENCE:{0}", this.Sequence.ToString()).AppendLine();
            if (this.Status != STATUS.UNKNOWN) sb.AppendFormat("STATUS:{0}", this.Status.ToString()).AppendLine();
            if (this.Summary != null) sb.Append(this.Summary.ToString()).AppendLine();
            if (this.Transparency != TRANSP.UNKNOWN) sb.AppendFormat("TRANSP:{0}", this.Transparency.ToString()).AppendLine();
            if (this.Url != null) sb.AppendFormat("URL:{0}", this.Url.ToString()).AppendLine();
            if (this.RecurrenceId != null) sb.Append(this.RecurrenceId.ToString()).AppendLine();
            if (this.RecurrenceRule != null) sb.AppendFormat("RRULE:{0}", this.RecurrenceRule.ToString()).AppendLine();
            if (this.End != default(DATE_TIME))
            {
                if (this.End.TimeZoneId != null)
                    sb.AppendFormat("DTEND;{0}:{1}", this.End.TimeZoneId.ToString(), this.End.ToString()).AppendLine();
                else
                    sb.AppendFormat("DTEND:{0}", this.End.ToString()).AppendLine();
            }
            else if (this.Duration != default(DURATION)) sb.Append(this.Duration.ToString()).AppendLine();

            if(!this.AttachmentBinaries.NullOrEmpty())
            {
                foreach (var attachment in this.AttachmentBinaries) if (attachment != null) sb.Append(attachment.ToString()).AppendLine();
            }

            if(!this.Attendees.NullOrEmpty())
            {
                foreach (var attendee in this.Attendees) if (attendee != null) sb.Append(attendee.ToString()).AppendLine();
            }

            if (this.Categories != null && !this.Categories.Values.NullOrEmpty()) sb.Append(this.Categories.ToString()).AppendLine();

            if (!this.Comments.NullOrEmpty())
            {
                foreach (var comment in Comments) if (comment != null) sb.Append(comment.ToString()).AppendLine();
            }

            if (!this.Contacts.NullOrEmpty())
            {
                foreach (var contact in this.Contacts) if (contact != null) sb.Append(contact.ToString()).AppendLine();
            }

            if (!this.ExceptionDates.NullOrEmpty())
            {
                foreach (var exdate in this.ExceptionDates) if (exdate != null) sb.Append(exdate.ToString()).AppendLine();
            }

            if (!this.RequestStatuses.NullOrEmpty())
            {
                foreach (var reqstat in this.RequestStatuses) if (reqstat != null) sb.Append(reqstat.ToString()).AppendLine();
            }

            if (!this.RelatedTos.NullOrEmpty())
            {
                foreach (var relatedto in this.RelatedTos) if (relatedto != null) sb.Append(relatedto.ToString()).AppendLine();
            }

            if (!this.Resources.NullOrEmpty())
            {
                foreach (var resource in this.Resources) if (resource != null) sb.Append(resource.ToString()).AppendLine();
            }

            if (!this.RecurrenceDates.NullOrEmpty())
            {
                foreach (var rdate in this.RecurrenceDates) if (rdate != null) sb.Append(rdate.ToString()).AppendLine();
            }

            if (!this.AudioAlarms.NullOrEmpty())
            {
                foreach (var alarm in this.AudioAlarms) if (alarm != null) sb.Append(alarm.ToString()).AppendLine();
            }
            if (!this.DisplayAlarms.NullOrEmpty())
            {
                foreach (var alarm in this.DisplayAlarms) if (alarm != null) sb.Append(alarm.ToString()).AppendLine();
            }
            if (!this.EmailAlarms.NullOrEmpty())
            {
                foreach (var alarm in this.EmailAlarms) if (alarm != null) sb.Append(alarm.ToString()).AppendLine();
            }
            if (!this.IANAProperties.NullOrEmpty())
            {
                foreach (var iana in this.IANAProperties.Values) if (iana != null) sb.Append(iana.ToString()).AppendLine();
            }

            if (!this.XProperties.NullOrEmpty())
            {
                foreach (var xprop in this.XProperties.Values) if (xprop != null) sb.Append(xprop.ToString()).AppendLine();
            }

            sb.Append("END:VEVENT");
            return sb.ToString();
        }

    }

}
