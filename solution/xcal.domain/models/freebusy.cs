using reexjungle.xcal.domain.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.serialization;
using reexjungle.xcal.infrastructure.extensions;


namespace reexjungle.xcal.domain.models
{
    [DataContract]
    public class VFREEBUSY : IFREEBUSY, IEquatable<VFREEBUSY>, IContainsKey<Guid>, ICalendarSerializable
    {
        /// <summary>
        /// Gets or sets the unique identifier of the free/busy time.
        /// </summary>
        [DataMember]
        [Index(Unique = true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of a non-recurrent free/busy time.
        /// </summary>
        [DataMember]
        [Index(Unique = false)]
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
        [Ignore]
        public ORGANIZER Organizer { get; set; }

        [DataMember]
        public URL Url { get; set; }

        [DataMember]
        public DATE_TIME End { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTACH> Attachments { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTENDEE> Attendees { get; set; }

        [DataMember]
        [Ignore]
        public List<COMMENT> Comments { get; set; }

        [DataMember]
        [Ignore]
        public List<FREEBUSY> FreeBusyIntervals { get; set; }

        [DataMember]
        [Ignore]
        public List<REQUEST_STATUS> RequestStatuses { get; set; }

        public bool Equals(VFREEBUSY other)
        {
            //primary reference
            var equals = Id.Equals(other.Id);

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
            return Equals((VFREEBUSY)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Datestamp.GetHashCode();
        }

        public static bool operator ==(VFREEBUSY left, VFREEBUSY right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VFREEBUSY left, VFREEBUSY right)
        {
            return !Equals(left, right);
        }

        private void WriteCalendarPrimitives(CalendarWriter writer)
        {
            writer.AppendProperty("DTSTAMP", Datestamp);

            writer.AppendProperty("UID", Uid);

            if (Start != default(DATE_TIME)) writer.AppendProperty("DTSTART", Start);

            if (End != default(DATE_TIME)) writer.AppendProperty("DTEND", End);

            if (Organizer != default(ORGANIZER)) writer.AppendProperty(Organizer);

            if (Url != default(URL)) writer.AppendProperty(Url);

        }

        private void WriteCalendarEnumerables(CalendarWriter writer)
        {
            if (Attendees.Any()) writer.AppendProperties(Attendees);

            if (Comments.Any()) writer.AppendProperties(Comments);

            if (RequestStatuses.Any()) writer.AppendProperties(RequestStatuses);

            if (Attachments.Any()) writer.AppendProperties(Attachments);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.WriteStartComponent(nameof(VFREEBUSY));
            WriteCalendarPrimitives(writer);
            WriteCalendarEnumerables(writer);
            writer.WriteLine();
            writer.WriteEndComponent(nameof(VFREEBUSY));
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Datestamp != default(DATE_TIME) && !string.IsNullOrEmpty(Uid) && !string.IsNullOrWhiteSpace(Uid);
    }
}