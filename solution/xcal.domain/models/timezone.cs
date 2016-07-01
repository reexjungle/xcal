using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.extensions;
using reexjungle.xcal.infrastructure.serialization;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;

namespace reexjungle.xcal.domain.models
{
    [DataContract]
    public class VTIMEZONE : ITIMEZONE, IEquatable<VTIMEZONE>, IContainsKey<Guid>, ICalendarSerializable
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public TZID TimeZoneId { get; protected set; }

        [DataMember]
        public URL Url { get; protected set; }

        [DataMember]
        public DATE_TIME LastModified { get; protected set; }

        [DataMember]
        [Ignore]
        public List<OBSERVANCE> Observances { get; protected set; }

        public VTIMEZONE()
        {
            Observances = new List<OBSERVANCE>();
        }

        public VTIMEZONE(TZID tzid, IEnumerable<STANDARD> standardTimes, DATE_TIME lastModified = default(DATE_TIME), URL url = null)
        {
            if (tzid == null) throw new ArgumentNullException(nameof(tzid));
            if (standardTimes == null) throw new ArgumentNullException(nameof(standardTimes));

            TimeZoneId = tzid;
            Observances = standardTimes.NullOrEmpty() ? new List<OBSERVANCE>() : new List<OBSERVANCE>(standardTimes);
            LastModified = lastModified;
            Url = url;
        }

        public VTIMEZONE(TZID tzid, IEnumerable<DAYLIGHT> daylights, DATE_TIME lastModified = default(DATE_TIME), URL url = null)
        {
            if (tzid == null) throw new ArgumentNullException(nameof(tzid));
            if (daylights == null) throw new ArgumentNullException(nameof(daylights));

            TimeZoneId = tzid;
            Observances = daylights.NullOrEmpty() ? new List<OBSERVANCE>() : new List<OBSERVANCE>(daylights);
            LastModified = lastModified;
            Url = url;
        }

        public bool Equals(VTIMEZONE other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VTIMEZONE)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(VTIMEZONE left, VTIMEZONE right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VTIMEZONE left, VTIMEZONE right)
        {
            return !Equals(left, right);
        }


