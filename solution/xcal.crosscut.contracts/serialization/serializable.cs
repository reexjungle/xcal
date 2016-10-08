using reexjungle.xcal.crosscut.contracts.io;

namespace reexjungle.xcal.crosscut.contracts.serialization
{
 
    public interface ICalendarSerializable
    {
        /// <summary>
        /// Can the object be converted to its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be serialized to its iCalendar representation, otherwise false.</returns>
        bool CanSerialize();

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be deserialized from its iCalendar representation, otherwise false.</returns>
        bool CanDeserialize();

        /// <summary>
        /// Converts an object into its iCalendar representation.
        /// </summary>
        /// <param name="writer">The iCalendar writer used to serialize the object.</param>
        void WriteCalendar(CalendarWriter writer);

        /// <summary>
        /// Generates an object from its iCalendar representation.
        /// </summary>
        /// <param name="reader">The iCalendar reader used to deserialize data into the iCalendar object.</param>
        /// <returns>True if the deserialization operation was successful; otherwise false.</returns>
        void ReadCalendar(CalendarReader reader);

    }

}
