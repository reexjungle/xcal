using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.serialization;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.concretes.models.properties
{
    /// <summary>
    /// Time Zone ID class
    /// </summary>
    [DataContract]
    public class TZID : ITZID, IEquatable<TZID>, IComparable<TZID>, ICalendarSerializable
    {
        private string prefix;
        private bool globallyUnique;

        /// <summary>
        /// Gets or sets the identifier for the time zone definition for a time component
        /// </summary>
        [DataMember]
        public string Prefix
        {
            get { return prefix; }
            set
            {
                prefix = value;
                globallyUnique = string.IsNullOrWhiteSpace(prefix);
            }
        }


        /// <summary>
        /// Gets the identifier for the time zone definition for a time component.
        /// </summary>
        [DataMember]
        public string Suffix { get; private set; }

        /// <summary>
        /// Specifies whether the time-zone prefix should be included.
        /// </summary>
        [DataMember]
        public bool GloballyUnique
        {
            get { return globallyUnique; }
            private set
            {
                globallyUnique = value;
                if (globallyUnique) prefix = null;
            }
        }

        protected TZID()
        {
            Prefix = string.Empty;
            Suffix = "GMT";
        }

        public TZID(ITZID other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            Prefix = other.Prefix;
            Suffix = other.Suffix;
        }

        public TZID(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

            var pattern = @"^TZID=(?<prefix>\w+)?/(?<suffix>\w+)$";

            var options = RegexOptions.IgnoreCase
                | RegexOptions.ExplicitCapture
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.CultureInvariant
                | RegexOptions.Compiled;

            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");

            GloballyUnique = true;
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["prefix"].Success)
                {
                    GloballyUnique = false;
                    Prefix = match.Groups["prefix"].Value;
                }
                Suffix = match.Groups["suffix"].Value;
            }
        }

        public TZID(string prefix, string suffix)
        {
            if (string.IsNullOrEmpty(suffix))
                throw new ArgumentException("suffix");

            Prefix = prefix;
            Suffix = suffix;
        }

        public bool Equals(TZID other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Suffix, other.Suffix) && GloballyUnique == other.GloballyUnique && string.Equals(Prefix, other.Prefix);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TZID)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Suffix != null ? Suffix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ GloballyUnique.GetHashCode();
                hashCode = (hashCode * 397) ^ (Prefix != null ? Prefix.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(TZID left, TZID right) => Equals(left, right);

        public static bool operator !=(TZID left, TZID right) => !Equals(left, right);

        public int CompareTo(TZID other) => string.Compare(Suffix, other.Suffix, StringComparison.OrdinalIgnoreCase);

        public static bool operator <(TZID a, TZID b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(TZID a, TZID b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.CompareTo(b) > 0;
        }

        public bool CanDeserialize()
        {
            throw new NotImplementedException();
        }

        public void WriteCalendar(ICalendarWriter writer)
        {
            writer.WriteParameter("TZID", GloballyUnique ? $"/{Suffix}" : $"{Prefix}/{Suffix}");
        }

        public void ReadCalendar(ICalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => !string.IsNullOrEmpty(Suffix) && !string.IsNullOrWhiteSpace(Suffix);
    }
}
