using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using ServiceStack.DataAnnotations;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.extensions;

namespace reexmonkey.xcal.domain.models
{
    #region Event core object

    /// <summary>
    /// Specifes a contract for the VEVENT component of the iCalendar Core Object
    /// </summary>
    [DataContract]
    [KnownType(typeof(ATTACH_BINARY))]
    [KnownType(typeof(ATTACH_URI))]
    [KnownType(typeof(AUDIO_ALARM))]
    [KnownType(typeof(DISPLAY_ALARM))]
    [KnownType(typeof(EMAIL_ALARM))]
    public class VEVENT : ICOMPONENT, IEquatable<VEVENT>, IComparable<VEVENT>, IContainsKey<string>
    {
        private string uid;
        private DATE_TIME start;
        private DATE_TIME end;
        private DURATION duration;

        /// <summary>
        /// Gets or sets the unique identifier of the event. It is synonymous to the &quot;Uid&quot; property of the event. 
        /// </summary>
        [DataMember]
        [Index(Unique = true)] 
        public string Id
        {
            get { return (this.RecurrenceId != null) ? string.Format(@"{0}\/{1}", this.Uid, this.RecurrenceId.Value) : this.Uid; }
            set
            {
                var pattern = @"^(?<uid>(\p{L})+)+(?<hyphen>\/)?(?<recurid>(\p{L}+\p{P}*\s*)+)$";
                if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                    {
                        if (match.Groups["uid"].Success) this.Uid = match.Groups["uid"].Value;
                        if (match.Groups["recurid"].Success)
                        {
                            var recurid = new DATE_TIME(match.Groups["recurid"].Value);
                            if (this.RecurrenceId == null)
                                this.RecurrenceId = (recurid != null) 
                                    ? new RECURRENCE_ID(recurid) 
                                    : ((this.Start != null) ? new RECURRENCE_ID(this.Start) : null);
                            else this.RecurrenceId.Value = (recurid != null && string.IsNullOrEmpty(recurid.ToString())) 
                                ? recurid 
                                : this.Start;
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the event.
        /// </summary>
        [DataMember]
        public string Uid 
        {
            get { return this.uid; }
            set { this.uid = value; }
        }

        [DataMember]
        public DATE_TIME Datestamp { get; set; }

        [DataMember]
        public DATE_TIME Start
        {
            get { return this.start; }
            set
            {
                this.start = value;
                if (this.end == null) this.end = start;
            }
        }

        [DataMember]
        public CLASS Classification { get; set; }

        [DataMember]
        public DATE_TIME Created { get; set; }

        [DataMember]
        public TEXT Description { get; set; }

        [DataMember]
        public GEO Geo { get; set; }

        [DataMember]
        public DATE_TIME LastModified { get; set; }

        [DataMember]
        public TEXT Location { get; set; }

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
        public TEXT Summary { get; set; }

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
                if (this.start != null && this.end != null)
                {
                    this.duration = new DATE_TIME(this.end) - new DATE_TIME(this.start);
                }
            }
        }

        [DataMember]
        public DURATION Duration 
        {
            get { return this.duration; } 
            set
            {
                this.duration = value;
                if(this.start != null && this.duration != null)
                {
                    this.end = start + duration;
                }
            }
        }

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
        public List<TEXT> Comments { get; set; }

        [DataMember]
        [Ignore]
        public List<TEXT> Contacts { get; set; }

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
        public List<IALARM> Alarms { get; set; }

        [DataMember]
        [Ignore]
        public List<IIANA_PROPERTY> IANAProperties { get; set; }

        [DataMember]
        [Ignore]
        public List<IXPROPERTY> XProperties { get; set; }

        public VEVENT()
        {
            this.Datestamp = new DATE_TIME(DateTime.UtcNow);
            this.Created = new DATE_TIME(DateTime.UtcNow);
            this.LastModified = new DATE_TIME(DateTime.UtcNow);
        }

        public VEVENT(DATE_TIME dtstamp, string uid, DATE_TIME start, DATE_TIME end, ORGANIZER organizer = null, TEXT location = null, 
            PRIORITY priority = null, STATUS status = STATUS.NEEDS_ACTION, TEXT summary = null, TRANSP transparency = TRANSP.TRANSPARENT,
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


        public VEVENT(DATE_TIME dtstamp, string uid, DATE_TIME start, DURATION duration, ORGANIZER organizer = null, TEXT location = null,
            PRIORITY priority = null, STATUS status = STATUS.NEEDS_ACTION, TEXT summary = null, TRANSP transparency = TRANSP.TRANSPARENT, RECURRENCE_ID recurid = null, RECUR rrule = null, List<ATTENDEE> attendees = null, CATEGORIES categories = null, List<RELATEDTO> relatedtos = null)
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


        public bool Equals(VEVENT other)
        {
            bool equals = false;

            //primary reference
            equals = this.Uid.Equals(other.Uid, StringComparison.OrdinalIgnoreCase);
            if (this.RecurrenceId != null && other.RecurrenceId != null) 
                equals = equals && this.RecurrenceId == other.RecurrenceId;

            //secondary reference if both events are equal by Uid/Recurrence Id
            if(equals) equals = this.Sequence == other.Sequence;

            //tie-breaker
            if(equals) equals = this.Datestamp == other.Datestamp;
            return equals;
        }

        public int CompareTo(VEVENT other)
        {
            var compare = 0;
            compare = this.Sequence.CompareTo(other.Sequence);
            if(compare == 0) //if sequences are equal
            {
                compare = this.Datestamp.CompareTo(other.Datestamp);
            }
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
            return base.GetHashCode();
        }

        public static bool operator ==(VEVENT a, VEVENT b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(VEVENT a, VEVENT b)
        {
            return !a.Equals(b);
        }

        public static bool operator <(VEVENT a, VEVENT b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(VEVENT a, VEVENT b)
        {
            return a.CompareTo(b) > 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VEVENT").AppendLine();
            sb.AppendFormat("DTSTAMP:{0}", this.Datestamp).AppendLine();
            sb.AppendFormat("string:{0}", this.Uid).AppendLine();

            if (this.Start != null)
            {
                if (this.Start.TimeZoneId != null )sb.AppendFormat("DTSTART;{0}:{1}", this.Start.TimeZoneId, this.Start).AppendLine();
                else sb.AppendFormat("DTSTART:{0}", this.Start).AppendLine();
            }

            if (this.Classification != CLASS.UNKNOWN) sb.AppendFormat("CLASS:{0}", this.Classification).AppendLine();
            if(this.Created != null) sb.AppendFormat("CREATED:{0}", this.Created).AppendLine();
            if (this.Description != null) sb.AppendFormat("DESCRIPTION:{0}", this.Description).AppendLine();
            if (this.Geo != null) sb.Append(this.Geo).AppendLine();
            if (this.LastModified != null) sb.AppendFormat("LAST-MODIFIED:{0}", this.LastModified).AppendLine();
            if (this.Location != null) sb.AppendFormat("LOCATION:{0}",this.Location).AppendLine();
            if (this.Organizer != null) sb.Append(this.Organizer).AppendLine();
            if (this.Priority != null) sb.Append(this.Priority).AppendLine();
            sb.AppendFormat("SEQUENCE:{0}", this.Sequence).AppendLine();
            if (this.Status != STATUS.UNKNOWN) sb.AppendFormat("STATUS:{0}",this.Status).AppendLine();
            if (this.Summary != null) sb.AppendFormat("SUMMARY:{0}", this.Summary).AppendLine();
            if (this.Transparency != TRANSP.UNKNOWN) sb.AppendFormat("TRANSP:{0}", this.Transparency).AppendLine();
            if (this.Url != null) sb.AppendFormat("URL:{0}", this.Url).AppendLine();
            if (this.RecurrenceId != null) sb.Append(this.RecurrenceId).AppendLine();
            if (this.RecurrenceRule != null) sb.AppendFormat("RRULE:{0}", this.RecurrenceRule).AppendLine();
            if (this.End != null)
            {
                if (this.Start.TimeZoneId != null) sb.AppendFormat("DTEND;{0}:{1}", this.End.TimeZoneId, this.End).AppendLine();
                else sb.AppendFormat("DTEND:{0}", this.End).AppendLine();
            }
            else if (this.Duration != null) sb.Append(this.Duration).AppendLine();

            if(!this.Attachments.NullOrEmpty())
            {
                foreach (var attachment in this.Attachments) if(attachment != null) sb.Append(attachment).AppendLine();
            }

            if(!this.Attendees.NullOrEmpty())
            {
                foreach (var attendee in this.Attendees) if(attendee != null) sb.Append(attendee).AppendLine();
            }

            if (this.Categories != null && !this.Categories.Values.NullOrEmpty()) sb.Append(this.Categories).AppendLine();

            if (!this.Comments.NullOrEmpty())
            {
                foreach (var comment in Comments) if (comment != null) sb.AppendFormat("COMMENT:{0}", comment).AppendLine();
            }

            if (!this.Contacts.NullOrEmpty())
            {
                foreach (var contact in this.Contacts) if (contact != null) sb.AppendFormat("TEXT:{0}", contact).AppendLine();
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

            if (!this.IANAProperties.NullOrEmpty())
            {
                foreach (var iana in this.IANAProperties) if (iana != null) sb.Append(iana).AppendLine();
            }

            if (!this.XProperties.NullOrEmpty())
            {
                foreach (var xprop in this.XProperties) if (xprop != null) sb.Append(xprop).AppendLine();
            }

            sb.Append("END:VEVENT");
            return sb.ToString();
        }



    }

    #endregion


}
