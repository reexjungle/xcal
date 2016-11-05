using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexjungle.xcal.core.domain.concretes.extensions
{
    /// <summary>
    /// Provides additional featues of the <see cref="ICalendarWriter"/> class.
    /// </summary>
    public static class ICalendarWriterExtensions
    {
        /// <summary>
        /// Writes the specified value to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of value to write.</typeparam>
        /// <param name="writer">The writer used in writing the value.</param>
        /// <param name="value">The value to be written.</param>
        /// <returns>The writer used in writing the value.</returns>
        public static ICalendarWriter WriteValue<T>(this ICalendarWriter writer, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize()) value.WriteCalendar(writer);
            return writer;
        }

        /// <summary>
        /// Writes the specified parameters to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter values to write.</typeparam>
        /// <param name="writer">The writer used in writing the value.</param>
        /// <param name="parameters">The parameters to be written</param>
        /// <returns>The writer used in writing the value.</returns>
        public static ICalendarWriter WriteParameters<T>(this ICalendarWriter writer, IEnumerable<T> parameters) where T : ICalendarSerializable
        {
            var first = parameters.FirstOrDefault();
            foreach (var parameter in parameters.Where(x => x.CanSerialize()))
            {
                if (first != null && !first.Equals(parameter)) writer.WriteSemicolon();
                parameter.WriteCalendar(writer);
            }
            return writer;
        }

        /// <summary>
        /// Writes a parameter specified by its name and value to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter value to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns></returns>
        public static ICalendarWriter WriteParameter<T>(this ICalendarWriter writer, string name, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteValue(name).WriteEquals();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        /// <summary>
        /// Writes a parameter specified by its name and value to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter value to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="values">The values of the parameter.</param>
        /// <returns>The writer used in writing the value.</returns>
        public static ICalendarWriter WriteParameter<T>(this ICalendarWriter writer, string name, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteValue(name).WriteEquals().WriteParameterValues(values);
            }
            return writer;
        }

        /// <summary>
        /// Writes a parameter specified by its name and value to the underlying text string or stream.
        /// <para /> The written parameter value is enclosed by double quotes (").
        /// </summary>
        /// <typeparam name="T">The type of parameter to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The writer used in writing the value.</returns>
        public static ICalendarWriter WriteParameterWithDQuotedValue<T>(this ICalendarWriter writer, string name, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteValue(name).WriteEquals().WriteDQuotedParameterValue(value);
            }
            return writer;
        }

        /// <summary>
        /// Writes a parameter specified by its name and values to the underlying text string or stream.
        /// <para /> Each written parameter value is enclosed by double quotes (").
        /// </summary>
        /// <typeparam name="T">The type of parameter to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="values">The values of the parameter.</param>
        /// <returns>The writer used in writing the value.</returns>
        public static ICalendarWriter WriteParameterWithDQuotedValues<T>(this ICalendarWriter writer, string name, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteValue(name).WriteEquals().WriteDQuotedParameterValues(values);
            }
            return writer;
        }

        /// <summary>
        /// Writes parameter values to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter value to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="values">The values of the parameter.</param>
        /// <returns>The writer used in writing the values.</returns>
        public static ICalendarWriter WriteParameterValues<T>(this ICalendarWriter writer, IEnumerable<T> values) where T : ICalendarSerializable
        {
            var first = values.FirstOrDefault();
            foreach (var value in values.Where(x => x.CanSerialize()))
            {
                if (first != null && !first.Equals(value)) writer.WriteComma();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        /// <summary>
        /// Writes quoted parameter values to the underlying text string or stream.
        /// <para /> The written parameter value is enclosed by double quotes (").
        /// </summary>
        /// <typeparam name="T">The type of parameter value to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The writer used in writing the value.</returns>
        public static ICalendarWriter WriteDQuotedParameterValue<T>(this ICalendarWriter writer, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteDQuote().WriteValue(value).WriteDQuote();
            }
            return writer;
        }

        /// <summary>
        /// Writes quoted parameter values to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter value to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="values">The values of the parameter.</param>
        /// <returns>The writer used in writing the values.</returns>
        public static ICalendarWriter WriteDQuotedParameterValues<T>(this ICalendarWriter writer, IEnumerable<T> values) where T : ICalendarSerializable
        {
            var first = values.FirstOrDefault();
            foreach (var value in values)
            {
                if (first != null && !first.Equals(value)) writer.WriteComma();
                writer.WriteDQuotedParameterValue(value);
            }
            return writer;
        }

        /// <summary>
        /// Appends a parameter value to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter value to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The writer used in writing the parameter values.</returns>
        public static ICalendarWriter AppendParameterValue<T>(this ICalendarWriter writer, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteComma();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        /// <summary>
        /// Appends parameter values to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter value to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="values">The values of the parameter.</param>
        /// <returns>The writer used in writing the parameter values.</returns>
        public static ICalendarWriter AppendParameterValues<T>(this ICalendarWriter writer, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteComma().WriteParameterValues(values);
            }
            return writer;
        }

        /// <summary>
        /// Appends parameter values to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter value to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="parameter">The value of the parameter.</param>
        /// <returns>The writer used in writing the parameter value.</returns>
        public static ICalendarWriter AppendParameter<T>(this ICalendarWriter writer, T parameter) where T : ICalendarSerializable
        {
            if (parameter.CanSerialize())
            {
                writer.WriteSemicolon().WriteValue(parameter);
            }
            return writer;
        }

        /// <summary>
        /// Appends a parameter value to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter value to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The writer used in writing the value.</returns>
        public static ICalendarWriter AppendParameter<T>(this ICalendarWriter writer, string name, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteSemicolon();
                writer.WriteParameter(name, value);
            }
            return writer;
        }

        /// <summary>
        /// Appends parameter values to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter values to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="values">The values of the parameter.</param>
        /// <returns>The writer used in writing the values.</returns>
        public static ICalendarWriter AppendParameter<T>(this ICalendarWriter writer, string name, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteSemicolon().WriteValue(name).WriteEquals().WriteParameterValues(values);
            }
            return writer;
        }

        /// <summary>
        /// Appends parameters to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of parameter to write.</typeparam>
        /// <param name="writer">The writer used in writing the parameter.</param>
        /// <param name="parameters">The parameters to be written.</param>
        /// <returns>The writer used in writing the parameters.</returns>
        public static ICalendarWriter AppendParameters<T>(this ICalendarWriter writer, IEnumerable<T> parameters) where T : ICalendarSerializable
        {
            foreach (var parameter in parameters.Where(x => x.CanSerialize()))
            {
                writer.WriteSemicolon();
                parameter.WriteCalendar(writer);
            }
            return writer;
        }

        /// <summary>
        /// Writes a property specified by its name and value to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of property to write.</typeparam>
        /// <param name="writer">The writer used in writing the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The writer used in writing the property.</returns>
        public static ICalendarWriter WriteProperty<T>(this ICalendarWriter writer, string name, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteValue(name).WriteColon().WriteValue(value);
            }

            return writer;
        }

        /// <summary>
        /// Writes a property specified by its name and values to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of property to write.</typeparam>
        /// <param name="writer">The writer used in writing the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="values">The values of the property.</param>
        /// <returns>The writer used in writing the property.</returns>
        public static ICalendarWriter WriteProperty<T>(this ICalendarWriter writer, string name, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteValue(name).WriteColon().WritePropertyValues(values);
            }
            return writer;
        }

        /// <summary>
        /// Writes properties to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of property to write.</typeparam>
        /// <param name="writer">The writer used in writing the properties.</param>
        /// <param name="properties">The properties to be written.</param>
        /// <returns>The writer used in writing the properties.</returns>
        public static ICalendarWriter WriteProperties<T>(this ICalendarWriter writer, IEnumerable<T> properties) where T : ICalendarSerializable
        {
            var first = properties.FirstOrDefault();
            foreach (var property in properties.Where(x => x.CanSerialize()))
            {
                if (first != null && !first.Equals(property)) writer.WriteLine();
                property.WriteCalendar(writer);
            }
            return writer;
        }

        /// <summary>
        /// Appends a property to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of property to append.</typeparam>
        /// <param name="writer">The writer used in appending the property.</param>
        /// <param name="property">The property to be appended.</param>
        /// <returns>The writer used in writing the property.</returns>
        public static ICalendarWriter AppendProperty<T>(this ICalendarWriter writer, T property) where T : ICalendarSerializable
        {
            if (property.CanSerialize())
            {
                writer.WriteLine();
                property.WriteCalendar(writer);
            }
            return writer;
        }

        /// <summary>
        ///  Appends properties to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of property to append.</typeparam>
        /// <param name="writer">The writer used in appending the property.</param>
        /// <param name="properties">The properties to be appended.</param>
        /// <returns>The writer used in appending the properties.</returns>
        public static ICalendarWriter AppendProperties<T>(this ICalendarWriter writer, IEnumerable<T> properties) where T : ICalendarSerializable
        {
            foreach (var property in properties.Where(x => x.CanSerialize()))
            {
                writer.WriteLine();
                property.WriteCalendar(writer);
            }
            return writer;
        }

        public static ICalendarWriter AppendPropertyValues<T>(this ICalendarWriter writer, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteSemicolon().WritePropertyValues(values);
            }
            return writer;
        }

        /// <summary>
        /// Appends a property value to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of property value to append.</typeparam>
        /// <param name="writer">The writer used in appending the property value.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property that shall be written.</param>
        /// <returns>The writer used in appending the property value.</returns>
        public static ICalendarWriter AppendProperty<T>(this ICalendarWriter writer, string name, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteLine().WriteValue(name).WriteColon().WriteValue(value);
            }
            return writer;
        }

        /// <summary>
        /// Appends property values to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of property values to append.</typeparam>
        /// <param name="writer">The writer used in appending the property values.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="values">The values of the property that shall be appended.</param>
        /// <returns>The writer used in appending the property value.</returns>
        public static ICalendarWriter AppendProperty<T>(this ICalendarWriter writer, string name, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                writer.WriteLine().WriteValue(name).WriteColon().WriteParameterValues(values);
            }
            return writer;
        }

        /// <summary>
        /// Writes property values to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of property values to write.</typeparam>
        /// <param name="writer">The writer used in writing the property values.</param>
        /// <param name="values">The values of the property that shall be written.</param>
        /// <returns>The writer used in writing the property value.</returns>
        public static ICalendarWriter WritePropertyValues<T>(this ICalendarWriter writer, IEnumerable<T> values) where T : ICalendarSerializable
        {
            var first = values.FirstOrDefault();
            foreach (var value in values.Where(x => x.CanSerialize()))
            {
                if (first != null && !first.Equals(value)) writer.WriteSemicolon();
                value.WriteCalendar(writer);
            }
            return writer;
        }

        /// <summary>
        /// Appends a property value to the underlying text string or stream.
        /// </summary>
        /// <typeparam name="T">The type of property values to write.</typeparam>
        /// <param name="writer">The writer used in writing the property value.</param>
        /// <param name="value">The value of the property that shall be written.</param>
        /// <returns>The writer used in writing the property value.</returns>
        public static ICalendarWriter AppendPropertyValue<T>(this ICalendarWriter writer, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                writer.WriteSemicolon();
                value.WriteCalendar(writer);
            }
            return writer;
        }
    }
}
