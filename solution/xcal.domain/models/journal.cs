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
    public class VJOURNAL : IJOURNAL, IEquatable<VJOURNAL>, IContainsKey<Guid>, ICalendarSerializable
    {

        /// <summary>
        ///     Gets or sets the unique identifier of the journal.
        /// </summary>
        [DataMember]
        [Index(Unique = true)]
        public Guid Id { get; set; }

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

        /// <summary>
        ///     Gets or sets the unique identifier of a non-recurrent journal.
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
        public URL Url { get; set; }

        [DataMember]
        public RECURRENCE_ID RecurrenceId { get; set; }

        [DataMember]
        [Ignore]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTACH> Attachments { get; set; }

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


        private void WriteCalendarPrimitives(CalendarWriter writer)
        {
            writer.AppendProperty("DTSTAMP", Datestamp);

            writer.AppendProperty("UID", Uid);

            if (Start != default(DATE_TIME)) writer.AppendProperty("DTSTART", Start);

            if (Classification != default(CLASS)) writer.AppendProperty("CLASS", Classification.ToString());

            if (Created != default(DATE_TIME)) writer.AppendProperty("CREATED", Created);

            if (Description != default(DESCRIPTION)) writer.AppendProperty(Description);

            if (LastModified != default(DATE_TIME)) writer.AppendProperty("LAST-MODIFIED", Created);

            if (Organizer != default(ORGANIZER)) writer.AppendProperty(Organizer);

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

            if (Attachments.Any()) writer.AppendProperties(Attachments);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.WriteStartComponent(nameof(VJOURNAL));
            WriteCalendarPrimitives(writer);
            WriteCalendarEnumerables(writer);
            writer.WriteLine();
            writer.WriteEndComponent(nameof(VJOURNAL));
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Datestamp != default(DATE_TIME) && !string.IsNullOrEmpty(Uid) && !string.IsNullOrWhiteSpace(Uid);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((VJOURNAL) obj);
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
    }
}