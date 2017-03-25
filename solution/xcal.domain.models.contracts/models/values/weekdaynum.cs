using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    [DataContract]
    public struct WEEKDAYNUM : IComparable, IComparable<WEEKDAYNUM>, IEquatable<WEEKDAYNUM>
    {
        /// <summary>
        /// Gets the nth occurence of the day within the MONTHLY or YEARLY recurrence rule
        /// </summary>
        [DataMember]
        public int NthOccurrence { get; private set; }

        /// <summary>
        /// Gets the weekday
        /// </summary>
        [DataMember]
        public WEEKDAY Weekday { get; private set; }

        public WEEKDAYNUM(int nthOccurrence, WEEKDAY weekday)
        {
            NthOccurrence = nthOccurrence;
            Weekday = weekday;
        }

        public WEEKDAYNUM(WEEKDAY weekday) : this(0, weekday)
        {
        }

        public WEEKDAYNUM(string value)
        {
            var ordweek = 0;
            var weekday = WEEKDAY.SU;

            const string pattern = @"^((?<minus>\-)? <?ordwk>\d{1,2})?(?<weekday>(SU|MO|TU|WE|TH|FR|SA)$";
            const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Compiled;
            var mulitplier = 1;
            var regex = new Regex(pattern, options);
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["minus"].Success && match.Groups["minus"].Value == "-")
                    mulitplier *= -1;
                if (match.Groups["ordwk"].Success)
                    ordweek = mulitplier * int.Parse(match.Groups["ordwk"].Value);
                if (match.Groups["weekday"].Success)
                    weekday = (WEEKDAY)Enum.Parse(typeof(WEEKDAY), match.Groups["weekday"].Value);
            }
            NthOccurrence = ordweek;
            Weekday = weekday;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is WEEKDAYNUM) return CompareTo((WEEKDAYNUM)obj);
            throw new ArgumentException(nameof(obj) + " is not of type " + nameof(WEEKDAYNUM));
        }

        public int CompareTo(WEEKDAYNUM other)
        {
            if (NthOccurrence == 0 && other.NthOccurrence == 0) return Weekday.CompareTo(other.Weekday);
            if (NthOccurrence < other.NthOccurrence) return -1;
            if (NthOccurrence > other.NthOccurrence) return 1;
            return Weekday.CompareTo(other.Weekday);
        }

        public bool Equals(WEEKDAYNUM other) => NthOccurrence == other.NthOccurrence && Weekday == other.Weekday;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is WEEKDAYNUM && Equals((WEEKDAYNUM)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (NthOccurrence * 397) ^ (int)Weekday;
            }
        }

        public static bool operator ==(WEEKDAYNUM left, WEEKDAYNUM right) => left.Equals(right);

        public static bool operator !=(WEEKDAYNUM left, WEEKDAYNUM right) => !left.Equals(right);

        public static bool operator <(WEEKDAYNUM left, WEEKDAYNUM right) => left.CompareTo(right) < 0;

        public static bool operator >(WEEKDAYNUM left, WEEKDAYNUM right) => left.CompareTo(right) > 0;

        public static bool operator <=(WEEKDAYNUM left, WEEKDAYNUM right) => left.CompareTo(right) <= 0;

        public static bool operator >=(WEEKDAYNUM left, WEEKDAYNUM right) => left.CompareTo(right) >= 0;


        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> containing a fully qualified type name.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            if (NthOccurrence != 0)
            {
                return NthOccurrence < 0
                    ? "-" + (uint)NthOccurrence + " " + Weekday
                    : "+" + (uint)NthOccurrence + " " + Weekday;
            }
            return Weekday.ToString();
        }

    }
}
