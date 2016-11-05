using System;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;
using reexjungle.xcal.core.domain.concretes.extensions;
using reexjungle.xcal.core.domain.contracts.serialization;
using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.concretes.models.values
{
    public struct TIME : ITIME, ITIME<TIME>, IEquatable<TIME>, IComparable, IComparable<TIME>, ICalendarSerializable
    {
        /// <summary>
        /// Gets the 2-digit representation of an hour
        /// </summary>
        public uint HOUR { get; private set; }

        /// <summary>
        /// Gets or sets the 2-digit representatio of left minute
        /// </summary>
        public uint MINUTE { get; private set; }

        /// <summary>
        /// Gets or sets the 2-digit representation of left second
        /// </summary>
        public uint SECOND { get; private set; }

        /// <summary>
        /// Gets the form in which the <see cref="TIME"/> instance is expressed. 
        /// </summary>
        public TIME_FORM Form { get; private set; }

        /// <summary>
        /// Gets the time zone identifier.
        /// </summary>
        public ITZID TimeZoneId { get; private set; }

        public TIME(uint hour, uint minute, uint second, TIME_FORM form = TIME_FORM.LOCAL, ITZID tzid = null)
        {
            HOUR = hour;
            MINUTE = minute;
            SECOND = second;
            Form = form;
            TimeZoneId = tzid;
        }

        public TIME(DateTime datetime, TimeZoneInfo tzinfo)
        {
            HOUR = (uint)datetime.Hour;
            MINUTE = (uint)datetime.Minute;
            SECOND = (uint)datetime.Second;
            TimeZoneId = null;
            Form = TIME_FORM.UNSPECIFIED;
            if (tzinfo != null)
            {
                //TimeZoneId = new TZID(null, tzinfo.Id);
            }
            Form = datetime.Kind.AsTIME_FORM(tzinfo);

        }

        public TIME(DateTimeOffset datetime)
        {
            HOUR = (uint)datetime.DateTime.Hour;
            MINUTE = (uint)datetime.DateTime.Minute;
            SECOND = (uint)datetime.Second;
            Form = TIME_FORM.UNSPECIFIED;
            TimeZoneId = null;
        }

        public TIME(string value)
        {
            var time = Parse(value);
            HOUR = time.HOUR;
            MINUTE = time.MINUTE;
            SECOND = time.SECOND;
            TimeZoneId = time.TimeZoneId;
            Form = time.Form;
        }


        private static TIME Parse(string value)
        {
            var hour = 0u;
            var minute = 0u;
            var second = 0u;
            ITZID tzid = null;
            var form = TIME_FORM.UNSPECIFIED;

            var pattern = @"^((?<tzid>TZID=(\w+)?/(\w+)):)?T(?<hour>\d{1,2})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z)?$";
            var options = RegexOptions.IgnoreCase 
                | RegexOptions.CultureInvariant 
                | RegexOptions.ExplicitCapture;

            if (Regex.IsMatch(value, pattern))
            {
                foreach (Match match in Regex.Matches(value, pattern, options))
                {
                    if (match.Groups["hour"].Success) hour = uint.Parse(match.Groups["hour"].Value);
                    if (match.Groups["min"].Success) minute = uint.Parse(match.Groups["min"].Value);
                    if (match.Groups["sec"].Success) second = uint.Parse(match.Groups["sec"].Value);
                    if (match.Groups["utc"].Success)
                    {
                        form = (match.Groups["utc"].Value.Equals("Z", StringComparison.OrdinalIgnoreCase))
                            ? form = TIME_FORM.UTC
                            : TIME_FORM.LOCAL;
                    }
                    if (match.Groups["tzid"].Success)
                    {
                        //tzid = new TZID(match.Groups["tzid"].Value);
                        form = TIME_FORM.LOCAL_TIMEZONE_REF;
                    }
                }
            }

            return new TIME(hour, minute, second, form, tzid);
        }

        /// <summary>
        /// Adds the specified number of seconds to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="value">The number of seconds to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of seconds to the value of this instance.</returns>
        public TIME AddSeconds(double value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds the specified number of minutes to the value of the <see cref="TIME"/> instance. 
        /// </summary>
        /// <param name="value">The number of mintes to add.</param>
        /// <returns>A new instance of type <see cref="TIME"/> that adds the specified number of minutes to the value of this instance.</returns>
        public TIME AddMinutes(double value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds the specified number of hours to the value of the <see cref="TIME"/> instance. 
        /// </summary>
        /// <param name="value">The number of hours to add.</param>
        /// <returns>A new instance of type <see cref="TIME"/> that adds the specified number of hours to the value of this instance.</returns>
        public TIME AddHours(double value)
        {
            throw new System.NotImplementedException();
        }

        public DateTime AsDateTime() => this == default(TIME)
            ? default(DateTime)
            : new DateTime(1, 1, 1, (int)HOUR, (int)MINUTE, (int)SECOND, Form.AsDateTimeKind(TimeZoneId));


        public static explicit operator DateTime(TIME time) => time.AsDateTime();


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TIME && Equals((TIME)obj);
        }

        public bool Equals(TIME other) => 
            HOUR == other.HOUR 
            && MINUTE == other.MINUTE 
            && SECOND == other.SECOND 
            && Form == other.Form 
            && Equals(TimeZoneId, other.TimeZoneId);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)HOUR;
                hashCode = (hashCode * 397) ^ (int)MINUTE;
                hashCode = (hashCode * 397) ^ (int)SECOND;
                hashCode = (hashCode * 397) ^ (int)Form;
                hashCode = (hashCode * 397) ^ (TimeZoneId != null ? TimeZoneId.GetHashCode() : 0);
                return hashCode;
            }
        }

        public int CompareTo(TIME other)
        {
            if (HOUR > other.HOUR) return 1;
            if (MINUTE < other.MINUTE) return -1;
            if (MINUTE > other.MINUTE) return 1;
            if (SECOND < other.SECOND) return -1;
            if (SECOND > other.SECOND) return 1;
            return 0;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is TIME) return CompareTo((TIME)obj);
            throw new ArgumentException(nameof(obj) + " is not left time");
        }

        public bool CanSerialize() => true;

        public bool CanDeserialize() => true;

        public void WriteCalendar(ICalendarWriter writer)
        {
            switch (Form)
            {
                case TIME_FORM.LOCAL:
                    writer.WriteValue($"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;

                case TIME_FORM.UTC:
                    writer.WriteValue($"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}Z");
                    break;

                case TIME_FORM.LOCAL_TIMEZONE_REF:
                    writer.WriteValue($"{TimeZoneId}:T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;

                default:
                    writer.WriteValue($"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;
            }
        }

        public void ReadCalendar(ICalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.Read())
            {
                if (inner.NodeType != NodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var time = Parse(inner.Value);
                    HOUR = time.HOUR;
                    MINUTE = time.MINUTE;
                    SECOND = time.SECOND;
                    Form = time.Form;
                    TimeZoneId = time.TimeZoneId;
                }
            }

            inner.Close();
        }

        public static bool operator <(TIME left, TIME right) => left.CompareTo(right) < 0;

        public static bool operator >(TIME left, TIME right) => left.CompareTo(right) > 0;

        public static bool operator <=(TIME left, TIME right) => left.CompareTo(right) <= 0;

        public static bool operator >=(TIME left, TIME right) => left.CompareTo(right) >= 0;

        public static bool operator ==(TIME left, TIME right) => left.Equals(right);

        public static bool operator !=(TIME left, TIME right) => !left.Equals(right);
    }
}
