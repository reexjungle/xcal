using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.extensions;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.operations;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace reexjungle.xcal.domain.models
{
    /// <summary>
    /// Specifes the VEVENT component of the iCalendar Core Object
    /// </summary>
    [DataContract]
    public class VEVENT : IEVENT, IEquatable<VEVENT>, IComparable<VEVENT>, IContainsKey<Guid>
    {
        private DATE_TIME start;
        private DATE_TIME end;
        private DURATION duration;

        public static readonly VEVENT Empty = new VEVENT();

        /// <summary>
        /// Gets or sets the unique identifier of the event..
        /// </summary>
        [DataMember]
        [Index(Unique = true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of a non-recurrent event.
        /// </summary>
        [DataMember]
        public Guid Uid
        {
            get { return Id; }
            set
            {
                Id = value;
            }
        }

        [DataMember]
        [Index(Unique = false)]
        public DATE_TIME Datestamp { get; set; }

        [DataMember]
        public DATE_TIME Start
        {
            get { return start; }
            set
            {
                start = value;
                end = start + duration;
            }
        }

        [DataMember]
        public CLASS Classification { get; set; }

        [DataMember]
        public DATE_TIME Created { get; set; }

        [DataMember]
        [StringLength(int.MaxValue)]
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
        [StringLength(int.MaxValue)]
        public RECURRENCE_ID RecurrenceId { get; set; }

        [DataMember]
        [Ignore]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        public DATE_TIME End
        {
            get { return end; }
            set
            {
                end = value;
                duration = end - start;
            }
        }

        [DataMember]
        public DURATION Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                end = start + duration;
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
        [StringLength(int.MaxValue)]
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
        public Dictionary<Guid, IANA_PROPERTY> IANAProperties { get; set; }

        [DataMember]
        [Ignore]
        public Dictionary<Guid, X_PROPERTY> XProperties { get; set; }

        public VEVENT()
        {
            RecurrenceId = null;
            Datestamp = new DATE_TIME(DateTime.UtcNow);
            Created = Datestamp;
            LastModified = Datestamp;
            AttachmentBinaries = new List<ATTACH_BINARY>();
            AttachmentUris = new List<ATTACH_URI>();
            Attendees = new List<ATTENDEE>();
            Categories = new CATEGORIES();
            Contacts = new List<CONTACT>();
            Comments = new List<COMMENT>();
            ExceptionDates = new List<EXDATE>();
            RequestStatuses = new List<REQUEST_STATUS>();
            RelatedTos = new List<RELATEDTO>();
            RecurrenceDates = new List<RDATE>();
            AudioAlarms = new List<AUDIO_ALARM>();
            DisplayAlarms = new List<DISPLAY_ALARM>();
            EmailAlarms = new List<EMAIL_ALARM>();
            IANAProperties = new Dictionary<Guid, IANA_PROPERTY>();
            XProperties = new Dictionary<Guid, X_PROPERTY>();
        }

        public VEVENT(DATE_TIME dtstamp, Guid uid, DATE_TIME start, DATE_TIME end, PRIORITY priority, ORGANIZER organizer = null, LOCATION location = null,
            STATUS status = STATUS.NEEDS_ACTION, SUMMARY summary = null, TRANSP transparency = TRANSP.TRANSPARENT,
            RECURRENCE_ID recurid = null, RECUR rrule = null, List<ATTENDEE> attendees = null, CATEGORIES categories = null, List<RELATEDTO> relatedtos = null)
        {
            Datestamp = dtstamp;
            Uid = uid;
            Start = start;
            Organizer = organizer;
            Location = location;
            Priority = priority;
            Status = status;
            Summary = summary;
            Transparency = transparency;
            RecurrenceId = recurid;
            RecurrenceRule = rrule;
            this.end = end;
            Attendees = attendees;
            Categories = categories;
            RelatedTos = relatedtos;
        }

        public VEVENT(DATE_TIME dtstamp, Guid uid, DATE_TIME start, DURATION duration, PRIORITY priority, ORGANIZER organizer = null, LOCATION location = null,
             STATUS status = STATUS.NEEDS_ACTION, SUMMARY summary = null, TRANSP transparency = TRANSP.TRANSPARENT, RECURRENCE_ID recurid = null, RECUR rrule = null, List<ATTENDEE> attendees = null, CATEGORIES categories = null, List<RELATEDTO> relatedtos = null)
        {
            Datestamp = dtstamp;
            Uid = uid;
            Start = start;
            Organizer = organizer;
            Location = location;
            Priority = priority;
            Status = status;
            Summary = summary;
            Transparency = transparency;
            RecurrenceId = recurid;
            RecurrenceRule = rrule;
            Duration = duration;
            Attendees = attendees;
            Categories = categories;
            RelatedTos = relatedtos;
        }

        public VEVENT(IEVENT value)
        {
            if (value == null) throw new ArgumentNullException("value");
            Uid = value.Uid;
            RecurrenceId = value.RecurrenceId;
            Start = value.Start;
            Organizer = value.Organizer;
            Location = value.Location;
            Sequence = value.Sequence;
            Priority = value.Priority;
            Status = value.Status;
            Position = value.Position;
            Classification = value.Classification;
            Transparency = value.Transparency;
            Summary = value.Summary;
            Description = value.Description;
            Transparency = value.Transparency;
            RecurrenceId = value.RecurrenceId;
            RecurrenceRule = value.RecurrenceRule;
            end = value.End;
            duration = value.Duration;
            AttachmentBinaries = value.AttachmentBinaries;
            AttachmentUris = value.AttachmentUris;
            Comments = value.Comments;
            Contacts = value.Contacts;
            ExceptionDates = value.ExceptionDates;
            RecurrenceDates = value.RecurrenceDates;
            RequestStatuses = value.RequestStatuses;
            Attendees = value.Attendees;
            Categories = value.Categories;
            RelatedTos = value.RelatedTos;
            AudioAlarms = value.AudioAlarms;
            DisplayAlarms = value.DisplayAlarms;
            EmailAlarms = value.EmailAlarms;
            IANAProperties = value.IANAProperties;
            XProperties = value.XProperties;
        }

        public bool Equals(VEVENT other)
        {
            //primary reference
            var equals = Uid.Equals(other.Uid);

            if (equals && RecurrenceId != null && other.RecurrenceId != null)
                equals = RecurrenceId == other.RecurrenceId;

            //secondary reference if both events are equal by Uid/Recurrence Id
            if (equals) equals = Sequence == other.Sequence;

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
            return Equals((VEVENT)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(VEVENT left, VEVENT right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VEVENT left, VEVENT right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(VEVENT other)
        {
            var compare = Id.CompareTo(other.Id);
            if (compare == 0) compare = Sequence.CompareTo(other.Sequence);
            if (compare == 0) compare = Datestamp.CompareTo(other.Datestamp);
            return compare;
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
            sb.AppendFormat("DTSTAMP:{0}", Datestamp.ToString()).AppendLine();
            sb.AppendFormat("UID:{0}", Uid.ToString()).AppendLine();
            if (Start.TimeZoneId != null)
                sb.AppendFormat("DTSTART;{0}:{1}", Start.TimeZoneId.ToString(), Start.ToString()).AppendLine();
            else
                sb.AppendFormat("DTSTART:{0}", Start.ToString()).AppendLine();
            if (Classification != CLASS.UNKNOWN) sb.AppendFormat("CLASS:{0}", Classification.ToString()).AppendLine();
            sb.AppendFormat("CREATED:{0}", Created.ToString()).AppendLine();
            if (Description != null) sb.Append(Description.ToString()).AppendLine();
            if (Position != default(GEO)) sb.Append(Position.ToString()).AppendLine();
            sb.AppendFormat("LAST-MODIFIED:{0}", LastModified.ToString()).AppendLine();
            if (Location != null) sb.Append(Location.ToString()).AppendLine();
            if (Organizer != null) sb.Append(Organizer.ToString()).AppendLine();
            if (Priority != default(PRIORITY)) sb.Append(Priority.ToString()).AppendLine();
            sb.AppendFormat("SEQUENCE:{0}", Sequence.ToString()).AppendLine();
            if (Status != STATUS.UNKNOWN) sb.AppendFormat("STATUS:{0}", Status.ToString()).AppendLine();
            if (Summary != null) sb.Append(Summary.ToString()).AppendLine();
            if (Transparency != TRANSP.UNKNOWN) sb.AppendFormat("TRANSP:{0}", Transparency.ToString()).AppendLine();
            if (Url != null) sb.AppendFormat("URL:{0}", Url.ToString()).AppendLine();
            if (RecurrenceId != null) sb.Append(RecurrenceId.ToString()).AppendLine();
            if (RecurrenceRule != null) sb.AppendFormat("RRULE:{0}", RecurrenceRule.ToString()).AppendLine();
            if (End != default(DATE_TIME))
            {
                if (End.TimeZoneId != null)
                    sb.AppendFormat("DTEND;{0}:{1}", End.TimeZoneId.ToString(), End.ToString()).AppendLine();
                else
                    sb.AppendFormat("DTEND:{0}", End.ToString()).AppendLine();
            }
            else if (Duration != default(DURATION)) sb.Append(Duration.ToString()).AppendLine();

            if (!AttachmentBinaries.NullOrEmpty())
            {
                foreach (var attachment in AttachmentBinaries) if (attachment != null) sb.Append(attachment.ToString()).AppendLine();
            }

            if (!Attendees.NullOrEmpty())
            {
                foreach (var attendee in Attendees)
                {
                    if (attendee != null) sb.Append(attendee.ToString()).AppendLine();
                }
            }

            if (Categories != null && !Categories.Values.NullOrEmpty()) sb.Append(Categories.ToString()).AppendLine();

            if (!Comments.NullOrEmpty())
            {
                foreach (var comment in Comments) if (comment != null) sb.Append(comment.ToString()).AppendLine();
            }

            if (!Contacts.NullOrEmpty())
            {
                foreach (var contact in Contacts) if (contact != null) sb.Append(contact.ToString()).AppendLine();
            }

            if (!ExceptionDates.NullOrEmpty())
            {
                foreach (var exdate in ExceptionDates) if (exdate != null) sb.Append(exdate.ToString()).AppendLine();
            }

            if (!RequestStatuses.NullOrEmpty())
            {
                foreach (var reqstat in RequestStatuses) if (reqstat != null) sb.Append(reqstat.ToString()).AppendLine();
            }

            if (!RelatedTos.NullOrEmpty())
            {
                foreach (var relatedto in RelatedTos) if (relatedto != null) sb.Append(relatedto.ToString()).AppendLine();
            }

            if (!Resources.NullOrEmpty())
            {
                foreach (var resource in Resources) if (resource != null) sb.Append(resource.ToString()).AppendLine();
            }

            if (!RecurrenceDates.NullOrEmpty())
            {
                foreach (var rdate in RecurrenceDates) if (rdate != null) sb.Append(rdate.ToString()).AppendLine();
            }

            if (!AudioAlarms.NullOrEmpty())
            {
                foreach (var alarm in AudioAlarms) if (alarm != null) sb.Append(alarm.ToString()).AppendLine();
            }
            if (!DisplayAlarms.NullOrEmpty())
            {
                foreach (var alarm in DisplayAlarms) if (alarm != null) sb.Append(alarm.ToString()).AppendLine();
            }
            if (!EmailAlarms.NullOrEmpty())
            {
                foreach (var alarm in EmailAlarms) if (alarm != null) sb.Append(alarm.ToString()).AppendLine();
            }
            if (!IANAProperties.NullOrEmpty())
            {
                foreach (var iana in IANAProperties.Values) if (iana != null) sb.Append(iana.ToString()).AppendLine();
            }

            if (!XProperties.NullOrEmpty())
            {
                foreach (var xprop in XProperties.Values) if (xprop != null) sb.Append(xprop.ToString()).AppendLine();
            }

            sb.Append("END:VEVENT");
            return sb.ToString().ToUtf8String();
        }

        public List<TEVENT> GenerateRecurrences<TEVENT>(IKeyGenerator<Guid> keyGenerator)
            where TEVENT : class, IEVENT, new()
        {
            var recurs = new List<TEVENT>();
            var dates = Start.GenerateRecurrences(RecurrenceRule);
            if (!RecurrenceDates.NullOrEmpty())
            {
                var rdates = RecurrenceDates.Where(x => !x.DateTimes.NullOrEmpty()).SelectMany(x => x.DateTimes).ToList();
                var rperiods = RecurrenceDates.Where(x => !x.Periods.NullOrEmpty()).SelectMany(x => x.Periods).ToList();

                if (!rdates.NullOrEmpty()) dates.AddRange(rdates);
                if (!rperiods.NullOrEmpty()) dates.AddRange(rperiods.Select(x => x.Start));
            }

            if (!ExceptionDates.NullOrEmpty())
            {
                var exdates = ExceptionDates.Where(x => !x.DateTimes.NullOrEmpty()).SelectMany(x => x.DateTimes).ToList();
                if (!exdates.NullOrEmpty()) dates = dates.Except(exdates).ToList();
            }

            int count = 0;
            foreach (var recurrence in dates)
            {
                var instance = new VEVENT
                {
                    Id = keyGenerator.GetNext(),
                    Start = recurrence,
                    End = recurrence + Duration,
                    RecurrenceRule = null
                };

                instance.RecurrenceId = new RECURRENCE_ID
                {
                    Id = instance.Id,
                    Range = RANGE.THISANDFUTURE,
                    TimeZoneId = recurrence.TimeZoneId,
                    Value = instance.Start
                };
                recurs.Add(instance as TEVENT);
            }

            return recurs;
        }
    }
}