using System;
using System.Text.RegularExpressions;
using NodaTime;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    public struct PERIOD : IComparable, IComparable<PERIOD>, IEquatable<PERIOD>
    {
        public static readonly PERIOD Zero = new PERIOD();

        public DATE_TIME Start { get; }

        public DATE_TIME End { get; }

        public DURATION Duration { get; }

        public bool Explicit { get; }

        public PERIOD(DATE_TIME start, DATE_TIME end)
        {
            Start = start;
            End = end;
            Duration = End - Start;
            Explicit = true;
        }

        public PERIOD(DATE start, DATE end) : this(new DATE_TIME(start), new DATE_TIME(end))
        {
        }

        public PERIOD(DateTime start, DateTime end)
            : this(new DATE_TIME(start), new DATE_TIME(end))
        {
        }

        public PERIOD(DateTime start, TimeSpan span)
            : this(new DATE_TIME(start), new DURATION(span))
        {
        }

        public PERIOD(LocalDate start, LocalDate end) : this(new DATE_TIME(start), new DATE_TIME(end))
        {
        }

        public PERIOD(LocalDateTime start, LocalDateTime end) : this(new DATE_TIME(start), new DATE_TIME(end))
        {
        }

        public PERIOD(ZonedDateTime start, ZonedDateTime end) : this(new DATE_TIME(start), new DATE_TIME(end))
        {
        }

        public PERIOD(DATE_TIME start, DURATION duration)
        {
            Start = start;
            Duration = duration;
            End = Start + Duration;
            Explicit = false;
        }

        public PERIOD(DATE start, DURATION duration) : this(new DATE_TIME(start), duration)
        {
        }

        public PERIOD(LocalDate start, DURATION duration) : this(new DATE_TIME(start), duration)
        {
        }

        public PERIOD(LocalDateTime start, DURATION duration) : this(new DATE_TIME(start), duration)
        {
        }

        public PERIOD(ZonedDateTime start, DURATION duration) : this(new DATE_TIME(start), duration)
        {
        }

        public PERIOD(DATE_TIME start, Duration duration) : this(start, new DURATION(duration))
        {
        }

        public PERIOD(DATE start, Duration duration) : this(new DATE_TIME(start), new DURATION(duration))
        {
        }

        public PERIOD(LocalDate start, Duration duration) : this(new DATE_TIME(start), new DURATION(duration))
        {
        }

        public PERIOD(LocalDateTime start, Duration duration) : this(new DATE_TIME(start), new DURATION(duration))
        {
        }

        public PERIOD(ZonedDateTime start, Duration duration) : this(new DATE_TIME(start), new DURATION(duration))
        {
        }

        public PERIOD(string value)
        {
            Start = default(DATE_TIME);
            End = default(DATE_TIME);
            Duration = End - Start;
            Explicit = false;
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
                    Start = new DATE_TIME(parts[0]);
                    End = new DATE_TIME(parts[1]);
                    Duration = End - Start;
                    Explicit = true;
                    break;
                }
                if (match.Groups["periodStart"].Success)
                {
                    var periodStart = match.Groups["periodStart"].Value;
                    var parts = periodStart.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    Start = new DATE_TIME(parts[0]);
                    Duration = new DURATION(parts[1]);
                    Duration = End - Start;
                    break;
                }
            }
        }

        public PERIOD Add(PERIOD other)
        {
            var start = DATE_TIME.Min(Start, other.Start);
            var end = DATE_TIME.Max(End, other.End);
            return Explicit
               ? new PERIOD(start, end)
               : new PERIOD(start, end - start);
        }

        public PERIOD Subtract(PERIOD other)
        {

            // ________ ________
            if (Start < other.Start && Start < other.End && End <= other.Start && End < other.End)
                return this;

            // ________
            //    ________
            if (Start <= other.Start && Start < other.End && End >= other.Start && End <= other.End)
                return new PERIOD(Start, other.Start) + new PERIOD(End, other.End) + other.Negate();

            //     _______
            // ________
            if (Start >= other.Start && Start <= other.End && End > other.Start && End >= other.End)
                return new PERIOD(other.End, End) + new PERIOD(other.Start, Start) + other.Negate();

            // ________
            //   ____
            if (Start <= other.Start && Start < other.End && End > other.Start && End >= other.End)
                return new PERIOD(Start, other.Start) + other.Negate();

            //   ____
            // ________
            if (Start >= other.Start && Start < other.End && End > other.Start && End <= other.End)
                return this +  other.Negate();

            return Zero;
        }

        public PERIOD Negate() => Explicit
            ? new PERIOD(End, Start)
            : new PERIOD(End, Start - End);

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

        public static PERIOD operator -(PERIOD period) => period.Negate();

        public static PERIOD operator -(PERIOD left, PERIOD right) => left.Subtract(right);

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

        public override string ToString()
        {
            return Explicit
                ? Start + "/" + End
                : Start + "/" + Duration;
        }
    }
}
