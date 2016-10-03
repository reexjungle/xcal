using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using xcal.domain.contracts.core.values;

namespace xcal.domain.concretes.core.values
{

    [DataContract]
    public class CAL_ADDRESS : ICAL_ADDRESS, IEquatable<CAL_ADDRESS>
    {
        protected CAL_ADDRESS()
        {
        }

        public CAL_ADDRESS(string uriString, bool emailable = true)
        {
            if (uriString == null) throw new ArgumentNullException(nameof(uriString));
            if (!Uri.IsWellFormedUriString(uriString, UriKind.RelativeOrAbsolute))
                throw new FormatException(nameof(uriString) + "is not well formed Uri");

            var formatted = emailable
                ? $"mailto:{uriString.Replace("mailto:", string.Empty)}"
                : uriString;

            Value = new Uri(formatted);
        }

        public CAL_ADDRESS(Uri value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        [DataMember]
        public Uri Value { get; protected set; }

        public bool Equals(CAL_ADDRESS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CAL_ADDRESS)obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public static bool operator ==(CAL_ADDRESS left, CAL_ADDRESS right) => Equals(left, right);

        public static bool operator !=(CAL_ADDRESS left, CAL_ADDRESS right) => !Equals(left, right);

        private sealed class CAL_ADDRESS_EqualityComparer : IEqualityComparer<ICAL_ADDRESS>
        {
            public bool Equals(ICAL_ADDRESS x, ICAL_ADDRESS y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                return x.GetType() == y.GetType() && Equals(x.Value, y.Value);
            }

            public int GetHashCode(ICAL_ADDRESS obj) => obj.Value != null ? obj.Value.GetHashCode() : 0;
        }

        public static IEqualityComparer<ICAL_ADDRESS> Comparer { get; } = new CAL_ADDRESS_EqualityComparer();
    }
}
