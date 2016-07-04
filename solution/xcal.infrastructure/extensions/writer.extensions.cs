using System;
using System.Collections.Generic;
using System.Linq;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.serialization;

namespace reexjungle.xcal.infrastructure.extensions
{
    public static class CalendarWriterExtensions
    {
        public static CalendarWriter WriteValue<T>(this CalendarWriter writer, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize()) value.WriteCalendar(writer);
            return writer;
        }

        public static CalendarWriter WriteParameterValues<T>(this CalendarWriter writer, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            var first = values.FirstOrDefault();
            foreach (var value in values.Where(x => x.CanSerialize()))
            {
                if (!first.Equals(value)) writer.WriteComma();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        public static CalendarWriter WritePropertyValues<T>(this CalendarWriter writer, IEnumerable<T> values)
    where T : ICalendarSerializable, IEquatable<T>
        {
            var first = values.FirstOrDefault();
            foreach (var value in values.Where(x => x.CanSerialize()))
            {
                if (!first.Equals(value)) writer.WriteSemicolon();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        public static CalendarWriter WriteDQuotedParameterValue<T>(this CalendarWriter writer, T value)
    where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteDQuote();
                writer.WriteValue(value);
                writer.WriteDQuote();
            }
            return writer;
        }

        public static CalendarWriter WriteDQuotedParameterValues<T>(this CalendarWriter writer, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            var first = values.FirstOrDefault();
            foreach (var value in values)
            {
                if (!first.Equals(value)) writer.WriteComma();
                writer.WriteDQuotedParameterValue(value);
            }
            return writer;
        }

        public static CalendarWriter AppendParameterValue<T>(this CalendarWriter writer, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteComma();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        public static CalendarWriter AppendPropertyValue<T>(this CalendarWriter writer, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteSemicolon();
                value.WriteCalendar(writer);
            }
            return writer;
        }


        public static CalendarWriter WriteParameters<T>(this CalendarWriter writer, IEnumerable<T> parameters)
            where T : ICalendarSerializable, IEquatable<T>
        {
            var first = parameters.FirstOrDefault();
            foreach (var parameter in parameters.Where(x => x.CanSerialize()))
            {
                if (!first.Equals(parameter)) writer.WriteSemicolon();
                parameter.WriteCalendar(writer);
            }
            return writer;
        }

        public static CalendarWriter WriteParameter<T>(this CalendarWriter writer, string name, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.Write(name);
                writer.WriteEquals();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        public static CalendarWriter WriteParameter<T>(this CalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if(values.Any(x => x.CanSerialize()))
            {
                writer.Write(name);
                writer.WriteEquals().WriteParameterValues(values);
            }
            return writer;
        }

        public static CalendarWriter WriteParameterWithDQuotedValue<T>(this CalendarWriter writer, string name, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.Write(name);
                writer.WriteEquals().WriteDQuotedParameterValue(value);
            }
            return writer;
        }

        public static CalendarWriter WriteParameterWithDQuotedValues<T>(this CalendarWriter writer, string name, IEnumerable<T> values)
    where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.Write(name);
                writer.WriteEquals().WriteDQuotedParameterValues(values); 
            }
            return writer;
        }




        public static CalendarWriter AppendParameterValues<T>(this CalendarWriter writer, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if(values.Any(x => x.CanSerialize())) writer.WriteComma().WriteParameterValues(values);
            return writer;

        }

        public static CalendarWriter AppendParameter<T>(this CalendarWriter writer, T parameter)
            where T : ICalendarSerializable
        {
            if(parameter.CanSerialize()) writer.WriteSemicolon().WriteValue(parameter);
            return writer;
        }

        public static CalendarWriter AppendParameter<T>(this CalendarWriter writer, string name, T value)
            where T : ICalendarSerializable
        {
            if(value.CanSerialize()) writer.WriteSemicolon().WriteParameter(name, value);
            return writer;
        }

        public static CalendarWriter AppendParameter<T>(this CalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteSemicolon().Write(name);
                writer.WriteEquals().WriteParameterValues(values); 
            }
            return writer;
        }

        public static CalendarWriter AppendParameters<T>(this CalendarWriter writer, IEnumerable<T> parameters)
            where T : ICalendarSerializable, IEquatable<T>
        {
            foreach (var parameter in parameters.Where(x => x.CanSerialize()))
            {
                writer.WriteSemicolon();
                parameter.WriteCalendar(writer);
            }
            return writer;
        }



        public static CalendarWriter WriteProperty<T>(this CalendarWriter writer, string name, T value)
    where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.Write(name);
                writer.WriteColon().WriteValue(value);
            }

            return writer;
        }

        public static CalendarWriter WriteProperty<T>(this CalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.Write(name);
                writer.WriteColon().WritePropertyValues(values); 
            }
            return writer;
        }

        public static CalendarWriter WriteProperties<T>(this CalendarWriter writer, IEnumerable<T> properties)
            where T : ICalendarSerializable
        {
            var first = properties.FirstOrDefault();
            foreach (var property in properties.Where(x => x.CanSerialize()))
            {
                if (!first.Equals(property)) writer.WriteLine();
                property.WriteCalendar(writer);
            }
            return writer;
        }

        public static CalendarWriter AppendProperty<T>(this CalendarWriter writer, T property)
            where T : ICalendarSerializable
        {
            if (property.CanSerialize())
            {
                writer.WriteLine();
                property.WriteCalendar(writer); 
            }
            return writer;
        }

        public static CalendarWriter AppendProperties<T>(this CalendarWriter writer, IEnumerable<T> properties)
    where T : ICalendarSerializable
        {
            foreach (var property in properties.Where(x => x.CanSerialize()))
            {
                writer.WriteLine();
                property.WriteCalendar(writer);
            }
            return writer;
        }



        public static CalendarWriter AppendPropertyValues<T>(this CalendarWriter writer, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize())) writer.WriteSemicolon().WritePropertyValues(values);
            return writer;
        }

        public static CalendarWriter AppendProperty<T>(this CalendarWriter writer, string name, T value)
    where T : ICalendarSerializable, IEquatable<T>
        {
            if (value.CanSerialize())
            {
                writer.WriteLine();
                writer.Write(name);
                writer.WriteColon().WriteValue(value); 
            }

            return writer;
        }

        public static CalendarWriter AppendProperty<T>(this CalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteLine();
                writer.Write(name);
                writer.WriteColon().WriteParameterValues(values); 
            }
            return writer;
        }
    }
}
