using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.extensions;
using reexjungle.xcal.infrastructure.serialization;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;

namespace reexjungle.xcal.domain.models
{
    [DataContract]
    public class VTODO : ITODO, IEquatable<VTODO>, IContainsKey<Guid>, ICalendarSerializable
    {
        private DATE_TIME due;
        private DURATION duration;

        /// <summary>
        ///     Gets or sets the unique identifier of the To-Do.
        /// </summary>
        [DataMember]
        [Index(Unique = true)]
        public Guid Id { get; set; }

        public bool Equals(VTODO other)
        {
            //primary reference
            var equals = Uid.Equals(other.Uid, StringComparison.OrdinalIgnoreCase);

            if (equals && RecurrenceId != null && other.RecurrenceId != null)
                equals = RecurrenceId == other.RecurrenceId;

            //secondary reference if both todos are equal by Uid/Recurrence Id
            if (equals) equals = Sequence == other.Sequence;

            //tie-breaker
            if (equals)
                equals = Datestamp == other.Datestamp;

            return equals;
        }

        /// <summary>
        ///     Gets or sets the unique identifier of a non-recurrent To-Do.
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
        public DATE_TIME Completed { get; set; }

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
        public int Percent { get; set; }

        [DataMember]
        public PRIORITY Priority { get; set; }

        [DataMember]
        public int Sequence { get; set; }

        [DataMember]
        public STATUS Status { get; set; }

        [DataMember]
        public SUMMARY Summary { get; set; }

        [DataMember]
        public URL Url { get; set; }

        [DataMember]
        [Ignore]
        public RECURRENCE_ID RecurrenceId { get; set; }

        [DataMember]
        [Ignore]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        public DATE_TIME Due
        {
            get { return due; }
            set
            {
                due = value;
                if (duration == default(DURATION))
                    duration = due - Start;
            }
        }

        [DataMember]
        public DURATION Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                if (due == default(DATE_TIME)) due = Start + duration;
            }
        }

        [DataMember]
        [Ignore]
        public List<ATTACH> Attachments { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTENDEE> Attendees { get; set; }

        [DataMember]
        [Ignore]
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
            return Equals((VTODO) obj);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(VTODO left, VTODO right) => Equals(left, right);

        public static bool operator !=(VTODO left, VTODO right) => !Equals(left, right);

        private void WriteCalendarPrimitives(CalendarWriter writer)
        {
            writer.AppendProperty("DTSTAMP", Datestamp);

            writer.AppendProperty("UID", Uid);

            if (Start != default(DATE_TIME)) writer.AppendProperty("DTSTART", Start);

            if (Duration != default(DURATION)) writer.AppendProperty("DURATION", Duration);

            if (Classification != default(CLASS)) writer.AppendProperty("CLASS", Classification.ToString());

            if (Created != default(DATE_TIME)) writer.AppendProperty("CREATED", Created);

            if (Description != default(DESCRIPTION)) writer.AppendProperty(Description);

            if (LastModified != default(DATE_TIME)) writer.AppendProperty("LAST-MODIFIED", Created);

            if (Location != default(LOCATION)) writer.AppendProperty(Location);

            if (Organizer != default(ORGANIZER)) writer.AppendProperty(Organizer);

            if (Priority != default(PRIORITY)) writer.AppendProperty(Priority);

            writer.AppendProperty("SEQUENCE", Sequence.ToString());

            if (Status != default(STATUS)) writer.AppendProperty("STATUS", Status.ToString());

            if (Summary != default(SUMMARY)) writer.AppendProperty(Summary);

            if (Url != default(URL)) writer.AppendProperty(Url);

            if (RecurrenceId != default(RECURRENCE_ID)) writer.AppendProperty(RecurrenceId);

            if (RecurrenceRule != default(RECUR)) writer.AppendProperty("RRULE", RecurrenceRule);

            if (Categories != default(CATEGORIES)) writer.AppendProperty(Categories);

        }

        private void WriteCalendarEnumerables(CalendarWriter writer)
        {
            if (Attendees.Any()) writer.AppendProperties(Attendees);

            if (Comments.Any()) writer.AppendProperties(Comments);

            if (Contacts.Any()) writer.AppendProperties(Contacts);

            if (RelatedTos.Any()) writer.AppendProperties(RelatedTos);

            if (ExceptionDates.Any()) writer.AppendProperties(ExceptionDates);

            if (RecurrenceDates.Any()) writer.AppendProperties(RecurrenceDates);

            if (Resources.Any()) writer.AppendProperties(Resources);

            if (RequestStatuses.Any()) writer.AppendProperties(RequestStatuses);

            if (Alarms.Any()) writer.AppendProperties(Alarms);

            if (Attachments.Any()) writer.AppendProperties(Attachments);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.WriteStartComponent(nameof(VTODO));
            WriteCalendarPrimitives(writer);
            WriteCalendarEnumerables(writer);
            writer.WriteLine();
            writer.WriteEndComponent(nameof(VTODO));
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Datestamp != default(DATE_TIME) && !string.IsNullOrEmpty(Uid) && !string.IsNullOrWhiteSpace(Uid);
    }
}