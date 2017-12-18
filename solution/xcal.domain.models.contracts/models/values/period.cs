using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NodaTime;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    public struct PERIOD : IComparable, IComparable<PERIOD>, IEquatable<PERIOD>
    {
        public static readonly PERIOD Zero = new PERIOD(DATE_TIME.Zero, DATE_TIME.Zero);

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
            Explicit = true;
            Start = DATE_TIME.Zero;
            End = DATE_TIME.Zero;
            Duration = End - Start;

            var tokens = value.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 2)
            {
                Start = new DATE_TIME(tokens[0]);
                End = new DATE_TIME(tokens[1]);
                if (Start != DATE_TIME.Zero && End != DATE_TIME.Zero)
                {
                    Duration = End - Start;
                }
                else if (Start != DATE_TIME.Zero && End == DATE_TIME.Zero)
                {
                    Duration = new DURATION(tokens[1]);
                    End = Start + Duration;
                    Explicit = false;
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

        public PERIOD[] Add(IEnumerable<PERIOD> others)
        {
            var sums = new List<PERIOD>();
            var items = others as IList<PERIOD> ?? others.ToList();
            for (int i = 0; i < items.Count; i++)  sums.Add(items[i]);
            return sums.ToArray();
        }

        public PERIOD[] Subtract(PERIOD other)
        {

            //equal periods
            if (this == other) return new PERIOD[] { };

            //non-overlapping periods
            if (Start < other.Start && Start < other.End && End < other.Start && End < other.End)
                return new[] { this, - other };

            //overlapping periods with equal edges
            if (Start == other.Start) return new[] { new PERIOD(DATE_TIME.Min(End, other.End), DATE_TIME.Max(End, other.End)) };
            if (End == other.End) return new[] { new PERIOD(DATE_TIME.Min(Start, other.Start), DATE_TIME.Max(Start, other.Start)) };

            //overlapping periods with non-overlapping edges
            return new[]
            {
                new PERIOD(DATE_TIME.Min(Start, other.Start), DATE_TIME.Max(Start, other.Start)),
                new PERIOD(DATE_TIME.Max(End, other.End), DATE_TIME.Min(End, other.End))
            };

        }

        public PERIOD[] Subtract(IEnumerable<PERIOD> others)
        {
            var differences = new List<PERIOD>();
            var items = others as IList<PERIOD> ?? others.ToList();
            for (int i = 0; i < items.Count; i++) differences.AddRange(this-items[i]);
            return differences.ToArray();
        }


        /// <summary>
        /// Returns the smaller of two periods.
        /// </summary>
        /// <param name="left">The first period to compare.</param>
        /// <param name="right">The second period to compare.</param>
        /// <returns><paramref name="left"/> if it is later than <paramref name="right"/>; otherwise <paramref name="right."/>.</returns>
        public static PERIOD Max(PERIOD left, PERIOD right) => left > right ? left : right;

        /// <summary>
        /// Returns the bigger of two periods.
        /// </summary>
        /// <param name="left">The first period to compare. </param>
        /// <param name="right">The second period to compare.</param>
        /// <returns><paramref name="left"/> if it is earlier than <paramref name="right"/>; otherwise <paramref name="right."/>.</returns>
        public static PERIOD Min(PERIOD left, PERIOD right) => left < right ? left : right;

        /// <summary>
        /// Returns the later of two periods.
        /// </summary>
        /// <param name="left">The first period to compare.</param>
        /// <param name="right">The second period to compare.</param>
        /// <returns><paramref name="left"/> if it is later than <paramref name="right"/>; otherwise <paramref name="right."/>.</returns>
        public static PERIOD Latest(PERIOD left, PERIOD right) => left.Start > right.Start ? left : right;

        /// <summary>
        /// Returns the earlier of two periods.
        /// </summary>
        /// <param name="left">The first period to compare. </param>
        /// <param name="right">The second period to compare.</param>
        /// <returns><paramref name="left"/> if it is earlier than <paramref name="right"/>; otherwise <paramref name="right."/>.</returns>
        public static PERIOD Earliest(PERIOD left, PERIOD right) => left.Start < right.Start ? left : right;

        public PERIOD Negate() => new PERIOD(End, Start); 

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is PERIOD) return CompareTo((PERIOD)obj);
            throw new ArgumentException(nameof(obj) + " is not of type " + nameof(PERIOD));
        }

        public int CompareTo(PERIOD other)
        {
            if (Duration < other.Duration) return -1;
            if (Duration > other.Duration) return 1;

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

        public static PERIOD operator -(PERIOD other) => other.Negate();


        public static PERIOD[] operator -(PERIOD left, PERIOD right) => left.Subtract(right);

        public static PERIOD[] operator -(PERIOD left, IEnumerable<PERIOD> right) => left.Subtract(right);

        public static PERIOD operator +(PERIOD left, PERIOD right) => left.Add(right);

        public static PERIOD[] operator +(PERIOD left, IEnumerable<PERIOD> right) => left.Add(right);

        public bool Equals(PERIOD other) => Start.Equals(other.Start)
                                            && Duration.Equals(other.Duration);

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
