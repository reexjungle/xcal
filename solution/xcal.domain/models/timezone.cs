using reexjungle.xcal.domain.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace reexjungle.xcal.domain.models
{
    [DataContract]
    public class VTIMEZONE : ITIMEZONE, IEquatable<VTIMEZONE>, IContainsKey<Guid>
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public TZID TimeZoneId { get; set; }

        [DataMember]
        public URI Url { get; set; }

        [DataMember]
        public DATE_TIME LastModified { get; set; }

        [DataMember]
        [Ignore]
        public List<STANDARD> StandardTimes { get; set; }

        [DataMember]
        [Ignore]
        public List<DAYLIGHT> DaylightTimes { get; set; }

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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VTIMEZONE").AppendLine();
            sb.AppendFormat("TZID={0}", TimeZoneId).AppendLine();
            if (!StandardTimes.NullOrEmpty()) StandardTimes.ForEach(x => sb.Append(x).AppendLine());
            else if (!DaylightTimes.NullOrEmpty()) DaylightTimes.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:VTIMEZONE");
            return sb.ToString();
        }
    }

    [DataContract]
    [KnownType(typeof(STANDARD))]
    [KnownType(typeof(DAYLIGHT))]
    public abstract class OBSERVANCE : IOBSERVANCE, IEquatable<OBSERVANCE>, IContainsKey<Guid>
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public DATE_TIME Start { get; set; }

        [DataMember]
        public UTC_OFFSET TimeZoneOffsetFrom { get; set; }

        [DataMember]
        public UTC_OFFSET TimeZoneOffsetTo { get; set; }

        [DataMember]
        [Ignore]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        [Ignore]
        public List<COMMENT> Comments { get; set; }

        [DataMember]
        [Ignore]
        public List<RDATE> RecurrenceDates { get; set; }

        [DataMember]
        [Ignore]
        public List<TZNAME> TimeZoneNames { get; set; }

        protected OBSERVANCE()
        {
            Comments = new List<COMMENT>();
            RecurrenceDates = new List<RDATE>();
            TimeZoneNames = new List<TZNAME>();
        }

        protected OBSERVANCE(DATE_TIME start, UTC_OFFSET from, UTC_OFFSET to)
        {
            Start = start;
            TimeZoneOffsetFrom = from;
            TimeZoneOffsetTo = to;
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

        public STANDARD(DATE_TIME start, UTC_OFFSET from, UTC_OFFSET to)
            : base(start, from, to)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:STANDARD").AppendLine();
            sb.AppendFormat("DTSTART:{0}", Start).AppendLine();
            sb.AppendFormat("TZOFFSETFROM", TimeZoneOffsetFrom).AppendLine();
            sb.AppendFormat("TZOFFSETTO", TimeZoneOffsetTo).AppendLine();
            if (RecurrenceRule != null) sb.Append(RecurrenceRule).AppendLine();
            if (!RecurrenceDates.NullOrEmpty()) RecurrenceDates.ForEach(x => sb.Append(x).AppendLine());
            if (!Comments.NullOrEmpty()) Comments.ForEach(x => sb.Append(x).AppendLine());
            if (!TimeZoneNames.NullOrEmpty()) TimeZoneNames.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:STANDARD");
            return sb.ToString();
        }
    }

    [DataContract]
    public class DAYLIGHT : OBSERVANCE
    {
        public DAYLIGHT()
        {
        }

        public DAYLIGHT(DATE_TIME start, UTC_OFFSET from, UTC_OFFSET to)
            : base(start, from, to)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:DAYLIGHT").AppendLine();
            sb.AppendFormat("DTSTART:{0}", Start).AppendLine();
            sb.AppendFormat("TZOFFSETFROM", TimeZoneOffsetFrom).AppendLine();
            sb.AppendFormat("TZOFFSETTO", TimeZoneOffsetTo).AppendLine();
            if (RecurrenceRule != null) sb.Append(RecurrenceRule).AppendLine();
            if (!RecurrenceDates.NullOrEmpty()) RecurrenceDates.ForEach(x => sb.Append(x).AppendLine());
            if (!Comments.NullOrEmpty()) Comments.ForEach(x => sb.Append(x).AppendLine());
            if (!TimeZoneNames.NullOrEmpty()) TimeZoneNames.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:DAYLIGHT");
            return sb.ToString();
        }
    }
}