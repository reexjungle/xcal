using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.domain.models
{
    [DataContract]
    public class VTIMEZONE: ITIMEZONE, IEquatable<VTIMEZONE>, IContainsKey<string>
    {
        public string Id { get; set; }

        [DataMember]
        public TZID TimeZoneId { get; set; }

        [DataMember]
        public URI Url { get; set; }

        [DataMember]
        public DATE_TIME LastModified { get; set; }

        [DataMember]
        public List<OBSERVANCE> StandardTimes { get; set; }

        [DataMember]
        public List<OBSERVANCE> DaylightTimes { get; set; }

        public bool Equals(VTIMEZONE other)
        {
            if (other == null) return false;
            return (this.TimeZoneId == other.TimeZoneId);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = obj as VTIMEZONE;
            if (other == null) return false;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.TimeZoneId.GetHashCode();
        }

        public static bool operator ==(VTIMEZONE a, VTIMEZONE b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(VTIMEZONE a, VTIMEZONE b)
        {
            if (a == null || b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VTIMEZONE").AppendLine();
            sb.AppendFormat("TZID={0}", this.TimeZoneId);
            if (!this.StandardTimes.NullOrEmpty())
            {
                sb.Append("BEGIN:STANDARD").AppendLine();
                this.StandardTimes.ForEach(x => sb.Append(x).AppendLine());
                sb.Append("END:STANDARD").AppendLine();
            }
            else
            {
                sb.Append("BEGIN:DAYLIGHT").AppendLine();
                this.DaylightTimes.ForEach(x => sb.Append(x).AppendLine());
                sb.Append("END:DAYLIGHT").AppendLine();
            }
            sb.Append("END:VTIMEZONE");
            return sb.ToString();
        }
    }

    [DataContract]
    public class OBSERVANCE: IOBSERVANCE, IEquatable<OBSERVANCE>, IContainsKey<string>
    {
        public string Id { get; set; }

        [DataMember]
        public DATE_TIME Start { get; set; }

        [DataMember]
        public UTC_OFFSET TimeZoneOffsetFrom { get; set; }

        [DataMember]
        public UTC_OFFSET TimeZoneOffsetTo { get; set; }

        [DataMember]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        public List<TEXT> Comments { get; set; }

        [DataMember]
        public List<RDATE> RecurrenceDates { get; set; }

        [DataMember]
        public List<TZNAME> TimeZoneNames { get; set; }

        public OBSERVANCE(DATE_TIME start, UTC_OFFSET from, UTC_OFFSET to)
        {
            this.Start = start;
            this.TimeZoneOffsetFrom = from;
            this.TimeZoneOffsetTo = to;
            this.RecurrenceDates = new List<RDATE>();
            this.Comments = new List<TEXT>();
            this.TimeZoneNames = new List<TZNAME>();
        }

        public bool Equals(OBSERVANCE other)
        {
            if (other == null) return false;
            return this.Start == other.Start &&
                (this.TimeZoneOffsetFrom) == (other.TimeZoneOffsetFrom) &&
                (this.TimeZoneOffsetTo) == (other.TimeZoneOffsetTo) &&
                (this.RecurrenceRule) == (other.RecurrenceRule ) &&
                this.Comments.OfType<TEXT>().AreDuplicatesOf(other.Comments.OfType<TEXT>()) &&
                this.RecurrenceDates.OfType<RDATE>().AreDuplicatesOf(other.RecurrenceDates.OfType<RDATE>()) &&
                this.TimeZoneNames.OfType<TZNAME>().AreDuplicatesOf(other.TimeZoneNames.OfType<TZNAME>());
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as OBSERVANCE);
        }

        public override int GetHashCode()
        {
            return this.Start.GetHashCode()^ 
                this.TimeZoneOffsetFrom.GetHashCode() ^ 
                this.TimeZoneOffsetTo.GetHashCode();
        }

        public static bool operator ==(OBSERVANCE a, OBSERVANCE b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(OBSERVANCE a, OBSERVANCE b)
        {
            if (a == null || b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.Start).AppendLine();
            sb.AppendFormat("TZOFFSETFROM", this.TimeZoneOffsetFrom).AppendLine();
            sb.AppendFormat("TZOFFSETTO", this.TimeZoneOffsetTo).AppendLine();
            if(this.RecurrenceRule != null) sb.Append(this.RecurrenceRule).AppendLine();
            this.RecurrenceDates.ForEach(x => sb.Append(x).AppendLine());
            this.Comments.ForEach(x => sb.Append(x).AppendLine());
            this.TimeZoneNames.ForEach(x => sb.Append(x).AppendLine());
            return sb.ToString();
        }


    }

}
