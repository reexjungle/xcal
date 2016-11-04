using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;

namespace reexjungle.xcal.core.domain.contracts.serialization
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
