using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.extensions;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.operations;
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

        /// <summary>
        ///
        /// </summary>
        public static readonly VEVENT Empty = new VEVENT
        {
            Datestamp = default(DATE_TIME)
        };

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
                LastModified = DateTime.UtcNow.ToDATE_TIME();
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
        public URL Url { get; set; }

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

        public VEVENT(IEVENT @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            Uid = @event.Uid;
            Created = @event.Created;
            Datestamp = @event.Datestamp;
            LastModified = @event.LastModified;
            RecurrenceId = new RECURRENCE_ID(@event.RecurrenceId);
            Start = @event.Start;
            Organizer = new ORGANIZER(@event.Organizer);
            Location = new LOCATION(@event.Location);
            Sequence = @event.Sequence;
            Priority = @event.Priority;
            Status = @event.Status;
            Position = @event.Position;
            Classification = @event.Classification;
            Transparency = @event.Transparency;
            Summary = new SUMMARY(@event.Summary);
            Description = new DESCRIPTION(@event.Description);
            RecurrenceRule = new RECUR(@event.RecurrenceRule);
            end = @event.End;
            duration = @event.Duration;

            AttachmentBinaries = @event.AttachmentBinaries.Any()
                ? @event.AttachmentBinaries.Select(x => new ATTACH_BINARY(x)).ToList()  
                : new List<ATTACH_BINARY>();

            AttachmentUris = @event.AttachmentUris.Any()
                ? @event.AttachmentUris.Select(x => new ATTACH_URI(x)).ToList()
                : new List<ATTACH_URI>();

            Comments = @event.Comments.Any()
                ? @event.Comments.Select(x => new COMMENT(x)).ToList()
                : new List<COMMENT>();

            Contacts = @event.Contacts.Any()
                ? @event.Contacts.Select(x => new CONTACT(x)).ToList()
                : new List<CONTACT>();

            ExceptionDates = @event.ExceptionDates.Any()
                ? @event.ExceptionDates.Select(x => x).ToList()
                :new List<EXDATE>();

            RecurrenceDates = @event.RecurrenceDates.Any()
                ? @event.RecurrenceDates.Select(x => x).ToList():
                new List<RDATE>();

            RequestStatuses = @event.RequestStatuses;
            Attendees = @event.Attendees;
            Categories = @event.Categories;
            RelatedTos = @event.RelatedTos;
            AudioAlarms = @event.AudioAlarms;
            DisplayAlarms = @event.DisplayAlarms;
            EmailAlarms = @event.EmailAlarms;
            IANAProperties = @event.IANAProperties;
            XProperties = @event.XProperties;
        }

        public bool Equals(VEVENT other)
        {
            //primary reference
            var equals = Id.Equals(other.Id);

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
            if (obj.GetType() != GetType()) return false;
            return Equals((VEVENT)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(VEVENT left, VEVENT right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(VEVENT left, VEVENT right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(VEVENT other)
        {
            var compare = Id.CompareTo(other.Id);
            if (compare == 0)
                compare = (RecurrenceId != null && other.RecurrenceId != null)
                    ? RecurrenceId.Id.CompareTo(other.RecurrenceId.Id)
                    : compare;
            if (compare == 0) compare = Sequence.CompareTo(other.Sequence);
            if (compare == 0) compare = Datestamp.CompareTo(other.Datestamp);
            return compare;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator <(VEVENT a, VEVENT b)
        {
            if (ReferenceEquals(null, a)) return false;
            if (ReferenceEquals(null, b)) return true;
            return a.CompareTo(b) < 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >(VEVENT a, VEVENT b)
        {
            if (ReferenceEquals(null, a)) return false;
            if (ReferenceEquals(null, b)) return true;

            return a.CompareTo(b) > 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VEVENT").AppendLine();
            sb.AppendFormat("DTSTAMP:{0}", Datestamp).AppendLine();
            sb.AppendFormat("UID:{0}", Uid).AppendLine();
            if (Start.TimeZoneId != null)
                sb.AppendFormat("DTSTART;{0}:{1}", Start.TimeZoneId, Start).AppendLine();
            else
                sb.AppendFormat("DTSTART:{0}", Start).AppendLine();
            if (Classification != CLASS.NONE) sb.AppendFormat("CLASS:{0}", Classification).AppendLine();
            sb.AppendFormat("CREATED:{0}", Created).AppendLine();
            if (Description != null) sb.Append(Description).AppendLine();
            if (Position != default(GEO)) sb.Append(Position).AppendLine();
            sb.AppendFormat("LAST-MODIFIED:{0}", LastModified).AppendLine();
            if (Location != null) sb.Append(Location).AppendLine();
            if (Organizer != null) sb.Append(Organizer).AppendLine();
            if (Priority != default(PRIORITY)) sb.Append(Priority).AppendLine();
            sb.AppendFormat("SEQUENCE:{0}", Sequence).AppendLine();
            if (Status != STATUS.NONE) sb.AppendFormat("STATUS:{0}", Status).AppendLine();
            if (Summary != null) sb.Append(Summary).AppendLine();
            if (Transparency != TRANSP.NONE) sb.AppendFormat("TRANSP:{0}", Transparency).AppendLine();
            if (Url != null) sb.AppendFormat("URL:{0}", Url).AppendLine();
            if (RecurrenceId != null) sb.Append(RecurrenceId).AppendLine();
            if (RecurrenceRule != null) sb.AppendFormat("RRULE:{0}", RecurrenceRule).AppendLine();
            if (End != default(DATE_TIME))
            {
                if (End.TimeZoneId != null)
                    sb.AppendFormat("DTEND;{0}:{1}", End.TimeZoneId, End).AppendLine();
                else
                    sb.AppendFormat("DTEND:{0}", End).AppendLine();
            }
            else if (Duration != default(DURATION)) sb.Append(Duration).AppendLine();

            if (!AttachmentBinaries.NullOrEmpty())
            {
                foreach (var attachment in AttachmentBinaries.Where(attachment => attachment != null))
                    sb.Append(attachment).AppendLine();
            }

            if (!Attendees.NullOrEmpty())
            {
                foreach (var attendee in Attendees.Where(attendee => attendee != null))
                {
                    sb.Append(attendee).AppendLine();
                }
            }

            if (Categories != null && !Categories.Values.NullOrEmpty()) sb.Append(Categories).AppendLine();

            if (!Comments.NullOrEmpty())
            {
                foreach (var comment in Comments.Where(comment => comment != null))
                    sb.Append(comment).AppendLine();
            }

            if (!Contacts.NullOrEmpty())
            {
                foreach (var contact in Contacts.Where(contact => contact != null))
                    sb.Append(contact).AppendLine();
            }

            if (!ExceptionDates.NullOrEmpty())
            {
                foreach (var exdate in ExceptionDates.Where(exdate => exdate != null))
                    sb.Append(exdate).AppendLine();
            }

            if (!RequestStatuses.NullOrEmpty())
            {
                foreach (var reqstat in RequestStatuses.Where(reqstat => reqstat != null))
                    sb.Append(reqstat).AppendLine();
            }

            if (!RelatedTos.NullOrEmpty())
            {
                foreach (var relatedto in RelatedTos.Where(relatedto => relatedto != null))
                    sb.Append(relatedto).AppendLine();
            }

            if (!Resources.NullOrEmpty())
            {
                foreach (var resource in Resources.Where(resource => resource != null))
                    sb.Append(resource).AppendLine();
            }

            if (!RecurrenceDates.NullOrEmpty())
            {
                foreach (var rdate in RecurrenceDates.Where(rdate => rdate != null))
                    sb.Append(rdate).AppendLine();
            }

            if (!AudioAlarms.NullOrEmpty())
            {
                foreach (var alarm in AudioAlarms.Where(alarm => alarm != null))
                    sb.Append(alarm).AppendLine();
            }
            if (!DisplayAlarms.NullOrEmpty())
            {
                foreach (var alarm in DisplayAlarms.Where(alarm => alarm != null))
                    sb.Append(alarm).AppendLine();
            }
            if (!EmailAlarms.NullOrEmpty())
            {
                foreach (var alarm in EmailAlarms.Where(alarm => alarm != null))
                    sb.Append(alarm).AppendLine();
            }
            if (!IANAProperties.NullOrEmpty())
            {
                foreach (var iana in IANAProperties.Values.Where(iana => iana != null))
                    sb.Append(iana).AppendLine();
            }

            if (!XProperties.NullOrEmpty())
            {
                foreach (var xprop in XProperties.Values.Where(xprop => xprop != null))
                    sb.Append(xprop).AppendLine();
            }

            sb.Append("END:VEVENT");
            return sb.ToString().ToUtf8String();
        }
    }
}