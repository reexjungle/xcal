using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.io.writers.specialized;
using reexjungle.xcal.core.domain.contracts.serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace xcal.infrastructure.io.concretes.writers
{
    /// <summary>
    /// Represents an abstract writer that encapsulates a <see cref="TextWriter"/> for writing
    /// iCalendar information to a string or stream.
    /// </summary>
    public abstract class CalendarWriter : ICalendarWriter, IGenericCalendarWriter
    {
        #region ICalendarWriter

        //literal Constants
        private const char HTAB = '\u0009';

        private const char EMPTY = '\0';
        private const string DQUOTE = @"""";
        private const string COMMA = ",";
        private const string COLON = ":";
        private const string SEMICOLON = ";";

        private const string EscapedDQUOTE = @"\""";
        private const string EscapedCOMMA = @"\,";
        private const string EscapedCOLON = @"\:";
        private const string EscapedSEMICOLON = @"\;";

        protected readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarWriter"/> class that encapsulates the given <see cref="TextWriter"/>.
        /// </summary>
        protected CalendarWriter(TextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            this.writer = writer;
        }

        /// <summary>
        /// Gives the encoding used by this writer.
        /// </summary>
        public Encoding Encoding => writer.Encoding;

        /// <summary>
        /// Writes the start tag of an iCalendar object or component with its specified name.
        /// </summary>
        /// <param name="name">The name of the iCalendar object or component.</param>
        /// <example>
        /// This example shows how to use the <see cref="WriteStartComponent(string)"/> method for a
        /// VCALENDAR component.
        /// <code>
        /// WriteStartComponent("VCALENDAR");
        /// </code>
        /// Produces the following result:
        /// <para/>
        /// BEGIN:VCALENDAR
        /// </example>
        public ICalendarWriter WriteStartComponent(string name)
        {
            writer.Write("BEGIN:" + name);
            return this;
        }

        /// <summary>
        /// Appends the start tag of an iCalendar object or component with its specified name to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the iCalendar object or component.</param>
        /// <returns>The current <see cref="ICalendarWriter"/> instance.</returns>
        public ICalendarWriter AppendStartComponent(string name)
        {
            writer.WriteLine();
            return WriteStartComponent(name);
        }

        /// <summary>
        /// Writes the end tag of an iCalendar object or component with its specified name.
        /// </summary>
        /// <param name="name">The name of the iCalendar object or component.</param>
        /// <example>
        /// This example shows how to use the <see cref="WriteEndComponent(string)"/> method for a
        /// VCALENDAR component.
        /// <code>
        /// WriteEndComponent("VCALENDAR");
        /// </code>
        /// Produces the following result:
        /// <para/>
        /// END:VCALENDAR
        /// </example>
        public ICalendarWriter WriteEndComponent(string name)
        {
            writer.Write("END:" + name);
            return this;
        }

        /// <summary>
        /// Appends the end tag of an iCalendar object or component with its specified name 
        /// to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the iCalendar object or component.</param>
        /// <returns>The current <see cref="ICalendarWriter"/> instance.</returns>
        public ICalendarWriter AppendEndComponent(string name)
        {
            writer.WriteLine();
            return WriteEndComponent(name);
        }

        /// <summary>
        /// Encloses a string value with double quotes ("...") and escapes any pre-existing quote (")
        /// within the string with a (\").
        /// </summary>
        /// <param name="value">The value to be enclosed in double quotes.</param>
        /// <returns>The value enclosed by double quotes.</returns>
        public string ConvertToQuotedString(string value)
            => DQUOTE + value.Replace(DQUOTE, EscapedDQUOTE) + DQUOTE;

        /// <summary>
        /// Escapes the following characters in the specified string value:
        /// <para/>
        /// ':' with '\:'
        /// <para/>
        /// ';' with '\;'
        /// <para/>
        /// ',' with '\,'
        /// </summary>
        /// <param name="value">The provided string value.</param>
        /// <returns>
        /// The string value where the colon, semicolon or comma characters have been escaped.
        /// </returns>
        public string ConvertToSAFE_STRING(string value)
            => value
            .Replace(COLON, EscapedCOLON)
            .Replace(SEMICOLON, EscapedSEMICOLON)
            .Replace(COMMA, EscapedCOMMA);

        /// <summary>
        /// Escapes the following characters in the specified string value:
        /// <para/>
        /// '\t' with '\0'
        /// <para/>
        /// '"' with '\";'
        /// </summary>
        /// <param name="value">The provided string value.</param>
        /// <returns>
        /// The string value where the horizontal tab or double quote characters have been escaped.
        /// </returns>
        public string ConvertToQSAFE_STRING(string value)
            => value
            .Replace(HTAB, EMPTY)
            .Replace(DQUOTE, EscapedDQUOTE);

        /// <summary>
        /// Writes the string value, in which the following characters have been escaped:
        /// <para/>
        /// ':' with '\:'
        /// <para/>
        /// ';' with '\;'
        /// <para/>
        /// ',' with '\,'
        /// </summary>
        /// <param name="value"></param>
        public ICalendarWriter WriteSafeStringValue(string value)
        {
            writer.Write(ConvertToSAFE_STRING(value));
            return this;
        }

        /// <summary>
        /// Writes the value, enclosed by double quotes (")
        /// </summary>
        /// <param name="value">The string value to be enclosed by double quotes.</param>
        public ICalendarWriter WriteQuotedStringValue(string value)
        {
            writer.Write(ConvertToQuotedString(value));
            return this;
        }

        public ICalendarWriter WriteDQuote()
        {
            writer.Write(DQUOTE);
            return this;
        }

        /// <summary>
        /// Writes a comma character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public ICalendarWriter WriteComma()
        {
            writer.Write(COMMA);
            return this;
        }

        /// <summary>
        /// Writes a semicolon character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public ICalendarWriter WriteSemicolon()
        {
            writer.Write(SEMICOLON);
            return this;
        }

        /// <summary>
        /// Writes a colon character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public ICalendarWriter WriteColon()
        {
            writer.Write(COLON);
            return this;
        }

        /// <summary>
        /// Writes the equality character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public ICalendarWriter WriteEquals()
        {
            writer.Write('=');
            return this;
        }

        /// <summary>
        /// Writes a line terminator to the unserlying text string or stream.
        /// </summary>
        /// <returns></returns>
        public ICalendarWriter WriteLine()
        {
            writer.WriteLine();
            return this;
        }

        /// <summary>
        /// Writes a string value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">
        /// The string value to be written to the underlying text string or stream.
        /// </param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValue(string value)
        {
            writer.Write(value);
            return this;
        }

        /// <summary>
        /// Writes a character value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">
        /// The character value to be written to the underlying text string or stream.
        /// </param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValue(char value)
        {
            writer.Write(value);
            return this;
        }

        /// <summary>
        /// Writes a boolean value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">
        /// The boolean value to be written to the underlying text string or stream.
        /// </param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValue(bool value)
        {
            writer.Write(value);
            return this;
        }

        /// <summary>
        /// Writes an unsigned integer value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">
        /// The unsigned integer value to be written to the underlying text string or stream.
        /// </param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValue(uint value)
        {
            writer.Write(value);
            return this;
        }

        /// <summary>
        /// Writes an integer value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">
        /// The integer value to be written to the underlying text string or stream.
        /// </param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValue(int value)
        {
            writer.Write(value);
            return this;
        }

        /// <summary>
        /// Writes a long integer value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">
        /// The integer value to be written to the underlying text string or stream.
        /// </param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValue(long value)
        {
            writer.Write(value);
            return this;
        }

        /// <summary>
        /// Writes an unsigned long integer value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">
        /// The unsigned long integer value to be written to the underlying text string or stream.
        /// </param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValue(ulong value)
        {
            writer.Write(value);
            return this;
        }

        /// <summary>
        /// Writes a floating value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">
        /// The floating value to be written to the underlying text string or stream.
        /// </param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValue(float value)
        {
            writer.Write(value);
            return this;
        }

        /// <summary>
        /// Writes a double precision value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">
        /// The double precision value to be written to the underlying text string or stream.
        /// </param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValue(double value)
        {
            writer.Write(value);
            return this;
        }

        /// <summary>
        /// Writes a subarray of characters to the underlying text string or stream.
        /// </summary>
        /// <param name="buffer">
        /// The subarray of characters to be written to the underlying text string or stream.
        /// </param>
        /// <param name="index">
        /// The character position in the buffer at which to start retrieving data.
        /// </param>
        /// <param name="count">The number of characters to write.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValue(char[] buffer, int index, int count)
        {
            writer.Write(buffer, index, count);
            return this;
        }

        /// <summary>
        /// Writes iCalendar VALUEs concatenated by a comma character to the text string or stream.
        /// </summary>
        /// <param name="values">The VALUEs to be written to the text string or stream.</param>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public ICalendarWriter WriteValues(IEnumerable<string> values)
        {
            writer.Write(string.Join(COMMA, values));
            return this;
        }

        /// <summary>
        /// Writes iCalendar PARAMETERs to the text string or stream.
        /// <para/>
        /// It is assumed each PARAMETER is correctly formatted in the iCalendar format.
        /// </summary>
        /// <param name="parameters">The PARAMETERs to be written to the text string or stream.</param>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public ICalendarWriter WriteParameters(IEnumerable<string> parameters)
        {
            writer.Write(string.Join(SEMICOLON, parameters));
            return this;
        }

        public ICalendarWriter WriteParameter(string name, string value)
        {
            writer.Write("{0}={1}", name, value);
            return this;
        }

        /// <summary>
        /// Writes an iCalendar PARAMETER to the text string or stream.
        /// </summary>
        /// <param name="name">The name of the PARAMETER.</param>
        /// <param name="values">The VALUEs of the PARAMETER.</param>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public ICalendarWriter WriteParameter(string name, IEnumerable<string> values)
        {
            writer.Write("{0}={1}", name, string.Join(COMMA, values));
            return this;
        }

        /// <summary>
        /// Writes the non-standard experimental parameter by prefixing the parameter name with an 'X-'.
        /// </summary>
        /// <param name="name">The name of the non-standard experimental parameter.</param>
        /// <param name="values">
        /// The values of the expermimental parameter.
        /// <para/>
        /// Note: It is assumed each VALUE is correctly written in the iCalendar format.
        /// </param>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public ICalendarWriter WriteXParameter(string name, IEnumerable<string> values) => WriteParameter("X-" + name, values);

        /// <summary>
        /// Appends an iCalendar parameter with its value to the underyling text string or stream.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendParameter(string name, string value) => WriteSemicolon().WriteParameter(name, value);

        /// <summary>
        /// Appends an iCalendar parameter with its values to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="values">The sequence of values of the parameter.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendParameter(string name, IEnumerable<string> values) => WriteSemicolon().WriteParameter(name, values);

        /// <summary>
        /// Appends a comma (",") and the specified value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">The value that is to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendByComma(string value) => WriteComma().WriteValue(value);

        /// <summary>
        /// Appends a semicolon (";") and the specified value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">The value that is to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendBySemicolon(string value) => WriteSemicolon().WriteValue(value);

        /// <summary>
        /// Appends a comma (",") and the specified sequence of iCalendar values, where each value is
        /// delimited by a comma (",").
        /// </summary>
        /// <param name="values">The sequence of iCalendar values to appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendByComma(IEnumerable<string> values)
        {
            return WriteComma().WriteValues(values);
        }

        /// <summary>
        /// Appends a semicolon (";") and the specified sequence of iCalendar values, where each
        /// value is delimited by a comma (";").
        /// </summary>
        /// <param name="values">The sequence of iCalendar values to appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendBySemicolon(IEnumerable<string> values)
        {
            return WriteSemicolon().WriteValues(values);
        }

        /// <summary>
        /// Appends a specified formatted iCalendar paraneter to the underlying text string or stream.
        /// </summary>
        /// <param name="parameter">The parameter to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendParameter(string parameter) => WriteSemicolon().WriteValue(parameter);

        /// <summary>
        /// Appends a specified sequence of parameters to the underlying text string or stream.
        /// </summary>
        /// <param name="parameters">The sequence of parameters to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendParameters(IEnumerable<string> parameters) => WriteSemicolon().WriteParameters(parameters);

        /// <summary>
        /// Appends the value of an iCalendar property to the underlying text string or stream.
        /// </summary>
        /// <param name="value">The value that is to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendPropertyValue(string value) => WriteColon().WriteValue(value);

        /// <summary>
        /// Appends the sequence of values of an iCalendar property to the underlying text string or stream.
        /// </summary>
        /// <param name="values">The sequence of values to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendPropertyValues(IEnumerable<string> values) => WriteColon().WriteValues(values);

        /// <summary>
        /// Writes an iCalendar property by its name, value and parameters to the underlying text
        /// string or stream.
        /// </summary>
        /// <param name="name">The name of the iCalendar property.</param>
        /// <param name="value">The value of the iCalendar property.</param>
        /// <param name="parameters">The sequence of parameter strings of the iCalendar property.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter WriteProperty(string name, string value, IEnumerable<string> parameters = null)
        {
            if (parameters != null && parameters.Count() != 0)
                writer.Write("{0};{1}:{2}", name, string.Join(";", parameters), value);
            else
                writer.Write("{0}:{1}", name, value);
            return this;
        }

        /// <summary>
        /// Appends an iCalendar property by its name, value and parameters to the underlying text
        /// string or stream.
        /// </summary>
        /// <param name="name">The name of the iCalendar property.</param>
        /// <param name="value">The value of the iCalendar property.</param>
        /// <param name="parameters">The sequence of parameter strings of the iCalendar property.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        public ICalendarWriter AppendProperty(string name, string value, IEnumerable<string> parameters = null)
        {
            writer.WriteLine();
            return WriteProperty(name, value, parameters);
        }

        /// <summary>
        /// Writes properties, where each written property is followed by the line terminator.
        /// </summary>
        /// <param name="properties"></param>
        /// <remarks>This method assumes each property has been correctly formatted.</remarks>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public ICalendarWriter WriteProperties(IEnumerable<string> properties)
        {
            foreach (var property in properties.Where(x => !string.IsNullOrWhiteSpace(x) && !string.IsNullOrEmpty(x)))
            {
                writer.Write(property);
            }
            return this;
        }

        /// <summary>
        /// Writes an iCalendar COMPONENT with the specifed component name and properties of the
        /// component to the text string or stream.
        /// </summary>
        /// <param name="name">The name of the component to be written.</param>
        /// <param name="properties">
        /// The properties of the component.
        /// <para/>
        /// Note: It is assumed each property is correctly written in the iCalendar format.
        /// </param>
        /// <remarks>This method assumes the parameters of the property have been correctly formatted.</remarks>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public ICalendarWriter WriteComponent(string name, IEnumerable<string> properties)
            => WriteStartComponent(name)
            .WriteProperties(properties)
            .WriteEndComponent(name);

        /// <summary>
        /// Writes iCalendar COMPONENTs to the text string or stream.
        /// </summary>
        /// <param name="components"></param>
        /// <returns>Writes iCalendar parameters to the text string or stream.</returns>
        public ICalendarWriter WriteComponents(IEnumerable<string> components)
        {
            foreach (var component in components.Where(x => !string.IsNullOrWhiteSpace(x) && !string.IsNullOrEmpty(x)))
            {
                writer.Write(component);
            }
            return this;
        }

        /// <summary>
        /// Inserts line breaks after every 75 characters in the string representation.
        /// <para>
        /// That is for eack line, a carriage return line feed (CRLF) followed by a single linear
        /// white-space character(i.e., SPACE or horizontal tab) is inserted after every 76
        /// characters in the string representation.
        /// </para>
        /// <para>
        /// Any sequence of CRLF followed immediately by a single linear white-space character is
        /// ignored(i.e., removed) when processing the content type.
        /// </para>
        /// <para/>
        /// Note: Each split line is not longer than 75 octets excluding line breaks.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public abstract ICalendarWriter InsertLineBreaks();

        /// <summary>
        /// Removes line breaks after every 75 characters in the string representation.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public abstract ICalendarWriter RemoveLineBreaks();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => writer.ToString();

        #endregion ICalendarWriter

        #region IGenericCalendarWriter

        public ICalendarWriter WriteValue<T>(T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize()) value.WriteCalendar(this);
            return this;
        }

        public ICalendarWriter WriteParameters<T>(IEnumerable<T> parameters) where T : ICalendarSerializable
        {
            var first = parameters.FirstOrDefault();
            foreach (var parameter in parameters.Where(x => x.CanSerialize()))
            {
                if (first != null && !first.Equals(parameter)) WriteSemicolon();
                parameter.WriteCalendar(this);
            }
            return this;
        }

        public ICalendarWriter WriteParameter<T>(string name, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                WriteValue(name).WriteEquals();
                value.WriteCalendar(this);
            }
            return this;
        }

        public ICalendarWriter WriteParameter<T>(string name, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                WriteValue(name).WriteEquals();
                WriteParameterValues(values);
            }
            return this;
        }

        public ICalendarWriter WriteParameterWithDQuotedValue<T>(string name, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                WriteValue(name).WriteEquals();
                WriteDQuotedParameterValue(value);
            }
            return this;
        }

        public ICalendarWriter WriteParameterWithDQuotedValues<T>(string name, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                WriteValue(name).WriteEquals();
                WriteDQuotedParameterValues(values);
            }
            return this;
        }

        public ICalendarWriter WriteParameterValues<T>(IEnumerable<T> values) where T : ICalendarSerializable
        {
            var first = values.FirstOrDefault();
            foreach (var value in values.Where(x => x.CanSerialize()))
            {
                if (first != null && !first.Equals(value)) WriteComma();
                value.WriteCalendar(this);
            }
            return this;
        }

        public ICalendarWriter WriteDQuotedParameterValue<T>(T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                WriteDQuote();
                WriteValue(value);
                WriteDQuote();
            }
            return this;
        }

        public ICalendarWriter WriteDQuotedParameterValues<T>(IEnumerable<T> values) where T : ICalendarSerializable
        {
            var first = values.FirstOrDefault();
            foreach (var value in values)
            {
                if (first != null && !first.Equals(value)) WriteComma();
                WriteDQuotedParameterValue(value);
            }
            return this;
        }

        public ICalendarWriter AppendParameterValue<T>(T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                WriteComma();
                value.WriteCalendar(this);
            }
            return this;
        }

        public ICalendarWriter AppendParameterValues<T>(IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                WriteComma();
                WriteParameterValues(values);
            }
            return this;
        }

        public ICalendarWriter AppendParameter<T>(T parameter) where T : ICalendarSerializable
        {
            if (parameter.CanSerialize())
            {
                WriteSemicolon();
                WriteValue(parameter);
            }
            return this;
        }

        public ICalendarWriter AppendParameter<T>(string name, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                WriteSemicolon();
                WriteParameter(name, value);
            }
            return this;
        }

        public ICalendarWriter AppendParameter<T>(string name, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                WriteSemicolon().WriteValue(name).WriteEquals();
                WriteParameterValues(values);
            }
            return this;
        }

        public ICalendarWriter AppendParameters<T>(IEnumerable<T> parameters) where T : ICalendarSerializable
        {
            foreach (var parameter in parameters.Where(x => x.CanSerialize()))
            {
                WriteSemicolon();
                parameter.WriteCalendar(this);
            }
            return this;
        }

        public ICalendarWriter WriteProperty<T>(string name, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                WriteValue(name).WriteColon();
                WriteValue(value);
            }

            return this;
        }

        public ICalendarWriter WriteProperty<T>(string name, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                WriteValue(name).WriteColon();
                WritePropertyValues(values);
            }
            return this;
        }

        public ICalendarWriter WriteProperties<T>(IEnumerable<T> properties) where T : ICalendarSerializable
        {
            var first = properties.FirstOrDefault();
            foreach (var property in properties.Where(x => x.CanSerialize()))
            {
                if (first != null && !first.Equals(property)) writer.WriteLine();
                property.WriteCalendar(this);
            }
            return this;
        }

        public ICalendarWriter AppendProperty<T>(T property) where T : ICalendarSerializable
        {
            if (property.CanSerialize())
            {
                writer.WriteLine();
                property.WriteCalendar(this);
            }
            return this;
        }

        public ICalendarWriter AppendProperties<T>(IEnumerable<T> properties) where T : ICalendarSerializable
        {
            foreach (var property in properties.Where(x => x.CanSerialize()))
            {
                writer.WriteLine();
                property.WriteCalendar(this);
            }
            return this;
        }

        public ICalendarWriter AppendPropertyValues<T>(IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                WriteSemicolon();
                WritePropertyValues(values);
            }
            return this;
        }

        public ICalendarWriter AppendProperty<T>(string name, T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                WriteLine().WriteValue(name).WriteColon();
                WriteValue(value);
            }

            return this;
        }

        public ICalendarWriter AppendProperty<T>(string name, IEnumerable<T> values) where T : ICalendarSerializable
        {
            if (values.Any(x => x.CanSerialize()))
            {
                WriteLine().WriteValue(name).WriteColon();
                WriteParameterValues(values);
            }
            return this;
        }

        public ICalendarWriter WritePropertyValues<T>(IEnumerable<T> values) where T : ICalendarSerializable
        {
            var first = values.FirstOrDefault();
            foreach (var value in values.Where(x => x.CanSerialize()))
            {
                if (first != null && !first.Equals(value)) WriteSemicolon();
                value.WriteCalendar(this);
            }
            return this;
        }

        public ICalendarWriter AppendPropertyValue<T>(T value) where T : ICalendarSerializable
        {
            if (value.CanSerialize())
            {
                WriteSemicolon();
                value.WriteCalendar(this);
            }
            return this;
        }

        #endregion IGenericCalendarWriter
    }
}
