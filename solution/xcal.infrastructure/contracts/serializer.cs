﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using reexjungle.xcal.infrastructure.serialization;

namespace reexjungle.xcal.infrastructure.contracts
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
