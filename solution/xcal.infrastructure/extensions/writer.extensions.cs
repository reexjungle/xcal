using System;
using System.Collections.Generic;
using System.Linq;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.serialization;

namespace reexjungle.xcal.infrastructure.extensions
{
    public static class CalendarWriterExtensions
    {
        public static CalendarWriter WriteDQuotedValue<T>(this CalendarWriter writer, T value)
            where T: ICalendarSerializable
        {
            writer.WriteDQuote();
            value.WriteCalendar(writer);
            writer.WriteDQuote();
            return writer;
        }


        public static CalendarWriter Write<T>(this CalendarWriter writer, T value)
            where T : ICalendarSerializable
        {
            value.WriteCalendar(writer);
            return writer;
        }

        public static CalendarWriter Write<T>(this CalendarWriter writer, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            var first = values.FirstOrDefault();
            if (values.Count() == 1 && !first.Equals(default(T))) return Write(writer, first);

            foreach (var value in values.Where(x => !x.Equals(default(T))))
            {
                if (!first.Equals(value))
                {
                    writer.WriteComma();
                }
                value.WriteCalendar(writer);
            }
            return writer;
        }

        public static CalendarWriter WriteDQuotedValues<T>(this CalendarWriter writer, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            var first = values.FirstOrDefault();
            if (values.Count() == 1 && !first.Equals(default(T))) return writer.WriteDQuotedValue(first);

            foreach (var value in values.Where(x => !x.Equals(default(T))))
            {
                if (!first.Equals(value))
                {
                    writer.WriteComma();
                }
                writer.WriteDQuotedValue(value);
            }
            return writer;
        }

        public static CalendarWriter WriteParameters<T>(this CalendarWriter writer, IEnumerable<T> parameters)
            where T : ICalendarSerializable, IEquatable<T>
        {
            var first = parameters.FirstOrDefault();
            if (parameters.Count() == 1 && !first.Equals(default(T))) return Write(writer, first);
            foreach (var value in parameters.Where(x => !x.Equals(default(T))))
            {
                if (!first.Equals(value)) writer.WriteSemicolon();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        public static CalendarWriter Append<T>(this CalendarWriter writer, T value)
            where T : ICalendarSerializable
        {
            return Write(writer.WriteComma(), value);
        }

        public static CalendarWriter AppendSpecial<T>(this CalendarWriter writer, T value)
            where T : ICalendarSerializable
        {
            return Write(writer.WriteSemicolon(), value);
        }

        public static CalendarWriter AppendParameter<T>(this CalendarWriter writer, T parameter)
    where T : ICalendarSerializable
        {
            return Write(writer.WriteSemicolon(), parameter);
        }

        public static CalendarWriter Append<T>(this CalendarWriter writer, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            return Write(writer.WriteComma(), values);
        }

        public static CalendarWriter AppendSpecial<T>(this CalendarWriter writer, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            return Write(writer.WriteSemicolon(), values);
        }

        public static CalendarWriter AppendParameters<T>(this CalendarWriter writer, IEnumerable<T> parameters)
            where T : ICalendarSerializable, IEquatable<T>
        {
            return writer.WriteSemicolon().WriteParameters(parameters);
        }

        public static CalendarWriter WriteParameter<T>(this CalendarWriter writer, string name, T value)
    where T : ICalendarSerializable
        {
            writer.Write(name);
            return Write(writer.WriteEquals(), value);
        }

        public static CalendarWriter WriteParameter<T>(this CalendarWriter writer, string name, IEnumerable<T> values)
where T : ICalendarSerializable, IEquatable<T>
        {
            writer.Write(name);
            return Write(writer.WriteEquals(), values);
        }

        public static CalendarWriter WriteParameterWithDQuotedvalue<T>(this CalendarWriter writer, string name, T value)
            where T : ICalendarSerializable
        {
            writer.Write(name);
            return writer.WriteEquals().WriteDQuotedValue(value);
        }

        public static CalendarWriter WriteParameterWithDQuotedValues<T>(this CalendarWriter writer, string name, IEnumerable<T> values)
    where T : ICalendarSerializable, IEquatable<T>
        {
            writer.Write(name);
            return writer.WriteEquals().WriteDQuotedValues(values);
        }



        public static CalendarWriter AppendParameter<T>(this CalendarWriter writer, string name, T value)
            where T : ICalendarSerializable
        {
            return writer.WriteSemicolon().WriteParameter(name, value);
        }

        public static CalendarWriter AppendParameter<T>(this CalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            writer.WriteSemicolon().Write(name);
            return Write(writer.WriteEquals(), values);
        }


        public static CalendarWriter WriteProperty<T>(this CalendarWriter writer, string name, T value)
            where T : ICalendarSerializable
        {
            writer.Write(name);
            return Write(writer.WriteColon(), value);
        }

        public static CalendarWriter WriteProperty<T>(this CalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            writer.Write(name);
            return writer.WriteColon().WriteParameters(values);
        }

        public static CalendarWriter AppendPropertyValue<T>(this CalendarWriter writer, T value)
            where T: ICalendarSerializable
        {
            return writer.WriteColon().Write<T>(value);
        }

        public static CalendarWriter AppendPropertyValues<T>(this CalendarWriter writer, IEnumerable<T> values)
    where T : ICalendarSerializable, IEquatable<T>
        {
            return writer.WriteColon().Write<T>(values);
        }

        public static CalendarWriter AppendProperty<T>(this CalendarWriter writer, string name, T value)
            where T : ICalendarSerializable
        {
            writer.WriteLine();
            return writer.WriteProperty(name, value);
        }

        public static CalendarWriter AppendProperty<T>(this CalendarWriter writer, T property)
    where T : ICalendarSerializable
        {
            writer.WriteLine();
            return writer.Write<T>(property);
        }


        public static CalendarWriter WriteProperties<T>(this CalendarWriter writer, IEnumerable<T> properties)
            where T : ICalendarSerializable, IEquatable<T>
        {
            var @default = default(T);
            foreach (var property in properties.Where(x => !x.Equals(@default)))
            {
                writer.Write<T>(property);
            }
            return writer;
        }

        public static CalendarWriter AppendProperties<T>(this CalendarWriter writer, IEnumerable<T> properties)
            where T : ICalendarSerializable
        {
            foreach (var property in properties)
            {
                writer.WriteLine();
                writer.Write<T>(property); 
            }
            return writer;
        }

        public static CalendarWriter AppendProperty<T>(this CalendarWriter writer, string name, IEnumerable<T> values)
            where T : ICalendarSerializable, IEquatable<T>
        {
            var first = values.FirstOrDefault();
            if (values.Count() == 1 && !first.Equals(default(T))) return writer.AppendProperty(name, first);
            writer.WriteLine();
            writer.Write(name);
            return  writer.WriteColon().Write<T>(values);
        }


    }
}
