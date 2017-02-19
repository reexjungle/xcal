using System;
using System.Runtime.Serialization;
using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.serialization;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    public sealed class MailtoUri : Uri
    {
        public MailtoUri(string uriString) : base(uriString)
        {
            if (uriString == null) throw new ArgumentNullException(nameof(uriString));
            if (!Uri.IsWellFormedUriString(uriString, UriKind.RelativeOrAbsolute))
                throw new FormatException(nameof(uriString) + " is not well formed Uri");
        }

        public MailtoUri(string uriString, UriKind uriKind) : base(uriString, uriKind)
        {
        }

        public MailtoUri(Uri baseUri, string relativeUri) : base(baseUri, relativeUri)
        {
        }

        public MailtoUri(Uri baseUri, Uri relativeUri) : base(baseUri, relativeUri)
        {
        }

        public MailtoUri(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }

    /// <summary>
    /// Represents a calendar user addresss.
    /// </summary>
    [DataContract]
    public class CAL_ADDRESS : IEquatable<CAL_ADDRESS>
    {
        /// <summary>
        /// Gets the value of the calendar user address.
        /// </summary>
        [DataMember]
        public Uri Value { get; private set; }

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
        /// implements the <see cref="CAL_ADDRESS"/> interface.
        /// </summary>
        /// <param name="other">The instance that implements</param>
        public CAL_ADDRESS(CAL_ADDRESS other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other.Value != null && !other.Value.Scheme.Equals(Uri.UriSchemeMailto, StringComparison.OrdinalIgnoreCase))
                throw new FormatException(nameof(other.Value) + " must be a mailto URI");

            Value = other.Value != null ? new Uri(other.Value.ToString()) : other.Value;
        }

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

        /// <summary>
        /// Determines whether two specified instances of <see cref="CAL_ADDRESS"/> are equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> and <paramref name="right"/> represent the same calendar user address
        /// instance; otherwise false.
        /// </returns>
        public static bool operator ==(CAL_ADDRESS left, CAL_ADDRESS right) => Equals(left, right);

        /// <summary>
        /// Determines whether two specified instances of <see cref="CAL_ADDRESS"/> are not equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> and <paramref name="right"/> do not represent the same calendar user address
        /// instance; otherwise false.
        /// </returns>
        public static bool operator !=(CAL_ADDRESS left, CAL_ADDRESS right) => !Equals(left, right);

        /// <summary>
        /// Converts this calendar address into an equivalent <see cref="Uri"/> instance.
        /// </summary>
        /// <param name="address">The calendar address that is to be converted to its equivalent <see cref="Uri"/> representation.</param>
        public static explicit operator Uri(CAL_ADDRESS address) => address.AsUri();

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
