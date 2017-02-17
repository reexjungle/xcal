using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.serialization;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    public struct PERIOD : IComparable, IComparable<PERIOD>, ICalendarSerializable, IEquatable<PERIOD>, IConvertible
    {
        public DATE_TIME Start { get; private set; }

        public DATE_TIME End { get; private set; }

        public DURATION Duration { get; private set; }

        public bool Explicit { get; private set; }

        public PERIOD(DATE_TIME start, DATE_TIME end)
        {
            Start = start;
            End = end;
            Duration = End - Start;
            Explicit = true;
        }

        public PERIOD(DATE_TIME start, DURATION duration)
        {
            Start = start;
            Duration = duration;
            End = Start + Duration;
            Explicit = false;
        }

        public PERIOD(DateTime start, TimeZoneInfo startTimeZoneInfo, DateTime end, TimeZoneInfo endTimeZoneInfo)
            : this(new DATE_TIME(start, startTimeZoneInfo), new DATE_TIME(end, endTimeZoneInfo))
        {
        }

        public PERIOD(DateTime start, TimeZoneInfo timeZoneInfo, TimeSpan span)
            : this(new DATE_TIME(start, timeZoneInfo), new DURATION(span))
        {
        }

        public static PERIOD Parse(string value)
        {
            const string datetimePattern = @"((TZID=(\w+)?/(\w+)):)?(\d{4,})(\d{2})(\d{2})T(\d{2})(\d{2})(\d{2})Z?";
            const string durationPattern = @"(\-)?P((\d*)W)?((\d*)D)?(T((\d*)H)?((\d*)M)?((\d*)S)?)?";
            const string explicitPattern = datetimePattern + "/" + datetimePattern;
            const string startPattern = datetimePattern + "/" + durationPattern;

            var pattern = $@"^(?<periodExplicit>{explicitPattern})|(?<periodStart>{startPattern})$";
            const RegexOptions options = RegexOptions.IgnoreCase
                                         | RegexOptions.ExplicitCapture
                                         | RegexOptions.IgnorePatternWhitespace
                                         | RegexOptions.CultureInvariant
                                         | RegexOptions.Compiled;

            var regex = new Regex(pattern, options);

            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["periodExplicit"].Success)
                {
                    var periodExplicit = match.Groups["periodExplicit"].Value;
                    var parts = periodExplicit.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    var start = new DATE_TIME(parts[0]);
                    var end = new DATE_TIME(parts[1]);
                    return new PERIOD(start, end);
                }
                if (match.Groups["periodStart"].Success)
                {
                    var periodStart = match.Groups["periodStart"].Value;
                    var parts = periodStart.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    var start = new DATE_TIME(parts[0]);
                    var duration = new DURATION(parts[1]);
                    return new PERIOD(start, duration);
                }
            }

            return default(PERIOD);
        }

        public PERIOD Add(PERIOD other)
        {
            var start = Start <= other.Start ? Start : other.Start;
            var end = End >= other.End ? End : other.End;
            return Explicit
               ? new PERIOD(start, end)
               : new PERIOD(start, end - start);
        }

        public PERIOD[] Subtract(PERIOD other)
        {
            //Case A: P1.Start < P2.Start P1.Start < P2.End && AND P1.End > P2.Start AND P1.End < P2.End
            if (Start < other.Start && Start < other.End && End > other.Start && End < other.End)
                return SubtractCaseA(other);

            //Case B: P1.Start < P2.Start AND P1.Start < P2.End && AND P1.End <= P2.Start AND P1.End < P2.End
            if (Start < other.Start && Start < other.End && End == other.Start && End < other.End)
                return SubtractCaseB();

            //Case C: P1.Start < P2.Start AND P1.Start <P2.End && AND P1.End > P2.Start AND P1.End == P2.End
            if (Start < other.Start && Start < other.End && End > other.Start && End == other.End)
                return SubtractCaseC(other);

            //Case D: P1.Start == P2.Start AND && P1.Start < P2.End AND P1.End > P2.Start && P1.End > P2.End
            if (Start == other.Start && Start < other.End && End > other.Start && End > other.End)
                return SubtractCaseD(other);

            //Case E: P1.Start > P2.Start AND P1.Start < P2.End AND P1.End == P2.End
            if (Start > other.Start && Start < other.End && End == other.End)
                return SubtractCaseE(other);

            //Case F: P1.Start == P2.Start AND P1.Start < P2.End && AND P1.End < P2.Start AND P1.End > P2.End
            if (Start == other.Start && Start < other.End && End > other.Start && End > other.End)
                return SubtractCaseF(other);

            //Case G: P1.Start > P2.Start AND P1.Start < P2.End AND P1.End > P2.Start AND P1.End > P2.End
            if (Start > other.Start && Start < other.End && End > other.Start && End > other.End)
                return SubtractCaseG(other);

            //Case H: P1.Start < P2.Start AND P1.Start < P2.End && AND P1.End > P2.Start AND P1.End > P2.End
            if (Start < other.Start && Start < other.End && End > other.Start && End > other.End)
                return SubtractCaseH(other);

            //Case I: P1.Start > P2.Start AND P1.Start < P2.End && AND P1.End < P2.Start AND P1.End < P2.End
            if (Start > other.Start && Start < other.End && End < other.Start && End < other.End)
                return SubtractCaseI(other);

            //Case J: P1 == P2
            return SubtractEqualPeriods(other);
        }

        private PERIOD[] SubtractEqualPeriods(PERIOD other)
        {
            return Explicit
                ? new[] { new PERIOD(Start, other.Start) }
                : new[] { new PERIOD(Start, Start - other.Start) };
        }

        private PERIOD[] SubtractCaseI(PERIOD other)
        {
            return Explicit
                ? new[] { new PERIOD(other.Start, Start), new PERIOD(End, other.End) }
                : new[] { new PERIOD(Start, Start - other.Start), new PERIOD(End, other.End - End) };
        }

        private PERIOD[] SubtractCaseH(PERIOD other)
        {
            return Explicit
                ? new[] { new PERIOD(Start, other.Start), new PERIOD(other.End, End) }
                : new[] { new PERIOD(Start, other.Start - Start), new PERIOD(other.End, End - other.End) };
        }

        private PERIOD[] SubtractCaseG(PERIOD other)
        {
            return Explicit
                ? new[] { new PERIOD(other.Start, Start), new PERIOD(other.End, End) } //-PERIOD, +PERIOD
                : new[] { new PERIOD(other.Start, Start - other.Start), new PERIOD(other.End, other.End - End) }; //-PERIOD, +PERIOD
        }

        private PERIOD[] SubtractCaseF(PERIOD other)
        {
            return Explicit
                ? new[] { new PERIOD(other.End, End) } //-PERIOD
                : new[] { new PERIOD(other.End, End - other.End) }; //-PERIOD
        }

        private PERIOD[] SubtractCaseE(PERIOD other)
        {
            return Explicit
                ? new[] { new PERIOD(Start, other.Start) } //-PERIOD
                : new[] { new PERIOD(Start, other.Start - Start) }; //-PERIOD
        }

        private PERIOD[] SubtractCaseD(PERIOD other)
            => Explicit
            ? new[] { new PERIOD(other.End, End) }
            : new[] { new PERIOD(other.End, other.End - End) };

        private PERIOD[] SubtractCaseC(PERIOD other)
            => Explicit
            ? new[] { new PERIOD(Start, other.Start) }
            : new[] { new PERIOD(Start, other.Start - Start) };

        private PERIOD[] SubtractCaseB() => new[] { this };

        private PERIOD[] SubtractCaseA(PERIOD other)
            => Explicit
            ? new[] { new PERIOD(Start, other.Start) }
            : new[] { new PERIOD(Start, other.Start - Start) };

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is PERIOD) return CompareTo((PERIOD)obj);
            throw new ArgumentException(nameof(obj) + " is not of type " + nameof(PERIOD));
        }

        public int CompareTo(PERIOD other)
        {
            //Irrespective of ends
            if (Start < other.Start) return -1;
            if (Start > other.Start) return 1;

            //Equal starts
            if (End < other.End) return -1;
            if (End > other.End) return 1;

            //Equal starts and ends
            return 0;
        }

        public static bool operator <(PERIOD left, PERIOD right) => left.CompareTo(right) < 0;

        public static bool operator >(PERIOD left, PERIOD right) => left.CompareTo(right) > 0;

        public static bool operator <=(PERIOD left, PERIOD right) => left.CompareTo(right) <= 0;

        public static bool operator >=(PERIOD left, PERIOD right) => left.CompareTo(right) >= 0;

        public static bool operator ==(PERIOD left, PERIOD right) => left.Equals(right);

        public static bool operator !=(PERIOD left, PERIOD right) => !left.Equals(right);

        public static PERIOD operator +(PERIOD period) => period;

        public static PERIOD operator -(PERIOD period)
            => period.Explicit
            ? new PERIOD(period.End, period.Start)
            : new PERIOD(period.End, period.Start - period.End);

        public static PERIOD[] operator -(PERIOD left, PERIOD right) => left.Subtract(right);

        public static PERIOD operator +(PERIOD left, PERIOD right) => left.Add(right);

        public bool Equals(PERIOD other) => Start.Equals(other.Start)
                                            && End.Equals(other.End)
                                            && Duration.Equals(other.Duration)
                                            && Explicit == other.Explicit;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PERIOD && Equals((PERIOD)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Start.GetHashCode();
                hashCode = (hashCode * 397) ^ End.GetHashCode();
                hashCode = (hashCode * 397) ^ Duration.GetHashCode();
                hashCode = (hashCode * 397) ^ Explicit.GetHashCode();
                return hashCode;
            }
        }


        public bool CanSerialize() => true;

        public bool CanDeserialize() => true;

        public void WriteCalendar(ICalendarWriter writer)
        {
            Start.WriteCalendar(writer);
            writer.WriteValue("/");
            if (Explicit)
                End.WriteCalendar(writer);
            else Duration.WriteCalendar(writer);
        }

        public override string ToString()
        {
            return Explicit
                ? Start.ToString(CultureInfo.InvariantCulture) + "/" + End.ToString(CultureInfo.InvariantCulture)
                : Start.ToString(CultureInfo.InvariantCulture) + "/" + Duration.ToString(CultureInfo.InvariantCulture);
        }

        public void ReadCalendar(ICalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.Read())
            {
                if (inner.NodeType != NodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var period = Parse(inner.Value);
                    Start = period.Start;
                    End = period.End;
                    Duration = period.Duration;
                    Explicit = period.Explicit;
                }
            }

            inner.Close();
        }

        public TypeCode GetTypeCode() => TypeCode.Object;

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(Boolean));
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(Char));
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(SByte));
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(Byte));
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(Int16));
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(UInt16));
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(Int32));
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(UInt32));
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(Int64));
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(UInt64));
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(Single));
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(Double));
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(Decimal));
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + nameof(DateTime));
        }

        public string ToString(IFormatProvider provider) => ToString();

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(string)) return ToString();
            throw new InvalidCastException("Invalid Cast from type" + nameof(PERIOD) + "to type " + conversionType.FullName);
        }
    }
}
