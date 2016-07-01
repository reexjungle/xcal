using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.extensions;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.extensions;
using reexjungle.xcal.infrastructure.serialization;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;

namespace reexjungle.xcal.domain.models
{
    /// <summary>
    ///     Specifes the VEVENT component of the iCalendar Core Object
    /// </summary>
    [DataContract]
    [KnownType(typeof(ATTACH_BINARY))]
    [KnownType(typeof(ATTACH_URI))]
    [KnownType(typeof(AUDIO_ALARM))]
    [KnownType(typeof(DISPLAY_ALARM))]
    [KnownType(typeof(EMAIL_ALARM))]
    public class VEVENT : IEVENT, IEquatable<VEVENT>, IComparable<VEVENT>, IContainsKey<Guid>, ICalendarSerializable
    {
        /// <summary>
        /// </summary>
        public static readonly VEVENT Empty = new VEVENT
        {
            Datestamp = default(DATE_TIME)
        };

        private DURATION duration;
        private DATE_TIME end;
        private DATE_TIME start;

        public VEVENT(): this(new DATE_TIME(DateTime.UtcNow), string.Empty, new DATE_TIME())
        {

        }

        public VEVENT(IEVENT other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            Uid = other.Uid;
            Created = other.Created;
            Datestamp = other.Datestamp;
            LastModified = other.LastModified;
            RecurrenceId = new RECURRENCE_ID(other.RecurrenceId);
            Start = other.Start;
            Organizer = new ORGANIZER(other.Organizer);
            Location = new LOCATION(other.Location);
            Sequence = other.Sequence;
            Priority = other.Priority;
            Status = other.Status;
            GeoPosition = other.GeoPosition;
            Classification = other.Classification;
            Transparency = other.Transparency;
            Summary = new SUMMARY(other.Summary);
            Description = new DESCRIPTION(other.Description);
            RecurrenceRule = new RECUR(other.RecurrenceRule);
            end = other.End;
            duration = other.Duration;

            Attachments = other.Attachments != null && other.Attachments.Any()
                ? new List<ATTACH>(other.Attachments)
                : new List<ATTACH>();

            Comments = other.Comments != null && other.Comments.Any()
                ? new List<COMMENT>(other.Comments)
                : new List<COMMENT>();

            Contacts = other.Contacts != null && other.Contacts.Any()
                ? new List<CONTACT>(other.Contacts)
                : new List<CONTACT>();

            ExceptionDates = other.ExceptionDates != null && other.ExceptionDates.Any()
                ? new List<EXDATE>(other.ExceptionDates)
                : new List<EXDATE>();

            RecurrenceDates = other.RecurrenceDates != null && other.RecurrenceDates.Any()
                ? new List<RDATE>(other.RecurrenceDates)
                : new List<RDATE>();

            Attendees = other.Attendees != null && other.Attendees.Any()
                ? new List<ATTENDEE>(other.Attendees)
                : new List<ATTENDEE>();

            Alarms = other.Alarms != null && other.Alarms.Any()
                ? new List<VALARM>(other.Alarms)
                : new List<VALARM>();

            RequestStatuses = other.RequestStatuses;
            Categories = other.Categories;
            RelatedTos = other.RelatedTos;
        }


