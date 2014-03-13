using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.domain.models
{

    [KnownType(typeof(TZID))]
    [KnownType(typeof(URI))]
    [KnownType(typeof(DATE_TIME))]
    [DataContract]
    public class VTIMEZONE: ITIMEZONE, IEquatable<VTIMEZONE>, IContainsKey<string>
    {
        public string Id { get { return this.TimeZoneId.ToString() ; } }

        [DataMember]
        public ITZID TimeZoneId { get; set; }

        [DataMember]
        public IURI Url { get; set; }

        [DataMember]
        public IDATE_TIME LastModified { get; set; }

        [DataMember]
        public List<ISTANDARD> StandardTimes { get; set; }

        [DataMember]
        public List<IDAYLIGHT> DaylightSaveTimes { get; set; }

        public VTIMEZONE(ITZID tzid, params ISTANDARD[] standards)
        {
            this.TimeZoneId = tzid;
            if (standards.NullOrEmpty()) throw new ArgumentException("At least one standard time must be provided");
            this.StandardTimes = standards.ToList();
        }

        public VTIMEZONE(ITZID tzid, params IDAYLIGHT[] daylights)
        {
            this.TimeZoneId = tzid;
            if (daylights.NullOrEmpty()) throw new ArgumentException("At least one standard time must be provided");
            this.DaylightSaveTimes = daylights.ToList();
        }

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
            if (!this.StandardTimes.NullOrEmpty()) this.StandardTimes.ForEach(x => sb.Append(x).AppendLine());
            else this.DaylightSaveTimes.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:VTIMEZONE");
            return sb.ToString();
        }
    }


    [KnownType(typeof(UTC_OFFSET))]
    [KnownType(typeof(RECUR))]
    [KnownType(typeof(COMMENT))]
    [KnownType(typeof(RDATE))]
    [KnownType(typeof(TZNAME))]
    [DataContract]
    public class STANDARD: ISTANDARD, IEquatable<STANDARD>, IContainsKey<string>
    {
        public string Id { get; set; }

        [DataMember]
        public IDATE_TIME StartDate { get; set; }

        [DataMember]
        public IUTC_OFFSET TimeZoneOffsetFrom { get; set; }

        [DataMember]
        public IUTC_OFFSET TimeZoneOffsetTo { get; set; }

        [DataMember]
        public IRECUR RecurrenceRule { get; set; }

        [DataMember]
        public List<ITEXT> Comments { get; set; }

        [DataMember]
        public List<IRDATE> RecurrenceDates { get; set; }

        [DataMember]
        public List<ITZNAME> Names { get; set; }

        public STANDARD(IDATE_TIME sdate, IUTC_OFFSET tzofrom, IUTC_OFFSET tzoto)
        {
            this.StartDate = sdate;
            this.TimeZoneOffsetFrom = tzofrom;
            this.TimeZoneOffsetTo = tzoto;
            this.RecurrenceDates = new List<IRDATE>();
            this.Comments = new List<ITEXT>();
            this.Names = new List<ITZNAME>();
        }

        public bool Equals(STANDARD other)
        {
            if (other == null) return false;
            return this.StartDate == other.StartDate &&
                (this.TimeZoneOffsetFrom) == (other.TimeZoneOffsetFrom) &&
                (this.TimeZoneOffsetTo) == (other.TimeZoneOffsetTo) &&
                (this.RecurrenceRule) == (other.RecurrenceRule ) &&
                this.Comments.OfType<COMMENT>().AreDuplicatesOf(other.Comments.OfType<COMMENT>()) &&
                this.RecurrenceDates.OfType<RDATE>().AreDuplicatesOf(other.RecurrenceDates.OfType<RDATE>()) &&
                this.Names.OfType<TZNAME>().AreDuplicatesOf(other.Names.OfType<TZNAME>());
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as STANDARD);
        }

        public override int GetHashCode()
        {
            return this.StartDate.GetHashCode()^ 
                this.TimeZoneOffsetFrom.GetHashCode() ^ 
                this.TimeZoneOffsetTo.GetHashCode();
        }

        public static bool operator ==(STANDARD a, STANDARD b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(STANDARD a, STANDARD b)
        {
            if (a == null || b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:STANDARD").AppendLine();
            sb.Append(this.StartDate).AppendLine();
            sb.AppendFormat("TZOFFSETFROM", this.TimeZoneOffsetFrom).AppendLine();
            sb.AppendFormat("TZOFFSETTO", this.TimeZoneOffsetTo).AppendLine();
            if(this.RecurrenceRule != null) sb.Append(this.RecurrenceRule).AppendLine();
            this.RecurrenceDates.ForEach(x => sb.Append(x).AppendLine());
            this.Comments.ForEach(x => sb.Append(x).AppendLine());
            this.Names.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:STANDARD");
            return sb.ToString();
        }


    }


    [KnownType(typeof(UTC_OFFSET))]
    [KnownType(typeof(RECUR))]
    [KnownType(typeof(COMMENT))]
    [KnownType(typeof(RDATE))]
    [KnownType(typeof(TZNAME))]
    [DataContract]
    public class DAYLIGHT : IDAYLIGHT, IEquatable<DAYLIGHT>, IContainsKey<string>
    {
        public string Id { get; set; }

        [DataMember]
        public IDATE_TIME StartDate { get; set; }

        [DataMember]
        public IUTC_OFFSET TimeZoneOffsetFrom { get; set; }

        [DataMember]
        public IUTC_OFFSET TimeZoneOffsetTo { get; set; }

        [DataMember]
        public IRECUR RecurrenceRule { get; set; }

        [DataMember]
        public List<ITEXT> Comments { get; set; }

        [DataMember]
        public List<IRDATE> RecurrenceDates { get; set; }

        [DataMember]
        public List<ITZNAME> Names { get; set; }

        public DAYLIGHT(IDATE_TIME sdate, IUTC_OFFSET tzofrom, IUTC_OFFSET tzoto)
        {
            this.StartDate = sdate;
            this.TimeZoneOffsetFrom = tzofrom;
            this.TimeZoneOffsetTo = tzoto;
            this.RecurrenceDates = new List<IRDATE>();
            this.Comments = new List<ITEXT>();
            this.Names = new List<ITZNAME>();
        }

        public bool Equals(DAYLIGHT other)
        {
            if (other == null) return false;
            return this.StartDate == other.StartDate &&
                (this.TimeZoneOffsetFrom) == (other.TimeZoneOffsetFrom ) &&
                (this.TimeZoneOffsetTo ) == (other.TimeZoneOffsetTo) &&
                (this.RecurrenceRule) == (other.RecurrenceRule) &&
                this.Comments.OfType<COMMENT>().AreDuplicatesOf(other.Comments.OfType<COMMENT>()) &&
                this.RecurrenceDates.OfType<RDATE>().AreDuplicatesOf(other.RecurrenceDates.OfType<RDATE>()) &&
                this.Names.OfType<TZNAME>().AreDuplicatesOf(other.Names.OfType<TZNAME>());
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as DAYLIGHT);
        }

        public override int GetHashCode()
        {
            return this.StartDate.GetHashCode() ^
                this.TimeZoneOffsetFrom.GetHashCode() ^
                this.TimeZoneOffsetTo.GetHashCode();
        }

        public static bool operator ==(DAYLIGHT a, DAYLIGHT b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(DAYLIGHT a, DAYLIGHT b)
        {
            if (a == null || b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:DAYLIGHT").AppendLine();
            sb.Append(this.StartDate).AppendLine();
            sb.AppendFormat("TZOFFSETFROM:{0}", this.TimeZoneOffsetFrom).AppendLine();
            sb.AppendFormat("TZOFFSETTO:{0}", this.TimeZoneOffsetTo).AppendLine();
            if (this.RecurrenceRule != null) sb.AppendFormat("RRULE:{0}",this.RecurrenceRule).AppendLine();
            this.RecurrenceDates.ForEach(x => sb.Append(x).AppendLine());
            this.Comments.ForEach(x => sb.Append(x).AppendLine());
            this.Names.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:DAYLIGHT");
            return sb.ToString();
        }
    }

}
