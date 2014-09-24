using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.infrastructure.operations.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.extensions;

namespace reexmonkey.xcal.domain.models
{
    [DataContract]
    public class BINARY : IBINARY, IEquatable<BINARY>
    {
        private ENCODING encoding;

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
                    if (this.encoding == ENCODING.BASE64) this.Value = this.Value.EncodeToBase64();
                    else this.Value = this.Value.EncodeToUtf8();
                };
            }

        }

        public BINARY()
        {
            this.Value = string.Empty;
            this.Encoding = ENCODING.UNKNOWN;
        }

        public BINARY(string value, ENCODING encoding)
        {
            this.Value = value;
            this.Encoding = encoding;
        }

        public BINARY(IBINARY binary)
        {
            if (binary == null) throw new ArgumentNullException("binary");
            this.Value = binary.Value;
            this.Encoding = binary.Encoding;
        }

        public bool Equals(BINARY other)
        {
            if (other == null) return false;
            return this.Encoding == other.Encoding && this.Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals((BINARY)obj);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode() ^ this.encoding.GetHashCode();
        }

        public static bool operator ==(BINARY x, BINARY y)
        {
            if ((object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(BINARY a, BINARY b)
        {
            if ((object)a == null || (object)b == null)  return !object.Equals(a, b);
            return !(a.Equals(b));
        }

        public override string ToString()
        {
            return this.Value ?? string.Empty;
        }

    }

    [DataContract]
    public struct DATE : IDATE, IEquatable<DATE>, IComparable<DATE>
    {
        private readonly uint fullyear, month, mday;

        /// <summary>
        /// Gets the 4-digit representation of a full year e.g. 2013 
        /// </summary>
        public uint FULLYEAR
        {
            get { return fullyear; }
        }

        /// <summary>
        /// Gets the 2-digit representation of a month
        /// </summary>
        public uint MONTH
        {
            get { return month; }
        }

        /// <summary>
        /// Gets the 2-digit representation of a month-day
        /// </summary>
        public uint MDAY
        {
            get { return mday; }
        }

        public DATE(uint fullyear, uint month, uint mday)
        {
            this.fullyear = fullyear;
            this.month = month;
            this.mday = mday;
        }

        public DATE(DATE_TIME datetime)
        {
            this.fullyear = datetime.FULLYEAR;
            this.month = datetime.MONTH;
            this.mday = datetime.MDAY;
        }

        public DATE(DateTime datetime)
        {
            this.fullyear = (uint)datetime.Year;
            this.month = (uint)datetime.Month;
            this.mday = (uint)datetime.Day;
        }

        public DATE(string value)
        {
            this.fullyear = 1u;
            this.month = 1u;
            this.mday = 1u;
            var pattern = @"^(?<year>\d{2,4})(?<month>\d{1,2})(?<day>\d{1,2})$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["year"].Success) this.fullyear = uint.Parse(match.Groups["year"].Value);
                    if (match.Groups["month"].Success) this.month = uint.Parse(match.Groups["month"].Value);
                    if (match.Groups["day"].Success) this.mday = uint.Parse(match.Groups["day"].Value);
                }
            }
        }

        public DATE(IDATE date)
        {
            if (date == null) throw new ArgumentNullException("date");
            this.fullyear = date.FULLYEAR;
            this.month = date.MONTH;
            this.mday = date.MDAY;
        }

        public override string ToString()
        {
            return string.Format("{0:D4}{1:D2}{2:D2}", this.fullyear, this.month, this.mday);  //YYYYMMDD
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
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals((DATE)obj);
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
            if ((object)a == null || (object)b == null)  return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public static bool operator <(DATE a, DATE b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator <=(DATE a, DATE b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return (a.CompareTo(b) == -1 || a.CompareTo(b) == 0);
        }

        public static bool operator >(DATE a, DATE b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator >=(DATE a, DATE b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return (a.CompareTo(b) == 1 || a.CompareTo(b) == 0);
        }

        public static DURATION operator -(DATE start, DATE end)
        {
           return new DURATION(end.ToDateTime() - start.ToDateTime());
        }

        public static DATE operator +(DATE start, DURATION duration)
        {
            return (start.ToDateTime().Add(duration.ToTimeSpan())).ToDATE();
        }

        public static DATE operator -(DATE end, DURATION duration)
        {
            return (end.ToDateTime().Subtract(duration.ToTimeSpan())).ToDATE();
        }

    }

    [DataContract]
    public struct DATE_TIME : IDATE_TIME, IEquatable<DATE_TIME>, IComparable<DATE_TIME>
    {
        private readonly uint hour, minute, second, fullyear, month, mday;
        private readonly TimeType format;
        private readonly TZID tzid;

        /// <summary>
        /// Gets the 4-digit representation of a full year e.g. 2013 
        /// </summary>
        public uint FULLYEAR
        {
            get { return fullyear; }
        }

        /// <summary>
        /// Gets the 2-digit representation of a month
        /// </summary>
        public uint MONTH
        {
            get { return month; }
        }

        /// <summary>
        /// Gets the 2-digit representation of a month-day
        /// </summary>
        public uint MDAY
        {
            get { return mday; }
        }

        /// <summary>
        /// Gets the value of the hours
        /// </summary>
        public uint HOUR
        {
            get { return this.hour; }
        }

        /// <summary>
        /// Gets  the value of the minutes
        /// </summary>
        public uint MINUTE
        {
            get { return this.minute; }
        }

        /// <summary>
        /// Gets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;60 </exception>
        public uint SECOND
        {
            get { return this.second; }
        }

        public TimeType Type
        {
            get { return this.format; }
        }

        public TZID TimeZoneId
        {
            get { return this.tzid; }
        }

        public DATE_TIME(uint fullyear, uint month, uint mday, uint hour, uint minute, uint second,
            TimeType format = TimeType.Local, TZID tzid = null)
        {
            this.fullyear = fullyear;
            this.month = month;
            this.mday = mday;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.format = format;
            this.tzid = tzid;
        }

        public DATE_TIME(DateTime datetime, TimeZoneInfo tzinfo = null)
        {
            this.fullyear = (uint)datetime.Year;
            this.month = (uint)datetime.Month;
            this.mday = (uint)datetime.Day;
            this.hour = (uint)datetime.Hour;
            this.minute = (uint)datetime.Minute;
            this.second = (uint)datetime.Second;
            this.tzid = null;
            this.format = contracts.TimeType.Unknown;
            if (tzinfo != null)
            {
                this.tzid = new TZID(null, tzinfo.Id);
                this.format = TimeType.LocalAndTimeZone;
            }
            else this.format = TimeType.Local;
            if (datetime.Kind == DateTimeKind.Utc) this.format = TimeType.Utc;
        }

        public DATE_TIME(DateTime datetime, TZID tzid)
        {
            this.fullyear = (uint)datetime.Year;
            this.month = (uint)datetime.Month;
            this.mday = (uint)datetime.Day;
            this.hour = (uint)datetime.Hour;
            this.minute = (uint)datetime.Minute;
            this.second = (uint)datetime.Second;
            this.tzid = null;
            this.format = contracts.TimeType.Unknown;
            if (tzid != null)
            {
                this.tzid = tzid;
                this.format = TimeType.LocalAndTimeZone;
            }
            else this.format = TimeType.Local;
            if (datetime.Kind == DateTimeKind.Utc) this.format = TimeType.Utc;
        }

        public DATE_TIME(DateTimeOffset datetime)
        {
            this.fullyear = (uint)datetime.Year;
            this.month = (uint)datetime.Month;
            this.mday = (uint)datetime.Day;
            this.hour = (uint)datetime.Hour;
            this.minute = (uint)datetime.Minute;
            this.second = (uint)datetime.Second;
            this.format = TimeType.Utc;
            this.tzid = null;
        }

        public DATE_TIME(DATE date, TZID tzid = null)
        {
            if (date == null) throw new ArgumentNullException("date");
            this.fullyear = date.FULLYEAR;
            this.month = date.MONTH;
            this.mday = date.MDAY;
            this.hour = 0u;
            this.minute = 0u;
            this.second = 0u;
            this.tzid = tzid;
            if (this.tzid != null) this.format = TimeType.LocalAndTimeZone;
            else this.format = TimeType.Unknown;
            this.format = TimeType.Unknown;
        }

        public DATE_TIME(TIME time)
        {
            if (time == null) throw new ArgumentNullException("time");
            this.fullyear = 1u;
            this.month = 1u;
            this.mday = 1u;
            this.hour = time.HOUR;
            this.minute = time.MINUTE;
            this.second = time.SECOND;
            this.tzid = time.TimeZoneId;
            if (this.tzid != null) this.format = TimeType.LocalAndTimeZone;
            else this.format = TimeType.Unknown;
            this.format = TimeType.Unknown;
        }

        public DATE_TIME(string value)
        {
            this.fullyear = 1u;
            this.month = 1u;
            this.mday = 1u;
            this.hour = 0u;
            this.minute = 0u;
            this.second = 0u;
            this.tzid = null;
            this.format = TimeType.Unknown;

            var pattern = @"^(?<tzid>((\p{L})+)*(\/)*((\p{L}+\p{P}*\s*)+):)*(?<year>\d{2,4})(?<month>\d{1,2})(?<day>\d{1,2})(T(?<hour>\d{1,2})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z?))?$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["year"].Success) this.fullyear = uint.Parse(match.Groups["year"].Value);
                    if (match.Groups["month"].Success) this.month = uint.Parse(match.Groups["month"].Value);
                    if (match.Groups["day"].Success) this.mday = uint.Parse(match.Groups["day"].Value);
                    if (match.Groups["hour"].Success) this.hour = uint.Parse(match.Groups["hour"].Value);
                    if (match.Groups["min"].Success) this.minute = uint.Parse(match.Groups["min"].Value);
                    if (match.Groups["sec"].Success) this.second = uint.Parse(match.Groups["sec"].Value);
                    if (match.Groups["utc"].Success)
                    {
                        if (match.Groups["utc"].Value.Equals("Z", StringComparison.OrdinalIgnoreCase))
                            this.format = TimeType.Utc;
                        else if (match.Groups["utc"].Value.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
                            this.format = TimeType.Local;
                    }
                    if (match.Groups["tzid"].Success)
                    {
                        this.tzid = new TZID(match.Groups["tzid"].Value);
                        this.format = TimeType.LocalAndTimeZone;
                    }
                }
            }
        }

        public DATE_TIME(IDATE_TIME datetime)
        {
            this.fullyear = datetime.FULLYEAR;
            this.month = datetime.MONTH;
            this.mday = datetime.MDAY;
            this.hour = datetime.HOUR;
            this.minute = datetime.MINUTE;
            this.second = datetime.SECOND;
            this.format = datetime.Type;
            this.tzid = datetime.TimeZoneId;
        }

        public override string ToString()
        {
            if (this.format == TimeType.Local) return string.Format("{0:D4}{1:D2}{2:D2}T{3:D2}{4:D2}{5:D2}", this.fullyear, this.month, this.mday, this.hour, this.minute, this.second);

            else if (this.format == TimeType.Utc)
                return string.Format("{0:D4}{1:D2}{2:D2}T{3:D2}{4:D2}{5:D2}Z", this.fullyear, this.month, this.mday, this.hour, this.minute, this.second);
            else if (this.Type == TimeType.LocalAndTimeZone)
                return string.Format("{0}:{1:D4}{2:D2}{3:D2}T{4:D2}{5:D2}{6:D2}", this.tzid, this.fullyear, this.month, this.mday, this.hour, this.minute, this.second);
            else
                return string.Format("{0:D4}{1:D2}{2:D2}T{3:D2}{4:D2}{5:D2}", this.fullyear, this.month, this.mday, this.hour, this.minute, this.second);
        }

        public bool Equals(DATE_TIME other)
        {
            if (other == null) return false;
            return (this.fullyear == other.FULLYEAR) && (this.month == other.MONTH) && (this.mday == other.MDAY) &&
                (this.hour == other.HOUR) && (this.minute == other.MINUTE) && (this.second == other.SECOND);

        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals((DATE_TIME)obj);
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
            return (start.ToDateTime().Add(duration.ToTimeSpan())).ToDATE_TIME();
        }

        public static DATE_TIME operator -(DATE_TIME end, DURATION duration)
        {
            return (end.ToDateTime().Subtract(duration.ToTimeSpan())).ToDATE_TIME(end.TimeZoneId);
        }

        public static DURATION operator -(DATE_TIME end, DATE_TIME start)
        {
            if (end > start) return new DURATION(end.ToDateTime() - start.ToDateTime());
            else if (end < start) return new DURATION(start.ToDateTime() - end.ToDateTime());
            else return new DURATION();
        }

        public static bool operator <(DATE_TIME a, DATE_TIME b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator >(DATE_TIME a, DATE_TIME b)
        {
            if ((object)a == null || (object)b == null) return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator <=(DATE_TIME a, DATE_TIME b)
        {
            if ((object)a == null || (object)b == null) return false;
            return (a.CompareTo(b) == -1 || a.CompareTo(b) == 0);
        }

        public static bool operator >=(DATE_TIME a, DATE_TIME b)
        {
            if ((object)a == null || (object)b == null)  return false;
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
    public struct TIME : ITIME, IEquatable<TIME>, IComparable<TIME>
    {
        private readonly uint hour, minute, second;
        private readonly TimeType format;
        private readonly TZID tzid;

        /// <summary>
        /// Gets or sets the value of the hours
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;23 </exception>
        public uint HOUR
        {
            get { return this.hour; }
        }

        /// <summary>
        /// Gets or sets the value of the minutes
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;59 </exception>
        public uint MINUTE
        {
            get { return this.minute; }
        }

        /// <summary>
        /// Gets or sets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;60 </exception>
        public uint SECOND
        {
            get { return this.second; }
        }

        public TimeType Type
        {
            get { return this.format; }
        }

        public TZID TimeZoneId
        {
            get { return this.tzid; }
        }

        public TIME(uint hour, uint minute, uint second, TimeType format = TimeType.Local, TZID tzid = null)
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.format = format;
            this.tzid = tzid;
        }

        public TIME(DateTime datetime, TimeZoneInfo tzinfo)
        {
            this.hour = (uint)datetime.Hour;
            this.minute = (uint)datetime.Minute;
            this.second = (uint)datetime.Second;
            this.tzid = null;
            this.format = contracts.TimeType.Unknown;
            if (tzinfo != null)
            {
                this.tzid = new TZID(null, tzinfo.Id);
                this.format = TimeType.LocalAndTimeZone;
            }
            else this.format = TimeType.Local;
            if (datetime.Kind == DateTimeKind.Utc) this.format = TimeType.Utc;
        }

        public TIME(DateTimeOffset datetime)
        {
            this.hour = (uint)datetime.Hour;
            this.minute = (uint)datetime.Minute;
            this.second = (uint)datetime.Second;
            this.format = TimeType.Utc;
            this.tzid = null;
        }

        public TIME(string value)
        {
            this.hour = 0u;
            this.minute = 0u;
            this.second = 0u;
            this.tzid = null;
            this.format = TimeType.Unknown;

            var pattern = @"^(?<tzid>((\p{L})+)*(\/)*((\p{L}+\p{P}*\s*)+):)*(T(?<hour>\d{1,2})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z?))?$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["hour"].Success) this.hour = uint.Parse(match.Groups["hour"].Value);
                    if (match.Groups["min"].Success) this.minute = uint.Parse(match.Groups["min"].Value);
                    if (match.Groups["sec"].Success) this.second = uint.Parse(match.Groups["sec"].Value);
                    if (match.Groups["utc"].Success)
                    {
                        if (match.Groups["utc"].Value.Equals("Z", StringComparison.OrdinalIgnoreCase))
                            this.format = TimeType.Utc;
                        else if (match.Groups["utc"].Value.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
                            this.format = TimeType.Local;
                    }
                    if (match.Groups["tzid"].Success)
                    {
                        this.tzid = new TZID(match.Groups["tzid"].Value);
                        this.format = TimeType.LocalAndTimeZone;
                    }
                }
            }
        }

        public TIME(DATE_TIME datetime)
        {
            this.hour = datetime.HOUR;
            this.minute = datetime.MINUTE;
            this.second = datetime.SECOND;
            this.format = datetime.Type;
            this.tzid = datetime.TimeZoneId;
        }

        public TIME(ITIME time)
        {
            if (time == null) throw new ArgumentNullException("time");
            this.hour = time.HOUR;
            this.minute = time.MINUTE;
            this.second = time.SECOND;
            this.format = time.Type;
            this.tzid = time.TimeZoneId;
        }

        public override string ToString()
        {
            if (this.format == TimeType.Local) return string.Format("T{0:D2}{1:D2}{2:D2}", this.hour, this.minute, this.second);

            else if (this.format == TimeType.Utc)
                return string.Format("T{0:D2}{1:D2}{2:D2}Z", this.hour, this.minute, this.second);
            else if (this.Type == TimeType.LocalAndTimeZone)
                return string.Format("{0}:T{0:D2}{1:D2}{2:D2}", this.tzid, this.hour, this.minute, this.second);
            else
                return string.Format("T{0:D2}{1:D2}{2:D2}", this.hour, this.minute, this.second);
        }

        public bool Equals(TIME other)
        {
            if (other == null) return false;
            return (this.hour == other.HOUR) && (this.minute == other.MINUTE) && (this.second == other.SECOND);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals((TIME)obj);
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
            return (start.ToTimeSpan().Add(duration.ToTimeSpan())).ToTIME(start.TimeZoneId);
        }

        public static TIME operator -(TIME end, DURATION duration)
        {
            return (end.ToTimeSpan().Subtract(duration.ToTimeSpan())).ToTIME(end.TimeZoneId);
        }

        public static DURATION operator -(TIME start, TIME end)
        {
            return new DURATION(end.ToTimeSpan() - start.ToTimeSpan());
        }

        public static bool operator <(TIME a, TIME b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator >(TIME a, TIME b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator <=(TIME a, TIME b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == -2;
        }

        public static bool operator >=(TIME a, TIME b)
        {
            if ((object)a == null || (object)b == null)  return false;
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
    public struct DURATION : IDURATION, IEquatable<DURATION>, IComparable<DURATION>
    {
        private readonly int weeks, days, hours, minutes, seconds;

        public int WEEKS
        {
            get { return weeks; }
        }

        public int HOURS
        {
            get { return this.hours; }
        }

        public int MINUTES
        {
            get { return this.minutes; }
        }

        public int SECONDS
        {
            get { return this.seconds; }
        }

        public int DAYS
        {
            get { return this.days; }
        }

        public DURATION(DURATION duration)
        {
            this.weeks = duration.WEEKS;
            this.days = duration.DAYS;
            this.hours = duration.HOURS;
            this.minutes = duration.MINUTES;
            this.seconds = duration.SECONDS;
        }

        public DURATION(int weeks, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
        {
            this.weeks = weeks;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
        }

        public DURATION(TimeSpan span)
        {
            this.days = span.Days;
            this.hours = span.Hours;
            this.minutes = span.Minutes;
            this.seconds = span.Seconds;
            this.weeks = span.Days 
                + (span.Hours / 24) 
                + (span.Minutes / (24 * 60)) 
                + (span.Seconds / (24 * 3600)) 
                + (span.Milliseconds / (24 * 3600000)) / 7;
        }

        public DURATION(string value)
        {
            this.weeks = this.days = this.hours = this.minutes = this.seconds = 0;
            var pattern = @"^(?<minus>\-)?P((?<weeks>\d*)W)?((?<days>\d*)D)?(T((?<hours>\d*)H)?((?<mins>\d*)M)?((?<secs>\d*)S)?)?$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["weeks"].Success) this.weeks = int.Parse(match.Groups["weeks"].Value);
                    if (match.Groups["days"].Success) this.days = int.Parse(match.Groups["days"].Value);
                    if (match.Groups["hours"].Success) this.hours = int.Parse(match.Groups["hours"].Value);
                    if (match.Groups["mins"].Success) this.minutes = int.Parse(match.Groups["mins"].Value);
                    if (match.Groups["secs"].Success) this.seconds = int.Parse(match.Groups["secs"].Value);
                    if (match.Groups["minus"].Success)
                    {
                        this.weeks = -this.weeks;
                        this.days = -this.days;
                        this.minutes = -this.minutes;
                        this.seconds = -this.seconds;
                    }
                }
            }
        }

        public DURATION(IDURATION duration)
        {
            if (duration == null) throw new ArgumentNullException("duration");
            this.weeks = duration.WEEKS;
            this.days = duration.DAYS;
            this.hours = duration.HOURS;
            this.minutes = duration.MINUTES;
            this.seconds = duration.SECONDS;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var sign = (this.weeks < 0 || this.days < 0 || this.hours < 0 || this.minutes < 0 || this.seconds < 0) ? "-" : string.Empty;
            sb.AppendFormat("{0}P", sign);
            if (this.weeks != 0) sb.AppendFormat("{0}W", this.weeks);
            if (this.days != 0) sb.AppendFormat("{0}D", this.days);
            if (this.hours != 0 || this.minutes != 0 || this.seconds != 0) sb.Append("T");
            if (this.hours != 0) sb.AppendFormat("{0}H", this.hours);
            if (this.minutes != 0) sb.AppendFormat("{0}M", this.minutes);
            if (this.seconds != 0) sb.AppendFormat("{0}S", this.seconds);
            return sb.ToString();
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
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals((DURATION)obj);
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
            var span = this.ToTimeSpan();
            var ospan = other.ToTimeSpan();
            return span.CompareTo(ospan);
        }

        #region overloaded operators

        public static DURATION operator - (DURATION duration)
        {
            return new DURATION(-duration.WEEKS, -duration.DAYS, -duration.HOURS, -duration.MINUTES, -duration.SECONDS);
        }

        public static DURATION operator +(DURATION duration)
        {
            return new DURATION(duration.WEEKS, duration.DAYS, duration.HOURS, duration.MINUTES, duration.SECONDS);
        }

        public static DURATION operator +(DURATION a, DURATION b)
        {
            return new DURATION(a.WEEKS + b.WEEKS, a.DAYS + b.DAYS, a.HOURS + b.HOURS, a.MINUTES + b.MINUTES, a.SECONDS + b.SECONDS);
        }

        public static DURATION operator -(DURATION a, DURATION b)
        {
            return new DURATION(a.WEEKS - b.WEEKS, a.DAYS - b.DAYS, a.HOURS - b.HOURS, a.MINUTES - b.MINUTES, a.SECONDS - b.SECONDS);

        }

        public static DURATION operator *(DURATION duration, int scalar)
        {
            return new DURATION(duration.WEEKS * scalar, duration.DAYS * scalar, duration.HOURS * scalar, duration.MINUTES *scalar, duration.SECONDS *scalar);
        }

        public static DURATION operator /(DURATION duration, int scalar)
        {
            if (scalar == 0) throw new DivideByZeroException("Zero dividend is forbidden!");
            return new DURATION(duration.WEEKS / scalar, duration.DAYS / scalar, duration.HOURS / scalar, duration.MINUTES / scalar, duration.SECONDS / scalar);
        }

        public static bool operator <(DURATION a, DURATION b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator >(DURATION a, DURATION b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(DURATION a, DURATION b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) < 0 || a.CompareTo(b) == 0;
        }

        public static bool operator >=(DURATION a, DURATION b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) > 0 || a.CompareTo(b) == 0;

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
    public struct WEEKDAYNUM : IWEEKDAYNUM, IEquatable<WEEKDAYNUM>, IComparable<WEEKDAYNUM>
    {
        private readonly int ordweek;
        private readonly WEEKDAY weekday;

        public int OrdinalWeek
        {
            get { return this.ordweek; }
        }

        public WEEKDAY Weekday
        {
            get { return this.weekday; }
        }

        public WEEKDAYNUM( WEEKDAY weekday)
        {
            this.ordweek = 0;
            this.weekday = weekday;
        }

        public WEEKDAYNUM(int ordweek, WEEKDAY weekday)
        {
            this.ordweek = ordweek;
            this.weekday = weekday;
        }

        public WEEKDAYNUM(string value)
        {
            this.ordweek = 0;
            this.weekday = WEEKDAY.UNKNOWN;

            var pattern = @"^((?<sign>\w)? <?ordwk>\d{1,2})?(?<weekday>(SU|MO|TU|WE|TH|FR|SA)$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                var mulitplier = 1;
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["sign"].Success)
                    {
                        if (match.Groups["sign"].Value == "-") mulitplier *= -1;
                    }
                    if (match.Groups["ordwk"].Success) this.ordweek = mulitplier * int.Parse(match.Groups["ordwk"].Value);
                    if (match.Groups["weekday"].Success) this.weekday = match.Groups["weekday"].Value.ToWEEKDAY();
                }
            }
        }

        public WEEKDAYNUM(IWEEKDAYNUM weekdaynum)
        {
            if (weekdaynum == null) throw new ArgumentNullException("weekdaynum");
            this.ordweek = weekdaynum.OrdinalWeek;
            this.weekday = weekdaynum.Weekday;
        }

        public bool Equals(WEEKDAYNUM other)
        {
            if (other == null) return false;
            return this.OrdinalWeek == other.OrdinalWeek && this.Weekday == other.Weekday;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
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
                    string.Format("-{0} {1}", (uint)this.OrdinalWeek, this.Weekday) :
                    string.Format("+{0} {1}", (uint)this.OrdinalWeek, this.Weekday);
            }
            else return string.Format("{0}", this.Weekday);

        }

        public int CompareTo(WEEKDAYNUM other)
        {
            if (this.ordweek == 0 && other.OrdinalWeek == 0) return this.weekday.CompareTo(other.Weekday);
            else // this.ordweek != 0 || other.OrdinalWeek != 0
            {
                if (this.ordweek < other.OrdinalWeek) return -1;
                else if (this.ordweek > other.OrdinalWeek) return 1;
                else return this.weekday.CompareTo(other.Weekday); //this.ordweek == other.OrdinalWeek
            }
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

        public static bool operator <(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            if (b == null) return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator >(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            if (b == null) return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator <=(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            if (b == null) return false;
            return a.CompareTo(b) == -1 || a.CompareTo(b) == 0; ;
        }

        public static bool operator >=(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            if (b == null) return false;
            return a.CompareTo(b) == 1 || a.CompareTo(b) == 0;
        }

    }

    [DataContract]
    public struct UTC_OFFSET : IUTC_OFFSET, IEquatable<UTC_OFFSET>, IComparable<UTC_OFFSET>
    {
        private readonly int hour, minute, second;

        /// <summary>
        /// Gets or sets the value of the hours
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;23 </exception>
        public int HOUR
        {
            get { return this.hour; }
        }

        /// <summary>
        /// Gets or sets the value of the minutes
        /// </summary>
        public int MINUTE
        {
            get { return this.minute; }
        }

        /// <summary>
        /// Gets or sets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;60 </exception>
        public int SECOND
        {
            get { return this.second; }
        }

        public UTC_OFFSET(string value)
        {
            this.hour = this.minute = 0;
            this.second = 1;

            var pattern = @"^(?<minus>\-|?<plus>\+)(?<hours>\d{1,2})(?<mins>\d{1,2})(?<secs>\d{1,2})?$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["hours"].Success) this.hour = int.Parse(match.Groups["hours"].Value);
                    if (match.Groups["mins"].Success) this.minute = int.Parse(match.Groups["mins"].Value);
                    if (match.Groups["secs"].Success) this.second = int.Parse(match.Groups["secs"].Value);
                    if (match.Groups["minus"].Success)
                    {
                        this.hour = -this.hour;
                        this.minute = -this.minute;
                        this.second = -this.second;
                    }
                }
            }
        }

        public UTC_OFFSET(int hour, int minute, int second)
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }

        public UTC_OFFSET(IUTC_OFFSET offset)
        {
            this.hour = offset.HOUR;
            this.minute = offset.MINUTE;
            this.second = offset.SECOND;
        }

        public override string ToString()
        {
            if (this.hour < 0 || this.minute < 0 || this.second < 0) 
                return string.Format("-{0:D2}{1:D2}{2:D2}", this.hour, this.minute, this.second);
            else 
                return string.Format("+{0:D2}{1:D2}{2:D2}", this.hour, this.minute, this.second);
        }

        public bool Equals(UTC_OFFSET other)
        {
            if (other == null) return false;
            return (this.hour == other.HOUR) && (this.minute == other.MINUTE) && (this.second == other.SECOND);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals((UTC_OFFSET)obj);
        }

        public override int GetHashCode()
        {
            return this.hour.GetHashCode() ^ this.minute.GetHashCode() ^ this.second.GetHashCode();
        }

        public int CompareTo(UTC_OFFSET other)
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

        #region overloaded operators

        public static UTC_OFFSET operator +(UTC_OFFSET offset)
        {
            return new UTC_OFFSET(offset.HOUR, offset.MINUTE, offset.SECOND);
        }

        public static UTC_OFFSET operator -(UTC_OFFSET offset)
        {
            return new UTC_OFFSET(-offset.HOUR, -offset.MINUTE, -offset.SECOND);
        }

        public static UTC_OFFSET operator +(UTC_OFFSET a, UTC_OFFSET b)
        {
            return new UTC_OFFSET((a.HOUR + b.HOUR) % 24, (a.MINUTE + b.MINUTE) % 60, (a.SECOND + b.SECOND)% 60);
        }

        public static UTC_OFFSET operator -(UTC_OFFSET a, UTC_OFFSET b)
        {
            return new UTC_OFFSET((a.HOUR - b.HOUR).Modulo(60), (a.MINUTE - b.MINUTE).Modulo(60), (a.SECOND - b.SECOND).Modulo(60));

        }

        public static bool operator <(UTC_OFFSET a, UTC_OFFSET b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == -1;
        }

        public static bool operator >(UTC_OFFSET a, UTC_OFFSET b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == 1;
        }

        public static bool operator <=(UTC_OFFSET a, UTC_OFFSET b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == -2;
        }

        public static bool operator >=(UTC_OFFSET a, UTC_OFFSET b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == 2;
        }

        public static bool operator ==(UTC_OFFSET a, UTC_OFFSET b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(UTC_OFFSET a, UTC_OFFSET b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        #endregion


    }
    
    [DataContract]
    public struct PERIOD: IPERIOD, IEquatable<PERIOD>, IComparable<PERIOD>
    {
        private readonly DATE_TIME start, end;
        private readonly DURATION duration;
        private readonly PeriodFormat format;

        public DATE_TIME Start
        {
            get { return start; }
        }

        public DATE_TIME End
        {
            get { return end; }
        }

        public DURATION Duration
        {
            get { return duration; }
        }

        public PERIOD(string value)
        {
            this.start = default(DATE_TIME);
            this.end = default(DATE_TIME);
            this.duration = default(DURATION);
            this.format = PeriodFormat.Start;

            var parts = value.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts == null || parts.Length != 2) throw new FormatException("Invalid this format");

            try
            {
                this.start = new DATE_TIME(parts[0]);
                this.end = new DATE_TIME(parts[1]);
                this.duration = this.end - this.start;
            }
            catch (ArgumentException)
            {
                try 
                { 
                    this.duration = new DURATION(parts[1]);
                    this.end = this.start + this.duration;
                }
                catch (FormatException) { throw; }
                catch (Exception) { throw; }
            }
            catch (FormatException)
            {
                try 
                { 
                    this.duration = new DURATION(parts[1]);
                    this.end = this.start + this.duration;
                }
                catch (FormatException) { throw; }
                catch (Exception) { throw; }
            }
            catch (Exception)
            {
                try { this.duration = new DURATION(parts[1]); }
                catch (FormatException) { throw; }
                catch (Exception) { throw; }
            }
        }

        public PERIOD(DATE_TIME start, DATE_TIME end)
        {
            this.start = start;
            this.end = end;
            this.duration = end - start;
            this.format = PeriodFormat.Explicit;
        }

        public PERIOD(DATE_TIME start, DURATION duration)
        {
            this.start = start;
            this.duration = duration;
            this.end = start + duration;
            this.format = PeriodFormat.Start;
        }

        public PERIOD(DateTime start, DateTime end, TimeZoneInfo stimezone = null, TimeZoneInfo etimezone = null)
        {
            this.start = new DATE_TIME(start, stimezone);
            this.end = new DATE_TIME(end, etimezone);
            this.duration = this.end - this.start;
            this.format = PeriodFormat.Explicit;
        }

        public PERIOD(DateTime start, TimeSpan span, TimeZoneInfo stimezone = null)
        {
            this.start = new DATE_TIME(start, stimezone);
            this.duration = new DURATION(span);
            this.end = this.start + this.duration;
            this.format = PeriodFormat.Start;
        }

        public PERIOD(IPERIOD period)
        {
            if (period == null) throw new ArgumentNullException("period");
            this.start = period.Start;
            this.end = period.End;
            this.duration = period.Duration;
            this.format = PeriodFormat.Explicit;
        }

        public override string ToString()
        {
            return (this.format == PeriodFormat.Start) ?
                string.Format("{0}/{1}", this.start, this.duration) :
                string.Format("{0}/{1}", start, duration);
        }

        public bool Equals(PERIOD other)
        {
            if (other == null) return false;
            return this.start == other.Start && this.duration == other.Duration;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals((PERIOD)obj);
        }

        public override int GetHashCode()
        {
            return this.format == PeriodFormat.Explicit ?
                this.start.GetHashCode() ^ this.end.GetHashCode() :
                this.start.GetHashCode() ^ this.duration.GetHashCode();
        }

        public int CompareTo(PERIOD other)
        {
            if (other == null) return -10; //undefined
            return (this.format == PeriodFormat.Explicit) ?
                this.start.CompareTo(other.start) + this.end.CompareTo(other.end) :
                this.start.CompareTo(other.start) + this.duration.CompareTo(other.duration);

        }

        #region overloaded operators

        public static bool operator <(PERIOD a, PERIOD b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == -2;
        }

        public static bool operator >(PERIOD a, PERIOD b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == 2;
        }

        public static bool operator <=(PERIOD a, PERIOD b)
        {
            if ((object)a == null || (object)b == null)  return false;
            return a.CompareTo(b) == -2 || a.CompareTo(b) == 0;
        }

        public static bool operator >=(PERIOD a, PERIOD b)
        {
            if ((object)a == null || (object)b == null)  return false;
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
    public class RECUR: IRECUR, IEquatable<RECUR>, IContainsKey<string>
    {
        #region fields

        private FREQ freq;
        private DATE_TIME until;
        private uint count;
        private uint interval;
        private WEEKDAY wkst;
        private List<uint> bysecond;
        private List<uint> byminute;
        private List<uint> byhour;
        private List<WEEKDAYNUM> byday;
        private List<int> bymonthday;
        private List<int> byyearday;
        private List<int> byweekno;
        private List<uint> bymonth;
        private List<int> bysetpos;

        #endregion

        #region properties

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public FREQ FREQ
        {
            get { return this.freq; }
            set { this.freq = value; }
        }

        [DataMember]
        public DATE_TIME UNTIL
        {
            get { return this.until; }
            set { this.until = value; }
        }

        [DataMember]
        public uint COUNT
        {
            get { return this.count; }
            set { this.count = value; }
        }

        [DataMember]
        public uint INTERVAL
        {
            get { return this.interval; }
            set { this.interval = value; }
        }

        [DataMember]
        public List<uint> BYSECOND
        {
            get { return this.bysecond; }
            set { this.bysecond = value; }
        }

        [DataMember]
        public List<uint> BYMINUTE
        {
            get { return this.byminute; }
            set { this.byminute = value; }
        }

        [DataMember]
        public List<uint> BYHOUR
        {
            get { return this.byhour; }
            set { this.byhour = value; }
        }

        [DataMember]
        public List<WEEKDAYNUM> BYDAY
        {
            get { return this.byday; }
            set { this.byday = value; }
        }

        [DataMember]
        public List<int> BYMONTHDAY
        {
            get { return this.bymonthday; }
            set { this.bymonthday = value; }
        }

        [DataMember]
        public List<int> BYYEARDAY
        {
            get { return this.byyearday; }
            set { this.byyearday = value; }
        }

        [DataMember]
        public List<int> BYWEEKNO
        {
            get { return this.byweekno; }
            set { this.byweekno = value; }
        }

        [DataMember]
        public List<uint> BYMONTH
        {
            get { return this.bymonth; }
            set { this.bymonth = value; }
        }

        [DataMember]
        public WEEKDAY WKST
        {
            get { return this.wkst; }
            set { this.wkst = value; }
        }
        [DataMember]
        public List<int> BYSETPOS
        {
            get { return this.bysetpos; }
            set { this.bysetpos = value; }
        }
        #endregion

        public RECUR()
        {
            this.freq = FREQ.UNKNOWN;
            this.until = default(DATE_TIME);
            this.count = 0u;
            this.interval = 1u;
            this.wkst = WEEKDAY.SU;
        }

        public RECUR(string value)
        {
            this.freq = FREQ.UNKNOWN;
            this.until = default(DATE_TIME);
            this.count = 0u;
            this.interval = 1u;
            this.wkst = WEEKDAY.SU;

            var tokens = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens == null || tokens.Length == 0) throw new FormatException("Invalid Recur format");

            try
            {
                foreach (var token in tokens)
                {
                    var pattern = @"^(FREQ|UNTIL|COUNT|INTERVAL|BYSECOND|BYMINUTE|BYHOUR|BYMONTHDAY|BYYEARDAY|BYWEEKNO|BYMONTH|WKST|BYSETPOS)=((\w+|\d+)(,[\s]*(\w+|\d+))*)$";
                    if (!Regex.IsMatch(token, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)) continue;

                    var pair = token.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                    //check FREQ
                    if (pair[0].Equals("FREQ", StringComparison.OrdinalIgnoreCase)) this.FREQ = pair[1].ToFREQ();
                    if (pair[0].Equals("UNTIL", StringComparison.OrdinalIgnoreCase)) this.UNTIL = new DATE_TIME(pair[1]);
                    if (pair[0].Equals("COUNT", StringComparison.OrdinalIgnoreCase)) this.COUNT = uint.Parse(pair[1]);
                    if (pair[0].Equals("INTERVAL", StringComparison.OrdinalIgnoreCase)) this.INTERVAL = uint.Parse(pair[1]);
                    if (pair[0].Equals("BYSECOND", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts == null || parts.Length == 0) continue;
                        this.BYSECOND = parts.Select(x => uint.Parse(x)).ToList();
                    }
                    if (pair[0].Equals("BYMINUTE", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts == null || parts.Length == 0) continue;
                        this.BYMINUTE = parts.Select(x => uint.Parse(x)).ToList();
                    }
                    if (pair[0].Equals("BYHOUR", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts == null || parts.Length == 0) continue;
                        this.BYMINUTE = parts.Select(x => uint.Parse(x)).ToList();
                    }

                    if (pair[0].Equals("BYDAY", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts == null || parts.Length == 0) continue;
                        this.BYDAY = parts.Select(x => new WEEKDAYNUM(x)).ToList();
                    }

                    if (pair[0].Equals("BYMONTHDAY", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts == null || parts.Length == 0) continue;
                        this.BYMONTHDAY = parts.Select(x => int.Parse(x)).ToList();
                    }

                    if (pair[0].Equals("BYYEARDAY", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts == null || parts.Length == 0) continue;
                        this.BYYEARDAY = parts.Select(x => int.Parse(x)).ToList();
                    }

                    if (pair[0].Equals("BYWEEKNO", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts == null || parts.Length == 0) continue;
                        this.BYWEEKNO = parts.Select(x => int.Parse(x)).ToList();
                    }

                    if (pair[0].Equals("BYMONTH", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts == null || parts.Length == 0) continue;
                        this.BYMONTH = parts.Select(x => uint.Parse(x)).ToList();
                    }

                    if (pair[0].Equals("WKST", StringComparison.OrdinalIgnoreCase)) this.WKST = pair[1].ToWEEKDAY();

                    if (pair[0].Equals("BYSETPOS", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts == null || parts.Length == 0) continue;
                        this.BYSETPOS = parts.Select(x => int.Parse(x)).ToList();
                    }
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (FormatException) { throw; }
            catch (OverflowException) { throw; }
            catch (Exception) { throw; }
        }

        public RECUR(FREQ freq, DATE_TIME until)
        {
            this.freq = freq;
            this.until = until;
            this.count = 0u;
            this.interval = 1u;
            this.wkst = WEEKDAY.SU;
        }

        public RECUR(FREQ freq, uint count, uint interval)
        {
            this.freq = freq;
            this.until = new DATE_TIME();
            this.count = count;
            this.interval = interval;
            this.wkst = WEEKDAY.SU;
        }

        public RECUR(IRECUR recur)
        {
            this.freq = recur.FREQ;
            this.until = recur.UNTIL;
            this.count = recur.COUNT;
            this.interval = recur.INTERVAL;
            this.wkst = recur.WKST;
            this.bysecond = recur.BYSECOND;
            this.byminute = recur.BYMINUTE;
            this.byhour = recur.BYHOUR;
            this.byday = recur.BYDAY;
            this.bymonthday = recur.BYMONTHDAY;
            this.byyearday = recur.BYYEARDAY;
            this.byweekno = recur.BYWEEKNO;
            this.bymonth = recur.BYMONTH;
            this.bysetpos = recur.BYSETPOS;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("FREQ={0};", this.FREQ);
            if (this.UNTIL != default(DATE_TIME)) sb.AppendFormat("UNTIL={0};", this.UNTIL);
            else if(this.COUNT != 0) sb.AppendFormat("COUNT={0};", this.COUNT);
            sb.AppendFormat("INTERVAL={0};", this.INTERVAL.ToString());
            if (!this.BYSECOND.NullOrEmpty())
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
                sb.AppendFormat("BYMONTH=");
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

            sb.AppendFormat("WKST={0}", this.WKST);
            return sb.ToString();
        }

        public bool Equals(RECUR other)
        {
            if (other == null) return false;
            return this.Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals(obj as RECUR);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
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

                if (Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute) || value == null) this.path = value;
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

        public URI(IURI uri)
        {
            if (uri == null) throw new ArgumentNullException("uri"); 
            this.path = uri.Path;
        }

        public override string ToString()
        {
            return this.path;
        }

        public bool Equals(URI other)
        {
            if (other == null) return false;
            return (this.path.Equals(other.Path, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals(obj as URI);
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

}
