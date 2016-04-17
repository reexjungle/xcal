using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using reexjungle.xcal.infrastructure.serialization;

namespace reexjungle.xcal.infrastructure.contracts
{

    public interface ICalendarSerializer
    {
        void Serialize(iCalWriter writer, object o);

        object Deserialize(iCalReader reader);
    }

    public interface ICalendarSerializer<TValue>
    {
        void Serialize(iCalWriter writer, TValue value);

        TValue Deserialize(iCalReader reader);

    }

}
