using reexjungle.xcal.core.domain.contracts.models.parameters;
using System;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// Specifies a point in time, typically expressed as time of day.
    /// <para /> Format 1 (Local Time): [HHMMSS]
    /// <para />Format 2 (UTC Time): [HHMMSS]Z
    /// <para />where HH is 2-digit hour, MM is 2-digit minute, SS is 2-digit second and Z is UTC zone indicator.
    /// </summary>
    public interface ITIME
    {
        /// <summary>
        /// Gets the 2-digit representation of an hour.
        /// </summary>
        uint HOUR { get; }

        /// <summary>
        /// Gets the 2-digit representatio of a minute.
        /// </summary>
        uint MINUTE { get; }

        /// <summary>
        /// Gets the 2-digit representation of a second.
        /// </summary>
        uint SECOND { get; }

        /// <summary>
        /// Gets the form in which the <see cref="ITIME"/> instance is expressed. 
        /// </summary>
        TIME_FORM Form { get; }

        /// <summary>
        /// Gets the time zone identifier.
        /// </summary>
        ITZID TimeZoneId { get; }

    }

    /// <summary>
    /// Extends the <see cref="ITIME"/> interface for a type that implements the <see cref="ITIME"/> interface.
    /// </summary>
    /// <typeparam name="T">The type that implements the <see cref="ITIME"/> interface.</typeparam>
    public interface ITIME<out T, out TDURATION> 
        where T : ITIME
        where TDURATION: IDURATION
    {
        /// <summary>
        /// Adds the specified number of seconds to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="value">The number of seconds to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of seconds to the value of this instance.</returns>
        T AddSeconds(double value);

        /// <summary>
        /// Adds the specified number of minutes to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="value">The number of mintes to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of minutes to the value of this instance.</returns>
        T AddMinutes(double value);

        /// <summary>
        /// Adds the specified number of hours to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="value">The number of hours to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of hours to the value of this instance.</returns>
        T AddHours(double value);

        /// <summary>
        /// Converts this time instance to its equivalent <see cref="DateTime"/> representation.
        /// </summary>
        /// <returns>The equivalent <see cref="DateTime"/> respresentation of this date instance.</returns>
        DateTime AsDateTime();

        /// <summary>
        /// Converts this time instance to its equivalent <see cref="DateTimeOffset"/> representation.
        /// </summary>
        /// <param name="func">Function to determine the offset from the time zone reference.</param>
        /// <returns>The equivalent <see cref="DateTimeOffset"/> respresentation of this date instance.</returns>
        DateTimeOffset AsDateTimeOffset(Func<ITZID, IUTC_OFFSET> func = null);

        T Add(IDURATION duration);

        T Subtract(IDURATION duration);

        TDURATION Subtract(ITIME end);
    }
}
