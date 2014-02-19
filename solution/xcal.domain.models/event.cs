﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ServiceStack.DataAnnotations;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.extensions;

namespace reexmonkey.xcal.domain.models
{
    #region Event core object

    /// <summary>
    /// Specifes a contract for the VEVENT component of the iCalendar Core Object
    /// </summary>
    [DataContract]
    [KnownType(typeof(DATE_TIME))]
    [KnownType(typeof(DESCRIPTION))]
    [KnownType(typeof(GEO))]
    [KnownType(typeof(LOCATION))]
    [KnownType(typeof(ORGANIZER))]
    [KnownType(typeof(PRIORITY))]
    [KnownType(typeof(SUMMARY))]
    [KnownType(typeof(URI))]
    [KnownType(typeof(RECURRENCE_ID))]
    [KnownType(typeof(RECUR))]
    [KnownType(typeof(DURATION))]
    [KnownType(typeof(ATTACH_BINARY))]
    [KnownType(typeof(ATTACH_URI))]
    [KnownType(typeof(ATTENDEE))]
    [KnownType(typeof(CATEGORIES))]
    [KnownType(typeof(COMMENT))]
    [KnownType(typeof(CONTACT))]
    [KnownType(typeof(EXDATE))]
    [KnownType(typeof(REQUEST_STATUS))]
    [KnownType(typeof(RESOURCES))]
    [KnownType(typeof(RELATEDTO))]
    [KnownType(typeof(RDATE))]
    [KnownType(typeof(AUDIO_ALARM))]
    [KnownType(typeof(DISPLAY_ALARM))]
    [KnownType(typeof(EMAIL_ALARM))]
    public class VEVENT : IEVENT, IEquatable<VEVENT>, IComparable<VEVENT>, IContainsId<string>
    {
        private IDATE_TIME dtstamp;
        private IDATE_TIME start;
        private IDATE_TIME end;
        private IDURATION duration;

        /// <summary>
        /// Gets or sets the unique identifier of the event. It is synonymous to the &quot;Uid&quot; property of the event. 
        /// </summary>
        [DataMember]
        [Index(Unique = true)] 
        public string Id
        {
            get { return this.Uid; }
            set { this.Uid = value; }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the event.
        /// </summary>
        [DataMember]
        public string Uid 
        {
            get { return this.Id; }
            set { this.Id = value; }
        }

        [DataMember]
        public IDATE_TIME Datestamp 
        {
            get { return dtstamp; }
            set 
            {
                if (value == null) throw new ArgumentNullException("Datestamp must not be null!");
                this.dtstamp = value; 
            } 
        }

        [DataMember]
        public IDATE_TIME Start
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
        public IDATE_TIME Created { get; set; }

        [DataMember]
        public ITEXT Description { get; set; }

        [DataMember]
        public IGEO Geo { get; set; }

        [DataMember]
        public IDATE_TIME LastModified { get; set; }

        [DataMember]
        public ITEXT Location { get; set; }

        [DataMember]
        [Ignore]
        public IORGANIZER Organizer { get; set; }

        [DataMember]
        public IPRIORITY Priority { get; set; }

        [DataMember]
        public int Sequence { get; set; }

        [DataMember]
        public STATUS Status { get; set; }

        [DataMember]
        public ITEXT Summary { get; set; }

        [DataMember]
        public TRANSP Transparency { get; set; }

        [DataMember]
        public IURI Url { get; set; }

        [DataMember]
        [Ignore]
        public IRECURRENCE_ID RecurrenceId { get; set; }

        [DataMember]
        [Ignore]
        public IRECUR RecurrenceRule { get; set; }

        [DataMember]
        public IDATE_TIME End
        {
            get { return this.end; }
            set { this.end = value; }
        }

        [DataMember]
        public IDURATION Duration
        {
            get { return this.duration; }
            set { this.duration = value; }
        }

        [DataMember]
        [Ignore]
        public List<IATTACH> Attachments { get; set; }

        [DataMember]
        [Ignore]
        public List<IATTENDEE> Attendees { get; set; }

        [DataMember]
        public ICATEGORIES Categories { get; set; }

        [DataMember]
        [Ignore]
        public List<ITEXT> Comments { get; set; }

        [DataMember]
        [Ignore]
        public List<ICONTACT> Contacts { get; set; }

        [DataMember]
        [Ignore]
        public List<IEXDATE> ExceptionDates { get; set; }

        [DataMember]
        [Ignore]
        public List<IREQUEST_STATUS> RequestStatuses { get; set; }

        [DataMember]
        [Ignore]
        public List<IRESOURCES> Resources { get; set; }

        [DataMember]
        [Ignore]
        public List<IRELATEDTO> RelatedTos { get; set; }

        [DataMember]
        [Ignore]
        public List<IRDATE> RecurrenceDates { get; set; }

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
            this.Datestamp = new DATE_TIME(DateTimeOffset.Now);
        }

