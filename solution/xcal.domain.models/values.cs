using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.extensions;

namespace reexmonkey.xcal.domain.models
{

    /// <summary>
    /// Provides a model for identifying properties that contain a character encoding of inline binary data.
    /// The character encoding is based on the Base64 encoding
    /// </summary>
    [DataContract]
    public class BINARY : IBINARY, IEquatable<BINARY>
    {
        private ENCODING encoding = ENCODING.BASE64;

        /// <summary>
        /// Gets or sets the value of this type 
        /// </summary>
        [DataMember]
        public string Value { get; set; }
   
        /// <summary>
        /// Gets or sets the encoding used for this type
        /// </summary>
        [DataMember]
        public ENCODING Encoding
        {
            get { return this.encoding; }
            set
            {
                this.encoding = value;
                if (!string.IsNullOrEmpty(this.Value))
                {
                    if(this.encoding == ENCODING.BASE64) this.Value = this.Value.EncodeToBase64();
                    else if(this.encoding == ENCODING.BIT8) this.Value = this.Value.EncodeToUtf8();
                };
            }

        }

        public bool IsDefault()
        {
             return this.Value.Equals(string.Empty);
        }

        /// <summary>
        /// Overloaded Constructor
        /// </summary>
        /// <param name="value"></param>
        public BINARY(string value, ENCODING encoding = ENCODING.BASE64)
        {
            this.Value = value;
            this.Encoding = encoding;
        }

        public bool Equals(BINARY other)
        {
            if (other == null) return false;
            return this.Encoding == other.Encoding && this.Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = obj as BINARY;
            return (other != null) ? this.Equals(other) : false;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode() ^ this.encoding.GetHashCode();
        }

