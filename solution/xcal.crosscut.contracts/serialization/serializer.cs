using reexjungle.xcal.core.domain.models.contracts.io;

namespace reexjungle.xcal.core.domain.models.contracts.serialization
{

    public interface ICalendarSerializer
    {
        void Serialize(ICalendarWriter writer, object o);

        object Deserialize(ICalendarReader reader);
    }

    public interface ICalendarSerializer<TValue>
    {

        void Serialize(TValue value, ICalendarWriter writer);

        TValue Deserialize(ICalendarReader reader);

    }

}