        public VEVENT(IDATE_TIME dtstamp, string uid, IDATE_TIME dtstart,  IORGANIZER organizer = null, ITEXT location = null, 
            IPRIORITY priority = null, STATUS status = STATUS.NEEDS_ACTION, ITEXT summary = null, TRANSP transparency = TRANSP.TRANSPARENT,
            IRECURRENCE_ID recurid = null, IRECUR rrule = null, IDATE_TIME dtend = null,
            List<IATTENDEE> attendees = null, ICATEGORIES categories = null, List<IRELATEDTO> relatedtos = null)
        {
            this.Datestamp = dtstamp;
            this.Uid = uid;
            this.Start = dtstart;
            this.Organizer = organizer;
            this.Location = location;
            this.Priority = priority;
            this.Status = status;
            this.Summary = summary;
            this.Transparency = transparency;
            this.RecurrenceId = recurid;
            this.RecurrenceRule = rrule;
            this.end = dtend;
            this.Attendees = attendees;
            this.Categories = categories;
            this.RelatedTos = relatedtos;
        }


        public VEVENT(IDATE_TIME dtstamp, string uid, IDATE_TIME dtstart = null, IORGANIZER organizer = null, ITEXT location = null,
            IPRIORITY priority = null, STATUS status = STATUS.NEEDS_ACTION, ITEXT summary = null, TRANSP transparency = TRANSP.TRANSPARENT,
            IRECURRENCE_ID recurid = null, IRECUR rrule = null, IDURATION duration = null,
            List<IATTENDEE> attendees = null, ICATEGORIES categories = null, List<IRELATEDTO> relatedtos = null)
        {
            this.Datestamp = dtstamp;
            this.Uid = uid;
            this.Start = dtstart;
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
                var s1 = new DATE_TIME(this.Datestamp);
                var s2 = new DATE_TIME (other.Datestamp);
                compare = s1.CompareTo(s2);
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
            sb.AppendFormat("UID:{0}", this.Uid).AppendLine();

            if (this.Start != null)
            {
                if (this.Start.TimeZoneId != null )sb.AppendFormat("DTSTART;{0}:{1}", this.Start.TimeZoneId, this.Start).AppendLine();
                else sb.AppendFormat("DTSTART:{0}", this.Start).AppendLine();
            }

            if (this.Classification != CLASS.UNKNOWN) sb.AppendFormat("CLASS:{0}", this.Classification).AppendLine();
            if(this.Created != null) sb.AppendFormat("CREATED:{0}", this.Created).AppendLine();
            if (this.Description != null) sb.Append(this.Description).AppendLine();
            if (this.Geo != null) sb.Append(this.Geo).AppendLine();
            if (this.LastModified != null) sb.AppendFormat("LAST-MODIFIED:{0}", this.LastModified).AppendLine();
            if (this.Location != null) sb.Append(this.Location).AppendLine();
            if (this.Organizer != null) sb.Append(this.Organizer).AppendLine();
            if (this.Priority != null) sb.Append(this.Priority).AppendLine();
            sb.AppendFormat("SEQUENCE:{0}", this.Sequence).AppendLine();
            if (this.Status != STATUS.UNKNOWN) sb.AppendFormat("STATUS:{0}",this.Status).AppendLine();
            if (this.Summary != null) sb.AppendFormat("SUMMARY:{0}", this.Summary).AppendLine();
            if (this.Transparency != TRANSP.UNKNWON) sb.AppendFormat("TRANSP:{0}", this.Transparency);
            if (this.Url != null) sb.Append(this.Url).AppendLine();
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
                foreach (var comment in Comments) if (comment != null) sb.Append(comment).AppendLine();
            }

            if (!this.Contacts.NullOrEmpty())
            {
                foreach (var contact in this.Comments) if (contact != null) sb.Append(contact).AppendLine();
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