        public VEVENT(DATE_TIME datestamp, 
            string uid, 
            DATE_TIME start, 
            ORGANIZER organizer = null, 
            SUMMARY summary = null,
            DESCRIPTION description = null,
            LOCATION location = null, 
            GEO geo = default(GEO), 
            CLASS classification = CLASS.NONE, 
            PRIORITY priority = default(PRIORITY),
            STATUS status = STATUS.NONE,
            TRANSP transparency = TRANSP.NONE,
            URL url = null,
            RECURRENCE_ID recurrenceId = null)
        {
            Uid = uid;
            this.start = start;
            Datestamp = datestamp;
            Organizer = organizer;
            Summary = summary;
            Description = description;
            Location = location;
            GeoPosition = geo;
            Classification = classification;
            Priority = priority;
            Status = status;
            Transparency = transparency;
            Url = url;
            RecurrenceId = recurrenceId;
            Datestamp = datestamp;
            Created = Datestamp;
            LastModified = Datestamp;
            Attachments = new List<ATTACH>();
            Attendees = new List<ATTENDEE>();
            Categories = new CATEGORIES();
            Contacts = new List<CONTACT>();
            Comments = new List<COMMENT>();
            ExceptionDates = new List<EXDATE>();
            Resources = new List<RESOURCES>();
            RequestStatuses = new List<REQUEST_STATUS>();
            RelatedTos = new List<RELATEDTO>();
            RecurrenceDates = new List<RDATE>();
            Alarms = new List<VALARM>();

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
        ///     Gets or sets the unique identifier of the event..
        /// </summary>
        [DataMember]
        [Index(Unique = true)]
        public Guid Id { get; set; }

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

        /// <summary>
        ///     Gets or sets the unique identifier of left non-recurrent event.
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
        public GEO GeoPosition { get; set; }

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
        public List<ATTACH> Attachments { get; set; }

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
        public List<VALARM> Alarms { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((VEVENT) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(VEVENT left, VEVENT right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(VEVENT left, VEVENT right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(VEVENT left, VEVENT right)
        {
            if (ReferenceEquals(null, left)) return false;
            if (ReferenceEquals(null, right)) return true;
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(VEVENT left, VEVENT right)
        {
            if (ReferenceEquals(null, left)) return false;
            if (ReferenceEquals(null, right)) return true;

            return left.CompareTo(right) > 0;
        }

        private void WriteCalendarPrimitives(CalendarWriter writer)
        {
            writer.AppendProperty("DTSTAMP", Datestamp);
            writer.AppendProperty("UID", Uid);

            if (Start != default(DATE_TIME)) writer.AppendProperty("DTSTART", Start);

            if (End != default(DATE_TIME)) writer.AppendProperty("DTEND", End);

            if (Duration != default(DURATION)) writer.AppendProperty("DURATION", Duration);

            if (Classification != default(CLASS)) writer.AppendProperty("CLASS", Classification.ToString());

            if (Created != default(DATE_TIME)) writer.AppendProperty("CREATED", Created);

            if (Description != default(DESCRIPTION)) writer.AppendProperty(Description);

            if (GeoPosition != default(GEO)) writer.AppendProperty(GeoPosition);


            if (LastModified != default(DATE_TIME)) writer.AppendProperty("LAST-MODIFIED", Created);

            if (Location != default(LOCATION)) writer.AppendProperty(Location);

            if (Organizer != default(ORGANIZER)) writer.AppendProperty(Organizer);

            if (Priority != default(PRIORITY)) writer.AppendProperty(Priority);

            writer.AppendProperty("SEQUENCE", Sequence.ToString());

            if (Status != default(STATUS)) writer.AppendProperty("STATUS", Status.ToString());

            if (Summary != default(SUMMARY)) writer.AppendProperty(Summary);


            if (Transparency != default(TRANSP)) writer.AppendProperty("TRANSP", Transparency.ToString());

            if (Url != default(URL)) writer.AppendProperty(Url);

            if (RecurrenceId != default(RECURRENCE_ID)) writer.AppendProperty(RecurrenceId);

            if (RecurrenceRule != default(RECUR)) writer.AppendProperty("RRULE", RecurrenceRule);

            if (Categories != default(CATEGORIES))
            {
                writer.AppendProperty(Categories);
            }
        }

        private void WriteCalendarLists(CalendarWriter writer)
        {
            if (Attendees.Any()) writer.AppendProperties(Attendees);

            if (Comments.Any())writer.AppendProperties(Comments);

            if (Contacts.Any())writer.AppendProperties(Contacts);


            if (RelatedTos.Any()) writer.AppendProperties(RelatedTos);


            if (ExceptionDates.Any()) writer.AppendProperties(ExceptionDates);

            if (RecurrenceDates.Any())writer.AppendProperties(RecurrenceDates);

            if (Resources.Any())writer.AppendProperties(Resources);


            if (RequestStatuses.Any())writer.AppendProperties(RequestStatuses);

            if (Alarms.Any())writer.AppendProperties(Alarms);


            if (Attachments.Any()) writer.AppendProperties(Attachments);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.WriteStartComponent("VEVENT");

            WriteCalendarPrimitives(writer);

            WriteCalendarLists(writer);

            writer.WriteEndComponent("VEVENT");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }
}