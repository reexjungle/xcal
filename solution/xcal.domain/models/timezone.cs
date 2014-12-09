using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

using reexjungle.foundation.essentials.contracts;
using reexjungle.foundation.essentials.concretes;
using reexjungle.xcal.domain.contracts;

namespace reexjungle.xcal.domain.models
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
        [Ignore]
        public List<STANDARD> StandardTimes { get; set; }

        [DataMember]
        [Ignore]
        public List<DAYLIGHT> DaylightTimes { get; set; }

        public bool Equals(VTIMEZONE other)
        {
            if (other == null) return false;
            return (this.TimeZoneId == other.TimeZoneId);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals(obj as VTIMEZONE);
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
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VTIMEZONE").AppendLine();
            sb.AppendFormat("TZID={0}", this.TimeZoneId).AppendLine();
            if (!this.StandardTimes.NullOrEmpty()) this.StandardTimes.ForEach(x => sb.Append(x).AppendLine());
            else if (!this.DaylightTimes.NullOrEmpty()) this.DaylightTimes.ForEach(x => sb.Append(x).AppendLine()); 
            sb.Append("END:VTIMEZONE");
            return sb.ToString();
        }
    }

    [DataContract]
    [KnownType(typeof(STANDARD))]
    [KnownType(typeof(DAYLIGHT))]
    public abstract class OBSERVANCE: IOBSERVANCE, IEquatable<OBSERVANCE>, IContainsKey<string>
    {
        public string Id { get; set; }

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

        public OBSERVANCE() 
        {
            this.Comments = new List<COMMENT>();
            this.RecurrenceDates = new List<RDATE>();
            this.TimeZoneNames = new List<TZNAME>();
        }

        public OBSERVANCE(DATE_TIME start, UTC_OFFSET from, UTC_OFFSET to)
        {
            this.Start = start;
            this.TimeZoneOffsetFrom = from;
            this.TimeZoneOffsetTo = to;
        }

        public bool Equals(OBSERVANCE other)
        {
            if (other == null) return false;
            return this.Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
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
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }


    }

    [DataContract]
    public class STANDARD: OBSERVANCE
    {
        public STANDARD(): base() {}

        public STANDARD(DATE_TIME start, UTC_OFFSET from, UTC_OFFSET to): base(start, from , to) { }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:STANDARD").AppendLine();
            sb.AppendFormat("DTSTART:{0}", this.Start).AppendLine();
            sb.AppendFormat("TZOFFSETFROM", this.TimeZoneOffsetFrom).AppendLine();
            sb.AppendFormat("TZOFFSETTO", this.TimeZoneOffsetTo).AppendLine();
            if (this.RecurrenceRule != null) sb.Append(this.RecurrenceRule).AppendLine();
            if (!this.RecurrenceDates.NullOrEmpty()) this.RecurrenceDates.ForEach(x => sb.Append(x).AppendLine());
            if (!this.Comments.NullOrEmpty()) this.Comments.ForEach(x => sb.Append(x).AppendLine());
            if (!this.TimeZoneNames.NullOrEmpty()) this.TimeZoneNames.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:STANDARD");
            return sb.ToString();
        }
    }

    [DataContract]
    public class DAYLIGHT : OBSERVANCE
    {
        public DAYLIGHT() : base() { }

        public DAYLIGHT(DATE_TIME start, UTC_OFFSET from, UTC_OFFSET to) : base(start, from, to) { }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:DAYLIGHT").AppendLine();
            sb.AppendFormat("DTSTART:{0}", this.Start).AppendLine();
            sb.AppendFormat("TZOFFSETFROM", this.TimeZoneOffsetFrom).AppendLine();
            sb.AppendFormat("TZOFFSETTO", this.TimeZoneOffsetTo).AppendLine();
            if (this.RecurrenceRule != null) sb.Append(this.RecurrenceRule).AppendLine();
            if(!this.RecurrenceDates.NullOrEmpty()) this.RecurrenceDates.ForEach(x => sb.Append(x).AppendLine());
            if(!this.Comments.NullOrEmpty())this.Comments.ForEach(x => sb.Append(x).AppendLine());
            if(!this.TimeZoneNames.NullOrEmpty())this.TimeZoneNames.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:DAYLIGHT");
            return sb.ToString();
        }
    }

}
