using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace xcal.infrastructure.io.concretes.extensions
{
    public static class CalendarWriterExtensions
    {
        public static ICalendarWriter WriteValue<T>(this ICalendarWriter writer, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize()) value.WriteCalendar(writer);
            return writer;
        }

        public static ICalendarWriter WriteParameterValues<T>(this ICalendarWriter writer, IEnumerable<T> values)
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

        public static ICalendarWriter WritePropertyValues<T>(this ICalendarWriter writer, IEnumerable<T> values)
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

        public static ICalendarWriter WriteDQuotedParameterValue<T>(this ICalendarWriter writer, T value)
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

        public static ICalendarWriter WriteDQuotedParameterValues<T>(this ICalendarWriter writer, IEnumerable<T> values)
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

        public static ICalendarWriter AppendParameterValue<T>(this ICalendarWriter writer, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteComma();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        public static ICalendarWriter AppendPropertyValue<T>(this ICalendarWriter writer, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteSemicolon();
                value.WriteCalendar(writer);
            }
            return writer;
        }


        public static ICalendarWriter WriteParameters<T>(this ICalendarWriter writer, IEnumerable<T> parameters)
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

        public static ICalendarWriter WriteParameter<T>(this ICalendarWriter writer, string name, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteValue(name);
                writer.WriteEquals();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        public static ICalendarWriter WriteParameter<T>(this ICalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteValue(name);
                writer.WriteEquals().WriteParameterValues(values);
            }
            return writer;
        }

        public static ICalendarWriter WriteParameterWithDQuotedValue<T>(this ICalendarWriter writer, string name, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteValue(name);
                writer.WriteEquals().WriteDQuotedParameterValue(value);
            }
            return writer;
        }

        public static ICalendarWriter WriteParameterWithDQuotedValues<T>(this ICalendarWriter writer, string name, IEnumerable<T> values)
    where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteValue(name);
                writer.WriteEquals().WriteDQuotedParameterValues(values);
            }
            return writer;
        }




        public static ICalendarWriter AppendParameterValues<T>(this ICalendarWriter writer, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize())) writer.WriteComma().WriteParameterValues(values);
            return writer;

        }

        public static ICalendarWriter AppendParameter<T>(this ICalendarWriter writer, T parameter)
            where T : ICalendarSerializable
        {
            if (parameter.CanSerialize()) writer.WriteSemicolon().WriteValue(parameter);
            return writer;
        }

        public static ICalendarWriter AppendParameter<T>(this ICalendarWriter writer, string name, T value)
            where T : ICalendarSerializable
        {
            if (value.CanSerialize()) writer.WriteSemicolon().WriteParameter(name, value);
            return writer;
        }

        public static ICalendarWriter AppendParameter<T>(this ICalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteSemicolon().WriteValue(name);
                writer.WriteEquals().WriteParameterValues(values);
            }
            return writer;
        }

        public static ICalendarWriter AppendParameters<T>(this ICalendarWriter writer, IEnumerable<T> parameters)
            where T : ICalendarSerializable, IEquatable<T>
        {
            foreach (var parameter in parameters.Where(x => x.CanSerialize()))
            {
                writer.WriteSemicolon();
                parameter.WriteCalendar(writer);
            }
            return writer;
        }



        public static ICalendarWriter WriteProperty<T>(this ICalendarWriter writer, string name, T value)
    where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteValue(name);
                writer.WriteColon().WriteValue(value);
            }

            return writer;
        }

        public static ICalendarWriter WriteProperty<T>(this ICalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteValue(name);
                writer.WriteColon().WritePropertyValues(values);
            }
            return writer;
        }

        public static ICalendarWriter WriteProperties<T>(this ICalendarWriter writer, IEnumerable<T> properties)
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

        public static ICalendarWriter AppendProperty<T>(this ICalendarWriter writer, T property)
            where T : ICalendarSerializable
        {
            if (property.CanSerialize())
            {
                writer.WriteLine();
                property.WriteCalendar(writer);
            }
            return writer;
        }

        public static ICalendarWriter AppendProperties<T>(this ICalendarWriter writer, IEnumerable<T> properties)
    where T : ICalendarSerializable
        {
            foreach (var property in properties.Where(x => x.CanSerialize()))
            {
                writer.WriteLine();
                property.WriteCalendar(writer);
            }
            return writer;
        }



        public static ICalendarWriter AppendPropertyValues<T>(this ICalendarWriter writer, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize())) writer.WriteSemicolon().WritePropertyValues(values);
            return writer;
        }

        public static ICalendarWriter AppendProperty<T>(this ICalendarWriter writer, string name, T value)
    where T : ICalendarSerializable, IEquatable<T>
        {
            if (value.CanSerialize())
            {
                writer.WriteLine();
                writer.WriteValue(name);
                writer.WriteColon().WriteValue(value);
            }

            return writer;
        }

        public static ICalendarWriter AppendProperty<T>(this ICalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteLine();
                writer.WriteValue(name);
                writer.WriteColon().WriteParameterValues(values);
            }
            return writer;
        }
    }
}