        public static bool operator ==(BINARY x, BINARY y)
        {
            if((object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(BINARY a, BINARY b)
        {
            if (a == null || b == null) return !object.Equals(a, b);
            return !(a.Equals(b));
        }

        public override string ToString()
        {
            return  this.Value ?? string.Empty;
        }


    }

    /// <summary>
    /// Specifies the contract for identifying properties that contain a calendar date.
    /// Format: [YYYYMMDD] where YYYY is 4-digit year, MM is 2-digit month and DD is 2-digit day
    /// </summary>
    [DataContract]
    public class DATE : IDATE, IEquatable<DATE>, IComparable<DATE> 
    {
        private uint fullyear;
        private uint month;
        private uint mday;

        /// <summary>
        /// Gets or sets the 4-digit representation of a full year e.g. 2013 
        /// </summary>
        [DataMember]
        public uint FULLYEAR
        {
            get { return fullyear; }
            set { this.fullyear = value; }
        }

        /// <summary>
        /// Gets or sets the 2-digit representation of a month
        /// </summary>
        [DataMember]
        public uint MONTH
        {
            get { return month; }
            set { this.month = value; }
        }

        /// <summary>
        /// Gets or sets the 2-digit representation of a month-day
        /// </summary>
        [DataMember]
        public uint MDAY
        {
            get { return mday; }
            set { this.mday = value; }
        }

        public bool IsDefault()
        {
            return (this.MDAY == 1 && this.MONTH == 1 && this.FULLYEAR == 1);
        }

        public DATE()
        {
            this.FULLYEAR = 1;
            this.MONTH = 1;
            this.MDAY = 1;
        }

        public DATE(uint fullyear, uint month, uint mday)
        {
            this.FULLYEAR = fullyear;
            this.MONTH = month;
            this.MDAY = mday;
        }

        public DATE(IDATE_TIME datetime)
        {
            this.FULLYEAR = datetime.FULLYEAR;
            this.MONTH = datetime.MONTH;
            this.MDAY = datetime.MDAY;
        }

        public DATE(DateTime datetime)
        {
            this.FULLYEAR = (uint)datetime.Year;
            this.MONTH = (uint)datetime.Month;
            this.MDAY = (uint)datetime.Day;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0:D4}{1:D2}{2:D2}", this.fullyear, this.month, this.mday);  //YYYYMMDD
            return sb.ToString();
        }

        public bool Equals(DATE other)
        {
            if (other == null) return false;
            return (this.fullyear == other.FULLYEAR) && 
                (this.month == other.MONTH) && 
                (this.mday == other.MDAY);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = (DATE)obj;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.fullyear.GetHashCode() ^ this.month.GetHashCode() ^ this.mday.GetHashCode();
        }

        public int CompareTo(DATE other)
        {
            if (other == null) return -2; //undefined
            if (this.fullyear < other.FULLYEAR) return -1;
            else if (this.fullyear > other.FULLYEAR) return 1;
            else
            {
                if (this.month < other.MONTH) return -1;
                else if (this.month > other.MONTH) return 1;
                else
                {
                    if (this.mday < other.MDAY) return -1;
                    else if (this.mday > other.MDAY) return 1;
                    else return 0;
                }
            }
        }

        public static bool operator ==(DATE a, DATE b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(DATE a, DATE b)
        {
            if (a == null || b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public static bool operator <(DATE a, DATE b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator <=(DATE a, DATE b)
        {
            if (a == null || b == null) return false;
            return (a.CompareTo(b) == -1 || a.CompareTo(b) == 0);
        }

        public static bool operator > (DATE a, DATE b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator >= (DATE a, DATE b)
        {
            if (a == null || b == null) return false;
            return (a.CompareTo(b) == 1 || a.CompareTo(b) == 0);
        }

        public static DURATION operator -(DATE start, DATE end)
        {
            var duration = new DURATION();
            var x = start; var y = start;

            if (start == end) return duration;

            if (end < start)
            {
                x = end;
                y = end;
                duration.Sign = SignType.Negative;
            }

            if (x.MDAY < y.MDAY)
            {
                duration.DAYS = (start.MONTH.CountDays(start.FULLYEAR) + start.MDAY) - end.MDAY; // borrow a '1' (days base) from month and add to a.MDAY
                if (start.MONTH > 1u) start.MONTH--;
            }
            else duration.DAYS = x.MDAY - y.MDAY;

            if (x.MONTH < y.MONTH)
            {
                duration.WEEKS = 4 * ((x.MONTH + 12u) - y.MONTH); //borrow a '1'  (12 months) from year and add to month
                if (x.FULLYEAR > 1u) x.FULLYEAR--;
            }
            else duration.WEEKS = 4 * (x.MONTH - y.MONTH);

            if (x.FULLYEAR > 0u && x.FULLYEAR >= y.FULLYEAR) duration.WEEKS += 52 * (x.FULLYEAR - y.FULLYEAR);

            return duration;
        }

        public static DATE operator +(DATE start, DURATION duration)
        {
            if (duration.Sign == SignType.Negative)
            {
                duration.Sign = SignType.Positive;
                return start - duration;
            }
            var fdate = start.ToDateTime().Add(duration.ToTimeSpan());
            return fdate.To_IDATE<DATE>();
        }

        public static DATE operator -(DATE end, DURATION duration)
        {
            if (duration.Sign == SignType.Negative) return end + duration;
            var fdate = end.ToDateTime().Subtract(duration.ToTimeSpan());
            return fdate.To_IDATE<DATE>();
        }
    }

    [DataContract]
    public class DATE_TIME : IDATE_TIME, IEquatable<DATE_TIME>, IComparable<DATE_TIME>
    {
        private uint hour;
        private uint minute;
        private uint second;
        private uint fullyear;
        private uint month;
        private uint mday;
        private TimeFormat tformat;
        private ITZID tzid;
        private IUTC_OFFSET utc_offset;

        /// <summary>
        /// Gets or sets the 4-digit representation of a full year e.g. 2013 
        /// </summary>
        [DataMember]
        public uint FULLYEAR
        {
            get { return fullyear; }
            set { this.fullyear = value; }
        }

        /// <summary>
        /// Gets or sets the 2-digit representation of a month
        /// </summary>
        [DataMember]
        public uint MONTH
        {
            get { return month; }
            set
            {
                this.month = value;
            }
        }

        /// <summary>
        /// Gets or sets the 2-digit representation of a month-day
        /// </summary>
        [DataMember]
        public uint MDAY
        {
            get { return mday; }
            set
            {
                 this.mday = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the hours
        /// </summary>
        [DataMember]
        public uint HOUR
        {
            get { return this.hour; }
            set
            {
                this.hour = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the minutes
        /// </summary>
        [DataMember]
        public uint MINUTE
        {
            get { return this.minute; }
            set
            {
               this.minute = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;60 </exception>
        [DataMember]
        public uint SECOND
        {
            get { return this.second; }
            set
            {
                this.second = value;
            }
        }

        [DataMember]
        public TimeFormat TimeFormat
        {
            get { return this.tformat; }
            set  { this.tformat = value; }
        }

        [DataMember]
        public ITZID TimeZoneId
        {
            get { return this.tzid; }
            set {  this.tzid = value; }
        }

        [DataMember]
        public IUTC_OFFSET Utc_Offset
        {
            get { return this.utc_offset; }
            set
            {
                this.utc_offset = value;
            }
        }

        public bool IsDefault()
        {
            return (this.MDAY == 1 && this.MONTH == 1 && this.FULLYEAR == 1 && this.SECOND == 0 && this.MINUTE == 0 && this.HOUR == 0);
        }

        public DATE_TIME()
        {
            this.FULLYEAR = 1u;
            this.MONTH = 1u;
            this.MDAY = 1u;
            this.HOUR = 0u;
            this.MINUTE = 0u;
            this.SECOND = 0u;
            this.tformat = TimeFormat.Local;
            this.tzid = null;
        }

        public DATE_TIME(uint fullyear, uint month, uint mday, uint hour, uint minute, uint second, 
            TimeFormat tformat = TimeFormat.Local, ITZID tzid = null, ValueFormat vformat = contracts.ValueFormat.UNKNOWN)
        {
            this.FULLYEAR = fullyear;
            this.MONTH = month;
            this.MDAY = mday;
            this.HOUR = hour;
            this.MINUTE = minute;
            this.SECOND = second;
            this.tformat = tformat;
            this.tzid = tzid;
        }

        public DATE_TIME(IDATE_TIME source)
        {
            this.FULLYEAR = source.FULLYEAR;
            this.MONTH = source.MONTH;
            this.MDAY = source.MDAY;
            this.HOUR = source.HOUR;
            this.MINUTE = source.MINUTE;
            this.SECOND = source.SECOND;
            this.tformat = source.TimeFormat;
            this.tzid = source.TimeZoneId;
        }

        public DATE_TIME(DateTime datetime, TimeZoneInfo tzinfo = null)
        {
            this.FULLYEAR = (uint)datetime.Year;
            this.MONTH = (uint)datetime.Month;
            this.MDAY = (uint)datetime.Day;
            this.HOUR = (uint)datetime.Hour;
            this.MINUTE = (uint)datetime.Minute;
            this.SECOND = (uint)datetime.Second;
            if (tzinfo != null)
            {
                this.TimeZoneId = new TZID(null, tzinfo.Id);
                this.TimeFormat = TimeFormat.LocalAndTimeZone;
            }
            else if (datetime.Kind == DateTimeKind.Utc) this.TimeFormat = TimeFormat.Utc;
        }

        public DATE_TIME(DateTimeOffset datetime)
        {
            this.FULLYEAR = (uint)datetime.Year;
            this.MONTH = (uint)datetime.Month;
            this.MDAY = (uint)datetime.Day;
            this.HOUR = (uint)datetime.Hour;
            this.MINUTE = (uint)datetime.Minute;
            this.SECOND = (uint)datetime.Second;
            this.Utc_Offset = new UTC_OFFSET(datetime.Offset.Hours, datetime.Offset.Minutes, datetime.Offset.Seconds);
            this.TimeFormat = TimeFormat.Utc;
        }

        public DATE_TIME(DATE date, TZID tzid = null)
        {
            if (date == null) throw new ArgumentNullException("Date value must be non-null");
            this.FULLYEAR = date.FULLYEAR;
            this.MONTH = date.MONTH;
            this.MDAY = date.MDAY;
            this.TimeZoneId = tzid;
        }

        public DATE_TIME(TIME time)
        {
            if (time == null) throw new ArgumentNullException("Date time value must be non-null");
            this.HOUR = time.HOUR;
            this.MINUTE = time.MINUTE;
            this.SECOND = time.SECOND;
            this.TimeZoneId = time.TimeZoneId;
        }   
     
        public override string ToString()
        {
            if (this.tformat == TimeFormat.Local) return string.Format("{0:D4}{1:D2}{2:D2}T{3:D2}{4:D2}{5:D2}", this.fullyear, this.month, this.mday, this.hour, this.minute, this.second);

            else if (this.tformat == TimeFormat.Utc) return string.Format("0:D4}{1:D2}{2:D2}T{3:D2}{4:D2}{5:D2}Z", this.fullyear, this.month, this.mday, this.hour, this.minute, this.second);
            else return string.Format("{0}:{1:D4}{2:D2}{3:D2}T{4:D2}{5:D2}{6:D2}", this.fullyear, this.month, this.mday, this.tzid, this.hour, this.minute, this.second); 
        }

        public bool Equals(DATE_TIME other)
        {
            if (other == null) return false;
            return (this.fullyear == other.FULLYEAR) && (this.month == other.MONTH) && (this.mday == other.MDAY) &&
                (this.hour == other.HOUR) && (this.minute == other.MINUTE) && (this.second == other.SECOND);

        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other =  (DATE_TIME)obj;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return
                this.fullyear.GetHashCode() ^
                this.month.GetHashCode() ^
                this.mday.GetHashCode() ^
                this.hour.GetHashCode() ^
                this.minute.GetHashCode() ^
                this.second.GetHashCode();
        }

        public int CompareTo(DATE_TIME other)
        {
            if (other == null) return -2; //undefined
            if (this.fullyear < other.FULLYEAR) return -1;
            else if (this.fullyear > other.FULLYEAR) return 1;
            else
            {
                if (this.month < other.MONTH) return -1;
                else if (this.month > other.MONTH) return 1;
                else
                {
                    if (this.mday < other.MDAY) return -1;
                    else if (this.mday > other.MDAY) return 1;
                    else
                    {
                        if (this.hour < other.hour) return -1;
                        else if (this.hour > other.hour) return 1;
                        else
                        {
                            if (this.minute < other.minute) return -1;
                            else if (this.minute > other.minute) return 1;
                            else
                            {
                                if (this.second < other.second) return -1;
                                else if (this.second > other.second) return 1;
                                else return 0;
                            }
                        
                        }
                    }
                }
            }      
        }

        #region overloaded operators

        public static DATE_TIME operator +(DATE_TIME start, DURATION duration)
        {
            if (duration.Sign == SignType.Negative)
            {
                duration.Sign = SignType.Positive;
                return start - duration;
            }
            if (duration.IsDefault()) return start;
            var fdate = start.ToDateTime().Add(duration.ToTimeSpan());
            return fdate.To_IDATE_TIME<DATE_TIME>();
        }

        public static DATE_TIME operator -(DATE_TIME end, DURATION duration)
        {
            if (duration.Sign == SignType.Negative) return end + duration;
            if (duration.IsDefault()) return end;

            var fdate = end.ToDateTime().Subtract(duration.ToTimeSpan());
            return fdate.To_IDATE_TIME<DATE_TIME>();
        }

        public static DURATION operator -(DATE_TIME start, DATE_TIME end)
        {
            var x = start;
            var y = end;

            var duration = new DURATION();
            if (start == end) return duration;

            if (start < end) 
            { 
                x = start; y = end;
                duration.Sign = SignType.Negative; 
            }

            if (x.SECOND < y.SECOND)
            {
                duration.SECONDS = (x.SECOND + 60u) - y.SECOND;
                if (x.MINUTE > 0u) x.MINUTE--;
            }
            else duration.SECONDS = x.SECOND - y.SECOND;

            if (x.MINUTE < y.MINUTE)
            {
                duration.MINUTES = (x.MINUTE + 60u) - y.MINUTE;
                if (x.MINUTE > 0u) x.HOUR--;
            }
            else duration.MINUTES = x.MINUTE - y.MINUTE;

            if (x.HOUR < y.HOUR)
            {
                duration.HOURS = (x.HOUR + 24u) - y.HOUR;
                if (x.MDAY > 1u) x.MDAY--;
            }
            else duration.HOURS = x.HOUR - y.HOUR;

            if (x.MDAY < y.MDAY)
            {
                duration.DAYS = (x.MONTH.CountDays(x.FULLYEAR) + x.MDAY) - y.MDAY; // borrow a '1' (days base) from month and add to a.MDAY
                if (x.MONTH > 1u) x.MONTH--;
            }
            else duration.DAYS = x.MDAY - y.MDAY;

            if (x.MONTH < y.MONTH)
            {
                duration.WEEKS = 4 * ((x.MONTH + 12u) - y.MONTH); //borrow a '1'  (12 months) from year and add to month
                if (x.FULLYEAR > 1u) x.FULLYEAR--;
            }
            else duration.WEEKS = 4 * (x.MONTH - y.MONTH);

            if (x.FULLYEAR > 0u && x.FULLYEAR >= y.FULLYEAR) duration.WEEKS += 52 * (x.FULLYEAR - y.FULLYEAR);

            return duration;
        }

        public static bool operator <(DATE_TIME a, DATE_TIME b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator >(DATE_TIME a, DATE_TIME b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator <=(DATE_TIME a, DATE_TIME b)
        {
            if (a == null || b == null) return false;
            return (a.CompareTo(b) == -1 || a.CompareTo(b) == 0);
        }

        public static bool operator >=(DATE_TIME a, DATE_TIME b)
        {
            if (a == null || b == null) return false;
            return (a.CompareTo(b) == 1 || a.CompareTo(b) == 0);
        }

        public static bool operator ==(DATE_TIME a, DATE_TIME b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(DATE_TIME a, DATE_TIME b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        #endregion
    }

    [DataContract]
    public class TIME : ITIME, IEquatable<TIME>, IComparable<TIME>
    {
        private uint hour;
        private uint minute;
        private uint second;
        private TimeFormat format;
        private ITZID tzid;
        private IUTC_OFFSET utc_offset;

        /// <summary>
        /// Gets or sets the value of the hours
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;23 </exception>
        [DataMember]
        public uint HOUR
        {
            get { return this.hour; }
            set
            {
                if (value >= 0 && value < 24) this.hour = value;
                else throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets or sets the value of the minutes
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;59 </exception>
        [DataMember]
        public uint MINUTE
        {
            get { return this.minute; }
            set
            {
                if (value >= 0 && value < 59) this.minute = value;
                else throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets or sets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;60 </exception>
        [DataMember]
        public uint SECOND
        {
            get { return this.second; }
            set
            {
                //Note the exceptional case of iCalendar specification that allows for the &quot;6oth&quot; positive leap second.
                if (value >= 0 && value < 61) this.second = value;
                else throw new ArgumentOutOfRangeException();
            }
        }

        [DataMember]
        public TimeFormat TimeFormat
        {
            get { return this.format; }
            set
            {
                if (this.tzid == null && value == TimeFormat.LocalAndTimeZone) throw new ArgumentException("A Time Zone Reference is required for this date time format");
                if (this.tzid != null && value == TimeFormat.Utc) throw new ArgumentException("Time Zone References MUST NOT be applied to UTC times!");
                this.format = value;
            }
        }

        [DataMember]
        public ITZID TimeZoneId
        {
            get { return this.tzid; }
            set
            {
                if (this.format == TimeFormat.LocalAndTimeZone && value == null) throw new ArgumentException("Null Time Zone References are not allowed for the Local and Time Zone format");
                if (this.format == TimeFormat.Utc) throw new ArgumentException("Time Zone References MUST NOT be applied to UTC times!");
                this.tzid = value;
            }
        }

        [DataMember]
        public IUTC_OFFSET Utc_Offset
        {
            get { return this.utc_offset; }
            set
            {
                if (this.format == TimeFormat.Utc) this.utc_offset = value;
                else throw new ArgumentException("UTC Offset value is only allowed for the Utc format");
            }
        }

        public bool IsDefault()
        {
            return (this.SECOND == 0 && this.MINUTE == 0 && this.HOUR == 0 && this.tzid == null && this.format == TimeFormat.Local);
        }

        public TIME()
        {
            this.HOUR = 0u;
            this.MINUTE = 0u;
            this.SECOND = 0u;
            this.TimeFormat = TimeFormat.Local;
        }

        public TIME(uint hour, uint minute, uint second, TimeFormat format = TimeFormat.Local, ITZID tzid = null)
        {
            this.HOUR = hour;
            this.MINUTE = minute;
            this.SECOND = second;
            this.format = format;
            this.tzid = tzid;
        }

        public TIME(DateTime datetime, TimeZoneInfo tzinfo)
        {
            this.HOUR = (uint)datetime.Hour;
            this.MINUTE = (uint)datetime.Minute;
            this.SECOND = (uint)datetime.Second;
            if (tzinfo != null)
            {
                this.TimeZoneId = new TZID(null, tzinfo.Id);
                this.TimeFormat = TimeFormat.LocalAndTimeZone;
            }
            else if (datetime.Kind == DateTimeKind.Utc) this.TimeFormat = TimeFormat.Utc;
        }

        public TIME(DateTimeOffset datetime)
        {
            this.HOUR = (uint)datetime.Hour;
            this.MINUTE = (uint)datetime.Minute;
            this.SECOND = (uint)datetime.Second;
            this.Utc_Offset = new UTC_OFFSET(datetime.Offset.Hours, datetime.Offset.Minutes, datetime.Offset.Seconds);
            this.TimeFormat = TimeFormat.Utc;
        }

        public override string ToString()
        {
            if (this.format == TimeFormat.Local) return string.Format("T{0:D2}{1:D2}{2:D2}", this.hour, this.minute, this.second);
            else if (this.format == TimeFormat.Utc) return string.Format("T{0:D2}{1:D2}{2:D2}Z", this.hour, this.minute, this.second);
            else return string.Format("{0}:T{1:D2}{2:D2}{3:D2}Z", this.tzid, this.hour, this.minute, this.second);
        }

        public bool Equals(TIME other)
        {
            if (other == null) return false;
            return (this.hour == other.HOUR) && (this.minute == other.MINUTE) && (this.second == other.SECOND);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = (TIME)obj;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.hour.GetHashCode() ^ this.minute.GetHashCode() ^ this.second.GetHashCode();
        }

        public int CompareTo(TIME other)
        {
            if (other == null) return 3;
            if (this.hour < other.HOUR && this.minute < other.MINUTE && this.second < other.SECOND) return -1;
            else if (this.hour <= other.HOUR && this.minute <= other.MINUTE && this.second <= other.SECOND) return -2;
            else if (this.Equals(other)) return 0;
            else if (this.hour > other.HOUR && this.minute > other.MINUTE && this.second > other.SECOND) return 1;
            else return 2; //>=
        }

        #region overloaded operators

        public static TIME operator +(TIME start, DURATION duration)
        {
            if (duration.Sign == SignType.Negative)
            {
                duration.Sign = SignType.Positive;
                return start - duration;
            }

            var end = start.ToTimeSpan().Add(duration.ToTimeSpan());
            return end.ToTime<TIME>();
        }

        public static TIME operator -(TIME end, DURATION duration)
        {
            if (duration.Sign == SignType.Negative) return end + duration;
            if (end.HOUR < duration.HOURS) throw new ArgumentException("Time hour MUST be bigger or equal to duration hour!");

            var start = end.ToTimeSpan().Add(duration.ToTimeSpan());
            return start.ToTime<TIME>();
        }

        public static DURATION operator -(TIME start, TIME end)
        {
            var duration = new DURATION();
            var x = start; var y = end;

            if (start == end) return duration;

            //check if end is earlier than start => reverse duration sign
            if (end < start)
            {
                x = end;
                y = start;
                duration.Sign = SignType.Negative;
            }

            if (x.SECOND < y.SECOND)
            {
                duration.SECONDS = (x.SECOND + 60u) - y.SECOND;
                if (y.MINUTE > 0u) x.MINUTE--;
            }
            else duration.SECONDS = x.SECOND - y.SECOND;

            if (start.MINUTE < end.MINUTE)
            {
                duration.MINUTES = (x.MINUTE + 60u) - y.MINUTE;
                if (x.MINUTE > 0u) x.HOUR--;
            }
            else duration.MINUTES = x.MINUTE - y.MINUTE;

            if (x.HOUR >= y.HOUR) duration.HOURS = x.HOUR - y.HOUR;
            return duration;
        }

        public static bool operator <(TIME a, TIME b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator >(TIME a, TIME b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator <=(TIME a, TIME b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == -2;
        }

        public static bool operator >=(TIME a, TIME b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 2;
        }

        public static bool operator ==(TIME a, TIME b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(TIME a, TIME b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        #endregion

    }

    [DataContract]
    public class DURATION : IDURATION, IEquatable<DURATION>, IComparable<DURATION>
    {
        private uint weeks, days, hours, minutes, seconds;
        private SignType sign;

        [DataMember]
        public uint WEEKS
        {
            get { return weeks; }
            set { this.weeks = value; }
        }

        [DataMember]
        public uint HOURS 
        {
            get { return this.hours; }
            set { this.hours = value; }
        }

        [DataMember]
        public uint MINUTES
        {
            get { return this.minutes; }
            set { this.minutes = value;}
        }

        [DataMember]
        public uint SECONDS
        {
            get { return this.seconds; }
            set { this.seconds = value; }
        }

        [DataMember]
        public uint DAYS
        {
            get { return this.days; }
            set { this.days = value; }
        }

        [DataMember]
        public SignType Sign
        {
            get { return this.sign; }
            set { this.sign = value; }
        }

        public DURATION()
        {
            this.weeks = 0;
            this.days = 0;
            this.hours = 0;
            this.minutes = 0;
            this.seconds = 0;
            this.sign = SignType.Neutral;
        }

        public DURATION(uint weeks, uint days = 0, uint hours = 0, uint minutes = 0, uint seconds = 0, SignType sign = SignType.Positive)
        {
            this.weeks = weeks;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.sign = sign;
        }

        public DURATION(TimeSpan span)
        {
            this.days = (uint) span.Days;
            this.hours = (uint) span.Hours;
            this.minutes = (uint)span.Minutes;
            this.seconds = (uint)span.Seconds;
            this.weeks = (uint)(span.TotalDays - (span.Days + (span.Hours / 24) + (span.Minutes /( 24*60)) + (span.Seconds /(24* 3600) ) + (span.Milliseconds /(24* 3600000))))/7u;
            var sum = span.Days + (span.Hours / 24) + (span.Minutes / (24 * 60)) + (span.Seconds / (24 * 3600)) + (span.Milliseconds / (24 * 3600000));
            this.sign = (sum >= 0) ? SignType.Positive : SignType.Negative;
        }

        public bool IsDefault()
        {
            return (this.weeks == 0 && this.hours == 0 && this.minutes == 0 && this.seconds == 0 && this.sign == SignType.Positive); 
        }

        public override string ToString()
        {
            var sym = (this.sign == SignType.Negative)? "-": string.Empty;
            if (this.weeks != 0) return string.Format("{0}P{1}W", sym, this.weeks);
            else
            {
                var sb = new StringBuilder();
                sb.Append(sym);
                if (this.days != 0) sb.AppendFormat("P{0}D", this.days);
                if (this.hours != 0 || this.minutes != 0 && this.seconds != 0) 
                {
                    sb.Append("T");
                    if (this.hours != 0) sb.AppendFormat("{0:D2}H", this.hours);
                    if (this.minutes != 0) sb.AppendFormat("{0:D2}M", this.minutes);
                    if (this.seconds != 0) sb.AppendFormat("{0:D}S", this.seconds);                
                }
                return sb.ToString();
            }               
        }

        public bool Equals(DURATION other)
        {
            if (other == null) return false;
            return (this.weeks == other.WEEKS) && (this.days == other.DAYS) &&
                (this.hours == other.HOURS) && (this.minutes == other.MINUTES) &&
                (this.seconds == other.SECONDS);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = (DURATION)obj;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return 
                this.weeks.GetHashCode() ^ 
                this.days.GetHashCode() ^ 
                this.hours.GetHashCode() ^
                this.minutes.GetHashCode() ^
                this.seconds.GetHashCode();
        }

        public int CompareTo(DURATION other)
        {
            if (other == null) return -2; //undefined
            var w = this.weeks + (this.days / 7) + (this.hours / 168) + (this.minutes / 10080) + (this.seconds / 604800);
            var ow = other.WEEKS + (other.DAYS / 7) + (other.HOURS / 168) + (other.MINUTES / 10080) + (other.SECONDS / 604800);
            if (w < ow) return -1;
            else if(w > ow) return 1;
            else
            {
                var d = this.days + (this.hours / 24) + (this.minutes / 1440) + (this.seconds / 86400);
                var od = other.DAYS + (other.HOURS / 24) + (other.MINUTES / 1440) + (other.SECONDS / 86400);
                if (d < od) return -1;
                else if (d > od) return 1;
                else
                {
                    var h = this.hours + (this.minutes / 60) + (this.seconds / 3600);
                    var oh = other.HOURS + (other.MINUTES / 60) + (other.SECONDS / 3600);
                    if (h < oh) return -1;
                    else if (h > oh) return 1;
                    else
                    {
                        var m = this.minutes + (this.seconds / 60);
                        var om = this.minutes + (this.seconds / 60);
                        if (m < om) return -1;
                        else if (m > om) return 1;
                        else
                        {
                            if (this.seconds < other.SECONDS) return -1;
                            else if (this.seconds > other.SECONDS) return 1;
                            else return 0;
                        }
                        
                    }
                }
            }

        }

        #region overloaded operators

        public static DURATION operator +(DURATION a, DURATION b)
        {
            var duration = new DURATION();
            duration.DAYS = (a.DAYS + b.DAYS);
            duration.SECONDS = (a.SECONDS + b.SECONDS);
            duration.MINUTES = (a.MINUTES + b.MINUTES);
            duration.HOURS = (a.MINUTES + b.MINUTES);
            duration.WEEKS = (a.WEEKS + b.WEEKS);
            return duration;
        }

        public static DURATION operator -(DURATION a, DURATION b)
        {
            var duration = new DURATION();
            duration.DAYS = (a.DAYS - b.DAYS);
            duration.SECONDS = (a.SECONDS - b.SECONDS);
            duration.MINUTES = (a.MINUTES - b.MINUTES);
            duration.HOURS = (a.MINUTES - b.MINUTES);
            duration.WEEKS = (a.WEEKS - b.WEEKS);
            return duration;
        }

        public static bool operator <(DURATION a, DURATION b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator >(DURATION a, DURATION b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator <=(DURATION a, DURATION b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 1 || a.CompareTo(b) == 0;
        }

        public static bool operator >=(DURATION a, DURATION b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 1 || a.CompareTo(b) == 0;

        }

        public static bool operator ==(DURATION a, DURATION b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(DURATION a, DURATION b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        #endregion
    }

    [DataContract]
    public class PERIOD : IPERIOD, IEquatable<PERIOD>, IComparable<PERIOD>
    {

        private DATE_TIME start, end;
        private DURATION duration;
        private PeriodFormat format;

        [DataMember]
        public IDATE_TIME Start
        {
            get { return start; }
            set 
            {
                if (value == null) throw new ArgumentNullException("start");
                this.start.FULLYEAR = value.FULLYEAR;
                this.start.MONTH = value.MONTH;
                this.start.MDAY = value.MDAY;
                this.start.HOUR = value.HOUR;
                this.start.MINUTE = value.MINUTE;
                this.start.SECOND = value.SECOND;
                this.start.TimeZoneId = value.TimeZoneId;
            }
        }

        [DataMember]
        public IDATE_TIME End
        {
            get { return end; }
            set
            {
                if (value == null) throw new ArgumentNullException("end");
                this.end.FULLYEAR = value.FULLYEAR;
                this.end.MONTH = value.MONTH;
                this.start.MDAY = value.MDAY;
                this.start.HOUR = value.HOUR;
                this.start.MINUTE = value.MINUTE;
                this.start.SECOND = value.SECOND;
                this.start.TimeZoneId = value.TimeZoneId; 
                this.duration = this.end - this.start;
                this.format = PeriodFormat.Explicit;
            }
        }

        [DataMember]
        public IDURATION Duration
        {
            get { return duration; }
            set
            {
                if (value == null) throw new ArgumentNullException("duration");
                this.duration.WEEKS = value.WEEKS;
                this.duration.DAYS = value.DAYS;
                this.duration.HOURS = value.HOURS;
                this.duration.MINUTES = value.MINUTES;
                this.duration.SECONDS = value.SECONDS;
                this.duration.Sign = value.Sign;
                this.end = this.start + this.duration;
                this.format = PeriodFormat.Start;
            }
        }

        public PERIOD()
        {
            this.start = new DATE_TIME();
            this.end = new DATE_TIME();
            this.format = PeriodFormat.Start;
        }

        public PERIOD (DATE_TIME start, DATE_TIME end)
        {
            this.Start = start;
            this.End = end;
            this.format = PeriodFormat.Explicit;
        }

        public PERIOD(DateTime start, DateTime end, TimeZoneInfo stzinfo=null, TimeZoneInfo etzinfo= null)
        {
            this.Start = start.To_IDATE_TIME<DATE_TIME, TZID>(stzinfo);
            this.End = end.To_IDATE_TIME<DATE_TIME, TZID>(etzinfo);
            this.format = PeriodFormat.Explicit;
        }

        public PERIOD(DATE_TIME start, DURATION duration)
        {
            this.Start = start;
            this.Duration = duration;
            this.format = PeriodFormat.Start;
        }

        public PERIOD(DateTime start, TimeSpan span, TimeZoneInfo stzinfo=null)
        {
            this.Start = start.To_IDATE_TIME<DATE_TIME, TZID>(stzinfo);
            this.Duration = span.To_IDURATION<DURATION>();
            this.format = PeriodFormat.Start;
        }

        public bool IsDefault()
        {
            return (this.start.IsDefault() && this.end.IsDefault());
        }

        public override string ToString()
        {
            return (this.format == PeriodFormat.Start)?
                string.Format("{0}/{1}", this.start, this.duration):
                string.Format("{0}/{1}", start, duration);
        }

        public bool Equals(PERIOD other)
        {
            if (other == null) return false;
            return this.start == (DATE_TIME)other.Start && this.duration == (DURATION)other.Duration;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals((PERIOD)obj);
        }

        public override int GetHashCode()
        {
            return this.start.GetHashCode() ^ this.duration.GetHashCode();
        }

        public int CompareTo(PERIOD other)
        {
            if (other == null) return -10; //undefined
            return (this.format == PeriodFormat.Explicit)?
                this.start.CompareTo(other.start) + this.end.CompareTo(other.end):
                this.start.CompareTo(other.start) + this.duration.CompareTo(other.duration);

        }

        #region overloaded operators

        public static bool operator <(PERIOD a, PERIOD b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == -2;
        }

        public static bool operator >(PERIOD a, PERIOD b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 2;
        }

        public static bool operator <=(PERIOD a, PERIOD b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == -2 || a.CompareTo(b) == 0;
        }

        public static bool operator >=(PERIOD a, PERIOD b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 2 || a.CompareTo(b) == 0;
        }

        public static bool operator ==(PERIOD a, PERIOD b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(PERIOD a, PERIOD b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        #endregion

    }

    [DataContract]
    public class WEEKDAYNUM : IWEEKDAYNUM, IEquatable<WEEKDAYNUM>, IComparable<WEEKDAYNUM>
    {

        [DataMember]
        public int OrdinalWeek { get; set; }

        [DataMember]
        public WEEKDAY Weekday { get; set; }

        public bool IsDefault
        {
            get
            {
                return (this.OrdinalWeek == 0 && this.Weekday == WEEKDAY.UNKNOWN);
            }
        }

        public WEEKDAYNUM(int OrdinalWeek, WEEKDAY Weekday)
        {
            this.OrdinalWeek = OrdinalWeek;
            this.Weekday = Weekday;
        }

        public bool Equals(WEEKDAYNUM other)
        {
            if (other == null) return false;
            return this.OrdinalWeek == other.OrdinalWeek && this.Weekday == other.Weekday;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals((WEEKDAYNUM)obj);
        }

        public override int GetHashCode()
        {
            return this.OrdinalWeek.GetHashCode() ^ this.Weekday.GetHashCode();
        }

        public override string ToString()
        {
            if (this.OrdinalWeek != 0)
            {
                return (this.OrdinalWeek < 0) ?
                    string.Format("{0} {1}", (uint)this.OrdinalWeek, this.Weekday) :
                    string.Format("+{0} {1}", (uint)this.OrdinalWeek, this.Weekday);
            }
            else return string.Format("{0}", this.Weekday);

        }

        public int CompareTo(WEEKDAYNUM other)
        {
            if (other == null) return -2; //undefined
            if (this.OrdinalWeek != 0)
            {
                if (this.OrdinalWeek < other.OrdinalWeek) return -1;
                else if (this.OrdinalWeek > other.OrdinalWeek) return 1;
                else
                {
                    if (this.Weekday < other.Weekday) return -1;
                    else if (this.Weekday > other.Weekday) return 1;
                    else return 0;
                }
            }

            if (this.Weekday < other.Weekday) return -1;
            else if (this.Weekday > other.Weekday) return 1;
            else return 0;
        }

        public static bool operator ==(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public static bool operator < (WEEKDAYNUM a, WEEKDAYNUM b)
        {
            if (b == null) return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator >(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            if (b == null) return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator <= (WEEKDAYNUM a, WEEKDAYNUM b)
        {
            if (b == null) return false;
            return a.CompareTo(b) == -1 || a.CompareTo(b) == 0;;
        }

        public static bool operator >=(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            if (b == null) return false;
            return a.CompareTo(b) == 1 || a.CompareTo(b) == 0;
        }

    }

    [DataContract]
    [KnownType(typeof(WEEKDAYNUM))]
    public class RECUR : IRECUR, IEquatable<RECUR>, IContainsKey<string>
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public RecurFormat Format { get; set; }


        [DataMember]
        public FREQ FREQ { get; set; }

        [DataMember]
        public IDATE_TIME UNTIL { get; set; }

        [DataMember]
        public uint COUNT { get; set; }

        [DataMember]
        public uint INTERVAL { get; set; }

        [DataMember]
        public IEnumerable<uint> BYSECOND { get; set; }

        [DataMember]
        public IEnumerable<uint> BYMINUTE { get; set; }

        [DataMember]
        public IEnumerable<uint> BYHOUR { get; set; }

        [DataMember]
        public IEnumerable<IWEEKDAYNUM> BYDAY { get; set; }

        [DataMember]
        public IEnumerable<int> BYMONTHDAY { get; set; }

        [DataMember]
        public IEnumerable<int> BYYEARDAY { get; set; }

        [DataMember]
        public IEnumerable<int> BYWEEKNO { get; set; }

        [DataMember]
        public IEnumerable<uint> BYMONTH { get; set; }

        [DataMember]
        public WEEKDAY WKST { get; set; }

        [DataMember]
        public IEnumerable<int> BYSETPOS { get; set; }

        public bool IsDefault()
        {
                return 
                    (
                        this.FREQ == FREQ.DAILY &&
                        this.COUNT == 0 &&
                        this.INTERVAL == 1 &&
                        this.Format == RecurFormat.DateTime &&
                        this.BYSECOND.Count() == 0 &&
                        this.BYMINUTE.Count() == 0 &&
                        this.BYHOUR.Count() == 0 &&
                        this.BYDAY.Count() == 0 &&
                        this.BYMONTHDAY.Count() == 0 &&
                        this.BYYEARDAY.Count() == 0 &&
                        this.BYWEEKNO.Count() == 0 &&
                        this.BYMONTH.Count() == 0 &&
                        this.WKST == WEEKDAY.MO &&
                        this.BYSETPOS.Count() == 0
                    );
        }

        public RECUR()
        {
            this.FREQ = contracts.FREQ.UNKNOWN;
            this.UNTIL = null;
            this.COUNT = 0;
            this.INTERVAL = 1;
            this.BYSECOND = new List<uint>();
            this.BYMINUTE = new List<uint>();
            this.BYHOUR = new List<uint>();
            this.BYDAY = new List<IWEEKDAYNUM>();
            this.BYMONTHDAY = new List<int>();
            this.BYYEARDAY = new List<int>();
            this.BYWEEKNO = new List<int>();
            this.BYMONTH = new List<uint>();
            this.WKST = WEEKDAY.SU;
            this.BYSETPOS = new List<int>();
            this.Format = RecurFormat.DateTime;
        }

        public RECUR(FREQ freq, IDATE_TIME until)
        {
            this.FREQ = freq;
            this.UNTIL = until;
            this.COUNT = 0;
            this.INTERVAL = 1;
            this.BYSECOND = new List<uint>();
            this.BYMINUTE = new List<uint>();
            this.BYHOUR = new List<uint>();
            this.BYDAY = new List<IWEEKDAYNUM>();
            this.BYMONTHDAY = new List<int>();
            this.BYYEARDAY = new List<int>();
            this.BYWEEKNO = new List<int>();
            this.BYMONTH = new List<uint>();
            this.WKST = WEEKDAY.SU;
            this.BYSETPOS = new List<int>();
            this.Format = RecurFormat.DateTime;
        }

        public RECUR(FREQ freq, uint count, uint interval)
        {
            this.FREQ = freq;
            this.COUNT = count;
            this.INTERVAL = interval;
            this.BYSECOND = new List<uint>();
            this.BYMINUTE = new List<uint>();
            this.BYHOUR = new List<uint>();
            this.BYDAY = new List<IWEEKDAYNUM>();
            this.BYMONTHDAY = new List<int>();
            this.BYYEARDAY = new List<int>();
            this.BYWEEKNO = new List<int>();
            this.BYMONTH = new List<uint>();
            this.WKST = WEEKDAY.SU;
            this.BYSETPOS = new List<int>();
            this.Format = RecurFormat.Range;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("FREQ={0};", this.FREQ);

            if(this.Format  == RecurFormat.DateTime ) sb.AppendFormat("UNTIL={0};", this.UNTIL);            
            else sb.AppendFormat("COUNT={0};", this.COUNT);

            sb.AppendFormat("INTERVAL={0};", this.INTERVAL.ToString());
           
            if (this.BYSECOND.Count() != 0)
            { 
                sb.AppendFormat("BYSECOND=");
                foreach (var val in this.BYSECOND) 
                {
                    if (val != this.BYSECOND.Last()) sb.AppendFormat("{0}, ", val);
                    else sb.AppendFormat("{0};", val);
                }
            }

            if (!this.BYMINUTE.NullOrEmpty())
            { 
                sb.AppendFormat("BYMINUTE=");
                foreach (var val in this.BYMINUTE)
                {
                    if (val != this.BYMINUTE.Last()) sb.AppendFormat("{0}, ", val);
                    else sb.AppendFormat("{0};", val);
                }
            }

            if (!this.BYHOUR.NullOrEmpty())
            { 
                sb.AppendFormat("BYHOUR=");
                foreach (var val in this.BYHOUR)
                {
                    if (val != this.BYHOUR.Last()) sb.AppendFormat("{0}, ", val);
                    else sb.AppendFormat("{0};", val);
                }
            }

            if (!this.BYDAY.NullOrEmpty())
            { 
                sb.AppendFormat("BYDAY=");
                foreach (var val in this.BYDAY)
                {
                    if (val != this.BYDAY.Last()) sb.AppendFormat("{0}, ", val);
                    else sb.AppendFormat("{0};", val);
                }
            }

            if (!this.BYMONTHDAY.NullOrEmpty())
            {
                sb.AppendFormat("BYMONTHDAY=");
                foreach (var val in this.BYMONTHDAY)
                {
                    if (val != this.BYMONTHDAY.Last()) sb.AppendFormat("{0}, ", val);
                    else sb.AppendFormat("{0};", val);
                }
            }

            if (!this.BYYEARDAY.NullOrEmpty())
            {
                sb.AppendFormat("BYYEARDAY=");
                foreach (var val in this.BYYEARDAY)
                {
                    if (val != this.BYYEARDAY.Last()) sb.AppendFormat("{0}, ", val);
                    else sb.AppendFormat("{0};", val);
                }
            } 
           
            if (!this.BYWEEKNO.NullOrEmpty())
            {
                sb.AppendFormat("BYWEEKNO=");
                foreach (var val in this.BYWEEKNO)
                {
                    if (val != this.BYWEEKNO.Last()) sb.AppendFormat("{0}, ", val);
                    else sb.AppendFormat("{0};", val);
                }
            } 

            if (!this.BYMONTH.NullOrEmpty())
            {
                sb.AppendFormat("BYWEEKNO=");
                foreach (var val in this.BYMONTH)
                {
                    if (val != this.BYMONTH.Last()) sb.AppendFormat("{0}, ", val);
                    else sb.AppendFormat("{0};", val);
                }
            }

            if (!this.BYSETPOS.NullOrEmpty())
            {
                sb.AppendFormat("BYSETPOS=");
                foreach (var m in this.BYMONTH)
                {
                    if (m != this.BYSETPOS.Last())
                    {
                        if (m < 0) sb.AppendFormat("-{0}, ", m);
                        else if (m > 0) sb.AppendFormat("+{0}, ", m);
                        else sb.AppendFormat("{0}, ", m);
                    }
                    else
                    {
                        if (m < 0) sb.AppendFormat("-{0};", m);
                        else if (m > 0) sb.AppendFormat("+{0};", m);
                        else sb.AppendFormat("{0};", m);
                    }
                }
            }

            sb.AppendFormat("WKST={0};", this.WKST);
            return sb.ToString();
        }

        public bool Equals(RECUR other)
        {
            if (other == null) return false;
            return
                this.FREQ == other.FREQ &&
                (this.Format == RecurFormat.DateTime) ?
                this.UNTIL.ToString().Equals(other.UNTIL.ToString(), StringComparison.OrdinalIgnoreCase) : 
                this.COUNT == other.COUNT &&
                this.INTERVAL == other.INTERVAL &&
                this.BYSECOND.AreDuplicatesOf(other.BYSECOND) &&
                this.BYHOUR.AreDuplicatesOf(other.BYHOUR) &&
                this.BYDAY.AreDuplicatesOf(other.BYDAY) &&
                this.BYMONTHDAY.AreDuplicatesOf(other.BYMONTHDAY) &&
                this.BYYEARDAY.AreDuplicatesOf(other.BYYEARDAY) &&
                this.BYWEEKNO.AreDuplicatesOf(other.BYWEEKNO) &&
                this.BYMONTH.AreDuplicatesOf(other.BYMONTH) &&
                this.WKST == other.WKST &&
                this.BYSETPOS == other.BYSETPOS;

        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as RECUR);
        }

        public override int GetHashCode()
        {
            var hash =
                this.FREQ.GetHashCode() ^
                this.INTERVAL.GetHashCode() ^
                this.BYSECOND.GetHashCode() ^
                this.BYHOUR.GetHashCode() ^
                this.BYDAY.GetHashCode() ^
                this.BYMONTHDAY.GetHashCode() ^
                this.BYYEARDAY.GetHashCode() ^
                this.BYWEEKNO.GetHashCode() ^
                this.BYMONTH.GetHashCode() ^
                this.WKST.GetHashCode() ^
                this.BYSETPOS.GetHashCode();

            if (this.Format == RecurFormat.DateTime) hash ^= this.UNTIL.GetHashCode();
            else hash ^= this.COUNT.GetHashCode();

            return hash;

        }

        public static bool operator ==(RECUR a, RECUR b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(RECUR a, RECUR b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);            
        }

    }

    [DataContract]
    public class URI : IURI, IEquatable<URI>, IComparable<URI>
    {
        private string path;

        [DataMember]
        public string Path 
        {
            get { return this.path; }
            set 
            {
                if (Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute)) this.path = value;
                else throw new FormatException("The format of the path is not URI-compatible");
            } 
        
        }

        public bool IsDefault()
        {
           return (this.Path == null);
        }

        public URI(string value)
        {
            if (Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute)) this.path = value;
            else throw new FormatException("The path is not a valid uri!");
        }

        public override string ToString()
        {
            return string.Format("{0}", this.path);
        }

        public bool Equals(URI other)
        {
            if (other == null) return false;
            return (this.path.Equals(other.Path, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = (URI)obj;
            return (other == null) ? false : Equals(other);
        }

        public override int GetHashCode()
        {
            return this.path.GetHashCode();
        }

        public int CompareTo(URI other)
        {
            return this.path.CompareTo(other.Path);
        }

        public static bool operator <(URI x, URI y)
        {
            if (x == null || y == null) return false;
            return x.CompareTo(y) < 0;
        }

        public static bool operator >(URI x, URI y)
        {
            if (x == null || y == null) return false;
            return x.CompareTo(y) > 0;
        }

        public static bool operator ==(URI x, URI y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(URI x, URI y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }

    }

    [DataContract]
    [KnownType(typeof(TIME))]
    public class UTC_OFFSET : IUTC_OFFSET, IEquatable<UTC_OFFSET>, IComparable<UTC_OFFSET>
    {
        private uint hour;
        private uint minute;
        private uint second;
        private SignType sign;

        [DataMember]
        public SignType Sign 
        {
            get { return this.sign; }
            set 
            {
                if (value == SignType.Neutral) throw new ArgumentException("Sign must be Positive or Negative!");
                this.sign = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the hours
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;23 </exception>
        [DataMember]
        public uint HOUR
        {
            get { return this.hour; }
            set
            {
                if (value >= 0 && value < 24) 
                {
                    if (this.minute == 0 && this.second == 0 && value == 0)
                        throw new ArgumentOutOfRangeException("Simultaneous zero values for hour, minute and second ARE NOT allowed!");
                    this.hour = value; 
                }
                else throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets or sets the value of the minutes
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;59 </exception>
        [DataMember]
        public uint MINUTE
        {
            get { return this.minute; }
            set
            {
                if (value >= 0 && value < 59) 
                {
                    if (this.hour == 0 && this.second == 0 && value == 0)
                        throw new ArgumentOutOfRangeException("Simultaneous zero values for hour, minute and second ARE NOT allowed!");
                    this.minute = value; 
                }
                else throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets or sets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;60 </exception>
        [DataMember]
        public uint SECOND
        {
            get { return this.second; }
            set
            {
                //Note the exceptional case of iCalendar specification that allows for the &quot;6oth&quot; positive leap second.
                if (value >= 0 && value < 61) 
                {
                    if (this.hour == 0 && this.minute == 0 && value == 0)
                        throw new ArgumentOutOfRangeException("Simultaneous zero values for hour, minute and second ARE NOT allowed!");
                    this.second = value; 
                }
                else throw new ArgumentOutOfRangeException();
            }
        }

        public bool IsDefault()
        {
            return (this.sign == SignType.Positive && this.hour == 0 && this.minute == 0 && this.hour == 1); 
        }

        public UTC_OFFSET()
        {
            this.HOUR = 0u;
            this.MINUTE = 0u;
            this.SECOND = 1;
            this.Sign = SignType.Positive;
        }

        public UTC_OFFSET(uint hour, uint minute, uint second, SignType sign)
        {
            this.HOUR = hour;
            this.MINUTE = minute;
            this.Sign = sign;
        }

        public UTC_OFFSET (int hours, int minutes, int seconds)
        {
            this.HOUR = (uint)hours;
            this.MINUTE = (uint)minutes;
            this.SECOND = (uint)hours;

            if (hours > 0)
            {
                if (minutes > 0)
                {
                    if (seconds > 0) this.Sign = SignType.Positive;
                    else if (seconds < 0) this.Sign = SignType.Negative;
                    else this.Sign = SignType.Positive;
                }
                else if (minutes < 0) this.Sign = SignType.Negative;
                else if (minutes == 0)
                {
                    if (seconds > 0) this.Sign = SignType.Positive;
                    else if (seconds < 0) this.Sign = SignType.Negative;
                    else this.Sign = SignType.Positive;
                }
            }
            else if (hours < 0) this.Sign = SignType.Negative;
            else if (hours == 0)
            {
                if (minutes > 0 && seconds > 0) this.Sign = SignType.Positive;
                else this.Sign = SignType.Negative;
            }
        }

        public override string ToString()
        {
            if (this.sign == SignType.Negative) return string.Format("-{0:D2}{1:D2}{2:D2}", this.hour, this.minute, this.second);
            else return string.Format("+{0:D2}{1:D2}{2:D2}", this.hour, this.minute, this.second);
        }

        public bool Equals(UTC_OFFSET other)
        {
            if (other == null) return false;
            return (this.hour == other.HOUR) && (this.minute == other.MINUTE) && (this.second == other.SECOND) && (this.sign == other.Sign);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals((UTC_OFFSET) obj);
        }

        public override int GetHashCode()
        {
            return this.sign.GetHashCode() ^ this.hour.GetHashCode() ^this.minute.GetHashCode()^this.second.GetHashCode();
        }
        
        public int CompareTo(UTC_OFFSET other)
        {
            if (other == null) return 3;
            if (this.sign == SignType.Negative && other.Sign == SignType.Negative)
            {
                if (this.hour < other.hour) return 1;
                else if (this.hour > other.hour) return -1;
                else
                {
                    if (this.minute < other.minute) return 1;
                    else if (this.minute > other.minute) return -1;
                    else
                    {
                        if (this.second < other.second) return 1;
                        else if (this.second > other.second) return -1;
                        else return 0;
                    }
                }
            }
            else if (this.sign == SignType.Negative && other.Sign == SignType.Positive) return -1;
            else if (this.sign == SignType.Positive && other.sign == SignType.Negative) return 1;
            else if (this.sign ==  SignType.Positive && other.sign == SignType.Positive)
            {
                if (this.hour > other.hour) return 1;
                else if (this.hour < other.hour) return -1;
                else
                {
                    if (this.minute > other.minute) return 1;
                    else if (this.minute < other.minute) return -1;
                    else
                    {
                        if (this.second > other.second) return 1;
                        else if (this.second < other.second) return -1;
                        else return 0;
                    }
                }
            }

            return -2; //undefined

        }

        #region overloaded operators

        public static UTC_OFFSET operator +(UTC_OFFSET a, UTC_OFFSET b)
        {
            var sign = SignType.Neutral;
            if (a.Sign == SignType.Positive && b.Sign == SignType.Positive) sign = SignType.Positive;
            else if (a.Sign == SignType.Neutral && b.Sign == SignType.Positive) sign = SignType.Positive;
            else if (a.Sign == SignType.Positive && b.Sign == SignType.Neutral) sign = SignType.Positive;
            else if (a.Sign == SignType.Negative && b.Sign == SignType.Negative) sign = SignType.Negative;
            else if (a.Sign == SignType.Neutral && b.Sign == SignType.Negative) sign = SignType.Negative;
            else if (a.Sign == SignType.Negative && b.Sign == SignType.Neutral) sign = SignType.Negative; 
            else if (a.Sign == SignType.Neutral && b.Sign == SignType.Neutral) sign = SignType.Neutral;
            var offset = new UTC_OFFSET((a.HOUR + b.HOUR)% 24, (a.MINUTE + b.MINUTE)% 60, (a.SECOND + b.SECOND) % 60, sign);
            return offset;
        }

        public static UTC_OFFSET operator -(UTC_OFFSET a, UTC_OFFSET b)
        {
            var sign = SignType.Neutral;
            if (a.Sign == SignType.Positive && b.Sign == SignType.Positive) sign = SignType.Positive;
            else if (a.Sign == SignType.Neutral && b.Sign == SignType.Positive) sign = SignType.Positive;
            else if (a.Sign == SignType.Positive && b.Sign == SignType.Neutral) sign = SignType.Positive;
            else if (a.Sign == SignType.Negative && b.Sign == SignType.Negative) sign = SignType.Negative;
            else if (a.Sign == SignType.Neutral && b.Sign == SignType.Negative) sign = SignType.Negative;
            else if (a.Sign == SignType.Negative && b.Sign == SignType.Neutral) sign = SignType.Negative;
            else if (a.Sign == SignType.Neutral && b.Sign == SignType.Neutral) sign = SignType.Neutral;

            //TODO: Re-implement correct negative modulo calculations (HINT: Check former geographical code)
            var offset = new UTC_OFFSET(a.HOUR - b.HOUR, a.MINUTE - b.MINUTE, a.SECOND - b.SECOND, sign);
            return offset;
        }

        public static bool operator <(UTC_OFFSET a, UTC_OFFSET b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator >(UTC_OFFSET a, UTC_OFFSET b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator <=(UTC_OFFSET a, UTC_OFFSET b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == -2;
        }

        public static bool operator >=(UTC_OFFSET a, UTC_OFFSET b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) == 2;
        }

        public static bool operator ==(UTC_OFFSET a, UTC_OFFSET b)
        {
            if((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(UTC_OFFSET a, UTC_OFFSET b)
        {
            if((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);           
        }

        #endregion


    }

}
