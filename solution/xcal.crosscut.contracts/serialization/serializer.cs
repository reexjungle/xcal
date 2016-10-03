using reexjungle.xcal.crosscut.contracts.io;

namespace reexjungle.xcal.crosscut.contracts.serialization
{

    public interface ICalendarSerializer
    {
        void Serialize(CalendarWriter writer, object o);

        object Deserialize(CalendarReader reader);
    }

    public interface ICalendarSerializer<TValue>
    {

        void Serialize(TValue value, CalendarWriter writer);

        TValue Deserialize(CalendarReader reader);

    }

}