        public void WriteCalendar(CalendarWriter writer)
        {
            if (TimeZoneId == null || Observances.Empty()) return;
            writer.WriteStartComponent("VTIMEZONE");
            writer.AppendProperty(TimeZoneId);
            writer.AppendProperties(Observances);

            if (LastModified != default(DATE_TIME)) writer.AppendProperty("LAST-MODIFIED", LastModified.ToString());

            if (Url != null) writer.AppendProperty(Url);

            writer.WriteEndComponent("VTIMEZONE");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [KnownType(typeof(STANDARD))]
    [KnownType(typeof(DAYLIGHT))]
    public abstract class OBSERVANCE : IOBSERVANCE, IEquatable<OBSERVANCE>, IContainsKey<Guid>, ICalendarSerializable
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public DATE_TIME Start { get; protected set; }

        [DataMember]
        public UTC_OFFSET TimeZoneOffsetFrom { get; protected set; }

        [DataMember]
        public UTC_OFFSET TimeZoneOffsetTo { get; protected set; }

        [DataMember]
        [Ignore]
        public RECUR RecurrenceRule { get; protected set; }

        [DataMember]
        [Ignore]
        public List<COMMENT> Comments { get; protected set; }

        [DataMember]
        [Ignore]
        public List<RDATE> RecurrenceDates { get; protected set; }

        [DataMember]
        [Ignore]
        public List<TZNAME> TimeZoneNames { get; protected set; }

        protected OBSERVANCE()
        {
            Comments = new List<COMMENT>();
            RecurrenceDates = new List<RDATE>();
            TimeZoneNames = new List<TZNAME>();
        }

        protected OBSERVANCE(DATE_TIME start, UTC_OFFSET from, UTC_OFFSET to,
            RECUR rrule = null,
            IEnumerable<COMMENT> comments = null,
            IEnumerable<RDATE> rdates = null,
            IEnumerable<TZNAME> tznames = null)
        {
            Start = start;
            TimeZoneOffsetFrom = from;
            TimeZoneOffsetTo = to;
            RecurrenceRule = rrule;
            Comments = comments.NullOrEmpty() ? new List<COMMENT>() : new List<COMMENT>(comments);
            RecurrenceDates = rdates.NullOrEmpty() ? new List<RDATE>() : new List<RDATE>(rdates);
            TimeZoneNames = tznames.NullOrEmpty() ? new List<TZNAME>() : new List<TZNAME>(tznames);
        }

        public bool Equals(OBSERVANCE other)
        {
            if (other == null) return false;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return Equals(obj as OBSERVANCE);
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^
                TimeZoneOffsetFrom.GetHashCode() ^
                TimeZoneOffsetTo.GetHashCode();
        }

        public abstract void WriteCalendar(CalendarWriter writer);

        public abstract void ReadCalendar(CalendarReader reader);

        public static bool operator ==(OBSERVANCE a, OBSERVANCE b)
        {
            if ((object)a == null || (object)b == null) return Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(OBSERVANCE a, OBSERVANCE b)
        {
            if ((object)a == null || (object)b == null) return !Equals(a, b);
            return !a.Equals(b);
        }
    }

    [DataContract]
    public class STANDARD : OBSERVANCE
    {
        public STANDARD()
        {
        }

        public STANDARD(DATE_TIME start, UTC_OFFSET from, UTC_OFFSET to,
            RECUR rrule = null,
            IEnumerable<COMMENT> comments = null,
            IEnumerable<RDATE> rdates = null,
            IEnumerable<TZNAME> tznames = null)
            : base(start, from, to, rrule, comments, rdates, tznames)
        {
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            if (Start == default(DATE_TIME)
                || TimeZoneOffsetFrom == default(UTC_OFFSET)
                || TimeZoneOffsetTo == default(UTC_OFFSET))
                return;

            writer.WriteStartComponent("STANDARD");
            writer.AppendProperty("DTSTART", TimeZoneOffsetFrom);
            writer.AppendProperty("TZOFFSETFROM", TimeZoneOffsetFrom);
            writer.AppendProperty("TZOFFSETTO", TimeZoneOffsetTo);

            if (RecurrenceRule != null) writer.WriteProperty("RRULE", RecurrenceRule);

            if (RecurrenceDates.Any())writer.AppendProperties(RecurrenceDates);

            if (Comments.Any()) writer.AppendProperties(Comments);

            if (TimeZoneNames.Any())writer.AppendProperties(TimeZoneNames);

            writer.WriteEndComponent("STANDARD");
        }
    }

    [DataContract]
    public class DAYLIGHT : OBSERVANCE
    {
        public DAYLIGHT()
        {
        }

        public DAYLIGHT(DATE_TIME start, UTC_OFFSET from, UTC_OFFSET to,
            RECUR rrule = null,
            IEnumerable<COMMENT> comments = null,
            IEnumerable<RDATE> rdates = null,
            IEnumerable<TZNAME> tznames = null)
            : base(start, from, to, rrule, comments, rdates, tznames)
        {
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            if (Start == default(DATE_TIME)
                || TimeZoneOffsetFrom == default(UTC_OFFSET)
                || TimeZoneOffsetTo == default(UTC_OFFSET))
                return;

            writer.WriteStartComponent("DAYLIGHT");
            writer.AppendProperty("DTSTART", TimeZoneOffsetFrom);
            writer.AppendProperty("TZOFFSETFROM", TimeZoneOffsetFrom);
            writer.AppendProperty("TZOFFSETTO", TimeZoneOffsetTo);

            if (RecurrenceRule != null) writer.WriteProperty("RRULE", RecurrenceRule);

            if (RecurrenceDates.Any()) writer.AppendProperties(RecurrenceDates);

            if (Comments.Any()) writer.AppendProperties(Comments);

            if (TimeZoneNames.Any()) writer.AppendProperties(TimeZoneNames);

            writer.WriteEndComponent("DAYLIGHT");
        }
    }
}