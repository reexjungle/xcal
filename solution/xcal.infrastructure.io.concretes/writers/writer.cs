using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace xcal.infrastructure.io.concretes.writers
{

    /// <summary>
    /// Represents a writer that provides a fast, non-cached, forward-only way to generate streams or
    /// files that contain iCalendar data.
    /// </summary>
    public abstract class CalendarWriter
    {
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
        /// Initializes a new instance of the <see cref="CalendarWriter"/> class.
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
        public CalendarWriter WriteStartComponent(string name)
        {
            writer.Write("BEGIN:" + name);
            return this;
        }

        public CalendarWriter AppendStartComponent(string name)
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
        public CalendarWriter WriteEndComponent(string name)
        {
            writer.Write("END:" + name);
            return this;
        }

        public CalendarWriter AppendEndComponent(string name)
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
        public CalendarWriter WriteSafeStringValue(string value)
        {
            writer.Write(ConvertToSAFE_STRING(value));
            return this;
        }

        /// <summary>
        /// Writes the value, enclosed by double quotes (")
        /// </summary>
        /// <param name="value">The string value to be enclosed by double quotes.</param>
        public CalendarWriter WriteQuotedStringValue(string value)
        {
            writer.Write(ConvertToQuotedString(value));
            return this;
        }

        public CalendarWriter WriteDQuote()
        {
            writer.Write(DQUOTE);
            return this;
        }

        /// <summary>
        /// Writes a comma character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public CalendarWriter WriteComma()
        {
            writer.Write(COMMA);
            return this;
        }

        /// <summary>
        /// Writes a semicolon character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public CalendarWriter WriteSemicolon()
        {
            writer.Write(SEMICOLON);
            return this;
        }

        /// <summary>
        /// Writes a colon character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public CalendarWriter WriteColon()
        {
            writer.Write(COLON);
            return this;
        }

        /// <summary>
        /// Writes the equality character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public CalendarWriter WriteEquals()
        {
            writer.Write('=');
            return this;
        }

        public CalendarWriter WriteValue(string value)
        {
            writer.Write(value);
            return this;
        }

        public CalendarWriter WriteValue(char value)
        {
            writer.Write(value);
            return this;
        }

        public CalendarWriter WriteValue(bool value)
        {
            writer.Write(value);
            return this;
        }

        public CalendarWriter WriteValue(uint value)
        {
            writer.Write(value);
            return this;
        }

        public CalendarWriter WriteValue(int value)
        {
            writer.Write(value);
            return this;
        }

        public CalendarWriter WriteValue(long value)
        {
            writer.Write(value);
            return this;
        }

        public CalendarWriter WriteValue(ulong value)
        {
            writer.Write(value);
            return this;
        }

        public CalendarWriter WriteValue(float value)
        {
            writer.Write(value);
            return this;
        }

        public CalendarWriter WriteValue(double value)
        {
            writer.Write(value);
            return this;
        }

        public CalendarWriter WriteValue(char[] buffer, int index, int count)
        {
            writer.Write(buffer, index, count);
            return this;

        }

        /// <summary>
        /// Writes iCalendar VALUEs concatenated by a comma character to the text string or stream.
        /// </summary>
        /// <param name="values">The VALUEs to be written to the text string or stream.</param>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public CalendarWriter WriteValues(IEnumerable<string> values)
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
        public CalendarWriter WriteParameters(IEnumerable<string> parameters)
        {
            writer.Write(string.Join(SEMICOLON, parameters));
            return this;
        }

        public CalendarWriter WriteParameter(string name, string value)
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
        public CalendarWriter WriteParameter(string name, IEnumerable<string> values)
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
        public CalendarWriter WriteXParameter(string name, IEnumerable<string> values) => WriteParameter("X-" + name, values);

        public CalendarWriter AppendParameter(string name, string value) => WriteSemicolon().WriteParameter(name, value);

        public CalendarWriter AppendParameter(string name, IEnumerable<string> values) => WriteSemicolon().WriteParameter(name, values);

        public CalendarWriter AppendByComma(string value) => WriteComma().WriteValue(value);

        public CalendarWriter AppendBySemicolon(string value) => WriteSemicolon().WriteValue(value);

        public CalendarWriter AppendByComma(IEnumerable<string> values)
        {
            return WriteComma().WriteValues(values);
        }

        public CalendarWriter AppendBySemicolon(IEnumerable<string> values)
        {
            return WriteSemicolon().WriteValues(values);
        }

        public CalendarWriter AppendParameter(string parameter) => WriteSemicolon().WriteValue(parameter);

        public CalendarWriter AppendParameters(IEnumerable<string> parameters) => WriteSemicolon().WriteParameters(parameters);

        public CalendarWriter AppendPropertyValue(string value) => WriteColon().WriteValue(value);

        public CalendarWriter AppendPropertyValues(IEnumerable<string> values) => WriteColon().WriteValues(values);

        public CalendarWriter WriteProperty(string name, string value, IEnumerable<string> parameters = null)
        {
            if (parameters != null && parameters.Count() != 0)
                writer.Write("{0};{1}:{2}", name, string.Join(";", parameters), value);
            else
                writer.Write("{0}:{1}", name, value);
            return this;
        }

        public CalendarWriter AppendProperty(string name, string value, IEnumerable<string> parameters = null)
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
        public CalendarWriter WriteProperties(IEnumerable<string> properties)
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
        public CalendarWriter WriteComponent(string name, IEnumerable<string> properties)
            => WriteStartComponent(name)
            .WriteProperties(properties)
            .WriteEndComponent(name);

        /// <summary>
        /// Writes iCalendar COMPONENTs to the text string or stream.
        /// </summary>
        /// <param name="components"></param>
        /// <returns>Writes iCalendar parameters to the text string or stream.</returns>
        public CalendarWriter WriteComponents(IEnumerable<string> components)
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
        /// That is for eack line, a carriage return line feed (CRLF) followed by a single linear white-space character(i.e., SPACE or horizontal tab) is inserted after every 76 characters in the string representation.
        /// </para>
        /// <para>
        /// Any sequence of CRLF followed immediately by a single linear white-space character is
        /// ignored(i.e., removed) when processing the content type.
        /// </para>
        /// <para/>
        /// Note: Each split line is not longer than 75 octets excluding line breaks.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public abstract CalendarWriter InsertLineBreaks();

        /// <summary>
        /// Removes line breaks after every 75 characters in the string representation.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public abstract CalendarWriter RemoveLineBreaks();


        public override string ToString() => writer.ToString();
    }




}
