using System.Collections.Generic;

namespace reexjungle.xcal.infrastructure.serialization
{
    public class CalendarFragment
    {
        public BeginCalendarNode Begin { get; }

        public EndCalendarNode End { get;  }

        public List<ValueCalendarNode> Values { get; }

        public List<ParameterCalendarNode> Parameters { get; }

        public List<PropertyCalendarNode> Properties { get; }


        public CalendarFragment(
            BeginCalendarNode begin, 
            PropertyCalendarNode properties, 
            IEnumerable<ParameterCalendarNode> parameters, 
            IEnumerable<ValueCalendarNode> values, 
            EndCalendarNode end)
        {
            //Begin = begin;
            //Properties = properties;
            //Parameters = parameters;
            //Values = values;
            //End = end;
        }
    }
}
