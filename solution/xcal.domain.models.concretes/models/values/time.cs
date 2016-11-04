namespace reexjungle.xcal.core.domain.concretes.models.values
{
    public struct TIME : ITIME, ITIME<TIME>
    {
        /// <summary>
        /// Gets the 2-digit representation of an hour
        /// </summary>
        public uint HOUR { get; }

        /// <summary>
        /// Gets or sets the 2-digit representatio of a minute
        /// </summary>
        public uint MINUTE { get; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a second
        /// </summary>
        public uint SECOND { get; }

        /// <summary>
        /// Gets the form in which the <see cref="TIME"/> instance is expressed. 
        /// </summary>
        public TIME_FORM Form { get; }

        /// <summary>
        /// Gets the time zone identifier.
        /// </summary>
        public ITZID TimeZoneId { get; }

        /// <summary>
        /// Adds the specified number of seconds to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="value">The number of seconds to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of seconds to the value of this instance.</returns>
        public TIME AddSeconds(double value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds the specified number of minutes to the value of the <see cref="TIME"/> instance. 
        /// </summary>
        /// <param name="value">The number of mintes to add.</param>
        /// <returns>A new instance of type <see cref="TIME"/> that adds the specified number of minutes to the value of this instance.</returns>
        public TIME AddMinutes(double value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds the specified number of hours to the value of the <see cref="TIME"/> instance. 
        /// </summary>
        /// <param name="value">The number of hours to add.</param>
        /// <returns>A new instance of type <see cref="TIME"/> that adds the specified number of hours to the value of this instance.</returns>
        public TIME AddHours(double value)
        {
            throw new System.NotImplementedException();
        }
    }
}
