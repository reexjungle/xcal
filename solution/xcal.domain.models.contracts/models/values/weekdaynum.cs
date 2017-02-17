using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.serialization;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    [DataContract]
    public struct WEEKDAYNUM : IComparable, IComparable<WEEKDAYNUM>, IEquatable<WEEKDAYNUM>, IConvertible, ICalendarSerializable
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
            var weekdaynum = Parse(value);
            NthOccurrence = weekdaynum.NthOccurrence;
            Weekday = weekdaynum.Weekday;
        }

        private static WEEKDAYNUM Parse(string value)
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

            return new WEEKDAYNUM(ordweek, weekday);
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
            if (NthOccurrence != 0)
            {
                writer.WriteValue(NthOccurrence < 0
                    ? "-" + (uint)NthOccurrence + " " + Weekday
                    : "+" + (uint)NthOccurrence + " " + Weekday);
            }
            else writer.WriteValue(Weekday.ToString());
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
                    var weekdaynum = Parse(inner.Value);
                    NthOccurrence = weekdaynum.NthOccurrence;
                    Weekday = weekdaynum.Weekday;
                }
            }

            inner.Close();
        }

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

        /// <summary>
        /// Returns the <see cref="T:System.TypeCode"/> for this instance.
        /// </summary>
        /// <returns>
        /// The enumerated constant that is the <see cref="T:System.TypeCode"/> of the class or value
        /// type that implements this interface.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public TypeCode GetTypeCode() => TypeCode.Object;

        /// <summary>
        /// Converts the value of this instance to an equivalent Boolean value using the specified
        /// culture-specific formatting information.
        /// </summary>
        /// <returns>A Boolean value equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(Boolean));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent Unicode character using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>A Unicode character equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(Char));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit signed integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(SByte));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit unsigned integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(Byte));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit signed integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(Int16));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit unsigned integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(UInt16));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit signed integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(Int32));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit unsigned integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(UInt32));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit signed integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(Int64));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit unsigned integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(UInt64));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent single-precision floating-point
        /// number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A single-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(Single));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent double-precision floating-point
        /// number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A double-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(Double));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.Decimal"/>
        /// number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Decimal"/> number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(Decimal));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.DateTime"/>
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.DateTime"/> instance equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + nameof(DateTime));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.String"/> using
        /// the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> instance equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(IFormatProvider provider) => ToString();

        /// <summary>
        /// Converts the value of this instance to an <see cref="T:System.Object"/> of the specified
        /// <see cref="T:System.Type"/> that has an equivalent value, using the specified
        /// culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> instance of type <paramref name="conversionType"/> whose
        /// value is equivalent to the value of this instance.
        /// </returns>
        /// <param name="conversionType">
        /// The <see cref="T:System.Type"/> to which the value of this instance is converted.
        /// </param>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(string)) return ToString();
            throw new InvalidCastException("Invalid Cast from type" + nameof(WEEKDAYNUM) + "to type " + conversionType.FullName);
        }
    }
}
