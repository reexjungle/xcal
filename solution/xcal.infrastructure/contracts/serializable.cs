using reexjungle.xcal.infrastructure.serialization;

namespace reexjungle.xcal.infrastructure.contracts
{
    public interface ICalendarSerializable
    {
        /// <summary>
        /// Converts an object into its iCalendar representation.
        /// </summary>
        /// <param name="writer"></param>
        void WriteCalendar(CalendarWriter writer);

        /// <summary>
        /// Generates an object from its iCalendar representation.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>True if the generation operation was successful; otherwise false.</returns>
        void ReadCalendar(CalendarReader reader);
    }

}
