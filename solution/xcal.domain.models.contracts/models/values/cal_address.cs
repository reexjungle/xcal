using System;
using System.Runtime.Serialization;
using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.serialization;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// Represents a calendar user addresss.
    /// </summary>
    [DataContract]
    public sealed class CAL_ADDRESS : Uri, IEquatable<CAL_ADDRESS>
    {
        public CAL_ADDRESS(string uriString) : base(uriString)
        {
            if (!Scheme.Equals(UriSchemeMailto, StringComparison.OrdinalIgnoreCase))
                throw new FormatException(nameof(uriString) + " is not a mailto address");
        }

        public CAL_ADDRESS(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
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
            return AbsolutePath.Equals(other.AbsolutePath, StringComparison.OrdinalIgnoreCase);
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
        public override int GetHashCode() => AbsolutePath.GetHashCode();

        /// <summary>
        /// Determines whether two specified instances of <see cref="CAL_ADDRESS"/> are equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> and <paramref name="right"/> represent the same calendar
        /// user address instance; otherwise false.
        /// </returns>
        public static bool operator ==(CAL_ADDRESS left, CAL_ADDRESS right) => Equals(left, right);

        /// <summary>
        /// Determines whether two specified instances of <see cref="CAL_ADDRESS"/> are not equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> and <paramref name="right"/> do not represent the same
        /// calendar user address instance; otherwise false.
        /// </returns>
        public static bool operator !=(CAL_ADDRESS left, CAL_ADDRESS right) => !Equals(left, right);

        public override string ToString()
        {
            return !string.IsNullOrEmpty(AbsolutePath) && !string.IsNullOrWhiteSpace(AbsolutePath)
                ? $"mailto:{AbsolutePath.Replace("mailto:", string.Empty)}"
                : string.Empty;
        }
    }
}
