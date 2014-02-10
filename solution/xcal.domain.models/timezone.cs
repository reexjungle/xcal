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

    [KnownType(typeof(TZIDPROP))]
    [KnownType(typeof(TZURL))]
    [KnownType(typeof(DATE_TIME))]
    [DataContract]
    public class VTIMEZONE: ITIMEZONE, IEquatable<VTIMEZONE>, IContainsId<string>
    {
        public string Id { get { return this.TimeZoneIdProperty.Value; } }

        [DataMember]
        public ITZIDPROP TimeZoneIdProperty { get; set; }

        [DataMember]
        public ITZURL Url { get; set; }

        [DataMember]
        public IDATE_TIME LastModified { get; set; }

        [DataMember]
        public List<ISTANDARD> StandardTimes { get; set; }

        [DataMember]
        public List<IDAYLIGHT> DaylightSaveTimes { get; set; }

        public VTIMEZONE(ITZIDPROP tzidprop, params ISTANDARD[] sts)
        {
            this.TimeZoneIdProperty = tzidprop;
            if (sts.NullOrEmpty()) throw new ArgumentException("At least one standard time must be provided");
            this.StandardTimes = sts.ToList();
        }

        public VTIMEZONE(ITZIDPROP tzidprop, params IDAYLIGHT[] dsts)
        {
            this.TimeZoneIdProperty = tzidprop;
            if (dsts.NullOrEmpty()) throw new ArgumentException("At least one standard time must be provided");
            this.DaylightSaveTimes = dsts.ToList();
        }

        public bool Equals(VTIMEZONE other)
        {
            if (other == null) return false;
            return (this.TimeZoneIdProperty.Value
                .Equals(other.TimeZoneIdProperty.Value, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as VTIMEZONE);
        }

        public override int GetHashCode()
        {
            return this.TimeZoneIdProperty.GetHashCode();
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
            sb.Append(this.TimeZoneIdProperty);
            if (!this.StandardTimes.NullOrEmpty()) this.StandardTimes.ForEach(x => sb.Append(x).AppendLine());
            else this.DaylightSaveTimes.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:VTIMEZONE");
            return sb.ToString();
        }
    }


    [KnownType(typeof(TZOFFSETFROM))]
    [KnownType(typeof(TZOFFSETTO))]
    [KnownType(typeof(RECUR))]
    [KnownType(typeof(COMMENT))]
    [KnownType(typeof(RDATE))]
    [KnownType(typeof(TZNAME))]
    [DataContract]
    public class STANDARD: ISTANDARD, IEquatable<STANDARD>
    {

        [DataMember]
        public IDATE_TIME StartDate { get; set; }

        [DataMember]
        public ITZOFFSETFROM TimeZoneOffsetFrom { get; set; }

        [DataMember]
        public ITZOFFSETTO TimeZoneOffsetTo { get; set; }

        [DataMember]
        public IRECUR RecurrenceRule { get; set; }

        [DataMember]
        public List<ITEXT> Comments { get; set; }

        [DataMember]
        public List<IRDATE> RecurrenceDates { get; set; }

        [DataMember]
        public List<ITZNAME> Names { get; set; }

        public STANDARD(DATE_TIME sdate, ITZOFFSETFROM tzofrom, ITZOFFSETTO tzoto)
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
            sb.Append(this.TimeZoneOffsetFrom).AppendLine();
            sb.Append(this.TimeZoneOffsetTo).AppendLine();
            if(this.RecurrenceRule != null) sb.Append(this.RecurrenceRule).AppendLine();
            this.RecurrenceDates.ForEach(x => sb.Append(x).AppendLine());
            this.Comments.ForEach(x => sb.Append(x).AppendLine());
            this.Names.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:STANDARD");
            return sb.ToString();
        }
    }


    [KnownType(typeof(TZOFFSETFROM))]
    [KnownType(typeof(TZOFFSETTO))]
    [KnownType(typeof(RECUR))]
    [KnownType(typeof(COMMENT))]
    [KnownType(typeof(RDATE))]
    [KnownType(typeof(TZNAME))]
    [DataContract]
    public class DAYLIGHT : IDAYLIGHT, IEquatable<DAYLIGHT>
    {

        [DataMember]
        public IDATE_TIME StartDate { get; set; }

        [DataMember]
        public ITZOFFSETFROM TimeZoneOffsetFrom { get; set; }

        [DataMember]
        public ITZOFFSETTO TimeZoneOffsetTo { get; set; }

        [DataMember]
        public IRECUR RecurrenceRule { get; set; }

        [DataMember]
        public List<ITEXT> Comments { get; set; }

        [DataMember]
        public List<IRDATE> RecurrenceDates { get; set; }

        [DataMember]
        public List<ITZNAME> Names { get; set; }

        public DAYLIGHT(DATE_TIME sdate, ITZOFFSETFROM tzofrom, ITZOFFSETTO tzoto)
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
            sb.Append(this.TimeZoneOffsetFrom).AppendLine();
            sb.Append(this.TimeZoneOffsetTo).AppendLine();
            if (this.RecurrenceRule != null) sb.Append(this.RecurrenceRule).AppendLine();
            this.RecurrenceDates.ForEach(x => sb.Append(x).AppendLine());
            this.Comments.ForEach(x => sb.Append(x).AppendLine());
            this.Names.ForEach(x => sb.Append(x).AppendLine());
            sb.Append("END:DAYLIGHT");
            return sb.ToString();
        }
    }

}
