using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.models.values;
using reexjungle.xcal.core.domain.contracts.serialization;
using System;
using System.Runtime.Serialization;

namespace reexjungle.xcal.core.domain.concretes.models.values
{
    /// <summary>
    /// Represents a calendar user addresss.
    /// </summary>
    [DataContract]
    public class CAL_ADDRESS : ICAL_ADDRESS, IEquatable<CAL_ADDRESS>, ICalendarSerializable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CAL_ADDRESS"/> class.
        /// </summary>
        protected CAL_ADDRESS()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CAL_ADDRESS"/> class with string
        /// representation of a <see cref="Uri"/>.
        /// </summary>
        /// <param name="value">
        /// The string representation of the <see cref="Uri"/> that is used to initialize the
        /// calkendar user address
        /// </param>
        public CAL_ADDRESS(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (!Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
                throw new FormatException(nameof(value) + " is not well formed Uri");

            var formatted = $"mailto:{value.Replace("mailto:", string.Empty)}";
            Value = new Uri(formatted);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CAL_ADDRESS"/> class with a mailto URI as
        /// defined by RFC2638.
        /// </summary>
        /// <param name="value">The mail URI to initialize this instance.</param>
        public CAL_ADDRESS(Uri value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (!value.Scheme.Equals(Uri.UriSchemeMailto, StringComparison.OrdinalIgnoreCase))
                throw new FormatException(nameof(value) + " must be a mailto URI");
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CAL_ADDRESS"/> class with an instance that
        /// implements the <see cref="ICAL_ADDRESS"/> interface.
        /// </summary>
        /// <param name="other">The instance that implements</param>
        public CAL_ADDRESS(ICAL_ADDRESS other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other.Value != null && !other.Value.Scheme.Equals(Uri.UriSchemeMailto, StringComparison.OrdinalIgnoreCase))
                throw new FormatException(nameof(other.Value) + " must be a mailto URI");

            Value = other.Value != null ? new Uri(other.Value.ToString()) : other.Value;
        }

        /// <summary>
        /// Gets the value of the calendar user address.
        /// </summary>
        [DataMember]
        public Uri Value { get; private set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(CAL_ADDRESS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Value, other.Value);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CAL_ADDRESS)obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public static bool operator ==(CAL_ADDRESS left, CAL_ADDRESS right) => Equals(left, right);

        public static bool operator !=(CAL_ADDRESS left, CAL_ADDRESS right) => !Equals(left, right);

        /// <summary>
        /// Converts this calendar address into an equivalent <see cref="Uri"/> instance.
        /// </summary>
        /// <param name="address">The calendar address that is to be converted to its equivalent <see cref="Uri"/> representation.</param>
        public static explicit operator Uri(CAL_ADDRESS address) => address.AsUri();

        /// <summary>
        /// Can the object be converted to its iCalendar representation?
        /// </summary>
        /// <returns>
        /// True if the object can be serialized to its iCalendar representation, otherwise false.
        /// </returns>
        public bool CanSerialize() => true;

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>
        /// True if the object can be deserialized from its iCalendar representation, otherwise false.
        /// </returns>
        public bool CanDeserialize() => true;

        /// <summary>
        /// Converts an object into its iCalendar representation.
        /// </summary>
        /// <param name="writer">The iCalendar writer used to serialize the object.</param>
        public void WriteCalendar(ICalendarWriter writer)
        {
            writer.WriteValue(Value.ToString());
        }

        /// <summary>
        /// Generates an object from its iCalendar representation.
        /// </summary>
        /// <param name="reader">
        /// The iCalendar reader used to deserialize data into the iCalendar object.
        /// </param>
        /// <returns>True if the deserialization operation was successful; otherwise false.</returns>
        public void ReadCalendar(ICalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.Read())
            {
                if (inner.NodeType != NodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    Uri uri;
                    if (Uri.TryCreate(inner.Value, UriKind.RelativeOrAbsolute, out uri))
                    {
                        Value = uri;
                    }
                }
            }
        }

        /// <summary>
        /// Converts this calendar address into an equivalent <see cref="Uri"/> instance.
        /// </summary>
        /// <returns>The equivalent <see cref="Uri"/> respresentation of this calendar address instance.</returns>
        public Uri AsUri() => Value;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => Value != null ? Value.ToString() : base.ToString();
    }
}