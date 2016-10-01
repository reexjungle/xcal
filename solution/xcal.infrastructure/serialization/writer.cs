using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace reexjungle.xcal.infrastructure.serialization
{
    /// <summary>
    /// Represents a writer that provides a fast, non-cached, forward-only way to generate streams or
    /// files that contain iCalendar data.
    /// </summary>
    public abstract class CalendarWriter : TextWriter
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarWriter"/> class.
        /// </summary>
        protected CalendarWriter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarWriter"/> class with the specified
        /// format provider.
        /// </summary>
        /// <param name="formatProvider">A <see cref="IFormatProvider"/> object that controls formatting.</param>
        protected CalendarWriter(IFormatProvider formatProvider) : base(formatProvider)
        {
        }

        /// <summary>
        /// Gives the encoding used by this writer.
        /// </summary>
        public override Encoding Encoding => Encoding.UTF8;

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
            Write("BEGIN:"+ name);
            return this;
        }

        public CalendarWriter AppendStartComponent(string name)
        {
            WriteLine();
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
            Write("END:"+name);
            return this;
        }

        public CalendarWriter AppendEndComponent(string name)
        {
            WriteLine();
            return WriteEndComponent(name);
        }

        /// <summary>
        /// Encloses a string value with double quotes ("...") and escapes any pre-existing quote (")
        /// within the string with a (\").
        /// </summary>
        /// <param name="value">The value to be enclosed in double quotes.</param>
        /// <returns>The value enclosed by double quotes.</returns>
        public string ConvertToQuotedString(string value)
        {
            return DQUOTE + value.Replace(DQUOTE, EscapedDQUOTE) + DQUOTE;
        }

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
        {
            return value
                .Replace(COLON, EscapedCOLON)
                .Replace(SEMICOLON, EscapedSEMICOLON)
                .Replace(COMMA, EscapedCOMMA);
        }

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
        {
            return value
                .Replace(HTAB, EMPTY)
                .Replace(DQUOTE, EscapedDQUOTE);
        }

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
            Write(ConvertToSAFE_STRING(value));
            return this;
        }

        /// <summary>
        /// Writes the value, enclosed by double quotes (")
        /// </summary>
        /// <param name="value">The string value to be enclosed by double quotes.</param>
        public CalendarWriter WriteQuotedStringValue(string value)
        {
            Write(ConvertToQuotedString(value));
            return this;
        }

        public CalendarWriter WriteDQuote()
        {
            Write(DQUOTE);
            return this;
        }

        /// <summary>
        /// Writes a comma character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public CalendarWriter WriteComma()
        {
            Write(COMMA);
            return this;
        }

        /// <summary>
        /// Writes a semicolon character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public CalendarWriter WriteSemicolon()
        {
            Write(SEMICOLON);
            return this;
        }

        /// <summary>
        /// Writes a colon character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public CalendarWriter WriteColon()
        {
            Write(COLON);
            return this;
        }

        /// <summary>
        /// Writes the equality character to the text string or stream.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public CalendarWriter WriteEquals()
        {
            Write('=');
            return this;
        }

        /// <summary>
        /// Writes iCalendar VALUEs concatenated by a comma character to the text string or stream.
        /// </summary>
        /// <param name="values">The VALUEs to be written to the text string or stream.</param>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public CalendarWriter WriteValues(IEnumerable<string> values)
        {
            Write(string.Join(COMMA, values));
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
            Write(string.Join(SEMICOLON, parameters));
            return this;
        }

        public CalendarWriter WriteParameter(string name, string value)
        {
            Write("{0}={1}", name, value);
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
            Write("{0}={1}", name, string.Join(COMMA, values));
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
        public CalendarWriter WriteXParameter(string name, IEnumerable<string> values)
        {
            return WriteParameter("X-" + name, values);
        }

        public CalendarWriter AppendParameter(string name, string value)
        {
            return WriteSemicolon().WriteParameter(name, value);
        }

        public CalendarWriter AppendParameter(string name, IEnumerable<string> values)
        {
            return WriteSemicolon().WriteParameter(name, values);
        }

        public CalendarWriter AppendByComma(string value)
        {
            WriteComma().Write(value);
            return this;
        }

        public CalendarWriter AppendBySemicolon(string value)
        {
            WriteSemicolon().Write(value);
            return this;
        }

        public CalendarWriter AppendByComma(IEnumerable<string> values)
        {
            return WriteComma().WriteValues(values);
        }

        public CalendarWriter AppendBySemicolon(IEnumerable<string> values)
        {
            return WriteSemicolon().WriteValues(values);
        }

        /// <summary>
        /// Writes a property string with for a property that contains at least one parameter.
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value"></param>
        /// <param name="parameters">The parameters of the property</param>
        /// <remarks>This method assumes each parameter of the property has been correctly formatted.</remarks>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>

        public CalendarWriter AppendParameter(string parameter)
        {
            WriteSemicolon().Write(parameter);
            return this;
        }

        public CalendarWriter AppendParameters(IEnumerable<string> parameters)
        {
            return WriteSemicolon().WriteParameters(parameters);
        }

        public CalendarWriter AppendPropertyValue(string value)
        {
            WriteColon();
            Write(value);
            return this;
        }

        public CalendarWriter AppendPropertyValues(IEnumerable<string> values)
        {
           return WriteColon().WriteValues(values);
        }

        public CalendarWriter WriteProperty(string name, string value, IEnumerable<string> parameters = null)
        {
            if (parameters != null && parameters.Count() != 0)
            {
                Write("{0};{1}:{2}", name, string.Join(";", parameters), value);
            }
            else
            {
                Write("{0}:{1}", name, value);
            }
            return this;
        }

        public CalendarWriter AppendProperty(string name, string value, IEnumerable<string> parameters = null)
        {
            WriteLine();
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
                Write(property);
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
        {
            return WriteStartComponent(name)
                .WriteProperties(properties)
                .WriteEndComponent(name);
        }

        /// <summary>
        /// Writes iCalendar COMPONENTs to the text string or stream.
        /// </summary>
        /// <param name="components"></param>
        /// <returns>Writes iCalendar parameters to the text string or stream.</returns>
        public CalendarWriter WriteComponents(IEnumerable<string> components)
        {
            foreach (var component in components.Where(x => !string.IsNullOrWhiteSpace(x) && !string.IsNullOrEmpty(x)))
            {
                Write(component);
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
    }

    public class CalendarTextWriter : CalendarWriter
    {
        private StringBuilder builder;
        private bool opened;
        private static readonly string CRLF = Environment.NewLine;
        private const int MAX = 75;
        private const char SPACE = '\u0020';

        public CalendarTextWriter() : this(new StringBuilder())
        {
        }

        public CalendarTextWriter(StringBuilder builder) : this(builder, CultureInfo.CurrentCulture)
        {
        }

        public CalendarTextWriter(StringBuilder builder, IFormatProvider provider) : base(provider)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            this.builder = builder;
            opened = true;
        }

        public CalendarTextWriter(IFormatProvider provider) : base(provider)
        {
            builder = new StringBuilder();
            opened = true;
        }

        public StringBuilder Datasource => builder;

        public override void Write(char value)
        {
            if (!opened) throw new ObjectDisposedException("CalendarTextWriter", "Writer closed");
            builder.Append(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            if (!opened) throw new ObjectDisposedException("CalendarTextWriter", "Writer closed");

            builder.Append(buffer, index, count);
        }

        public override void Write(string value)
        {
            if (!opened) throw new ObjectDisposedException("CalendarTextWriter", "Writer closed");
            if (value != null) builder.Append(value);
        }

        public override void Close()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            opened = false;
            base.Dispose(disposing);
        }

        public override string ToString()
        {
            return builder.ToString();
        }

        public static CalendarWriter Create(StringBuilder builder, IFormatProvider provider)
        {
            return new CalendarTextWriter(builder, provider);
        }

        public static CalendarWriter Create(IFormatProvider provider)
        {
            return new CalendarTextWriter(provider);
        }

        private static string Fold(string value, string newline, int max, Encoding encoding)
        {
            var lines = value.Split(new[] { newline }, StringSplitOptions.RemoveEmptyEntries);

            using (var ms = new MemoryStream(value.Length))
            {
                var crlf = encoding.GetBytes(newline); //CRLF
                var crlfs = encoding.GetBytes(newline + new string(SPACE, 1)); //CRLF and SPACE
                foreach (var line in lines)
                {
                    var bytes = encoding.GetBytes(line);
                    var size = bytes.Length;
                    if (size <= max)
                    {
                        ms.Write(bytes, 0, size);
                        ms.Write(crlf, 0, crlf.Length);
                    }
                    else
                    {
                        var blocksize = size / max; //calculate block length
                        var remainder = size % max; //calculate remaining length
                        var b = 0;
                        while (b < blocksize)
                        {
                            ms.Write(bytes, (b++) * max, max);
                            ms.Write(crlfs, 0, crlfs.Length);
                        }
                        if (remainder > 0)
                        {
                            ms.Write(bytes, blocksize * max, remainder);
                            ms.Write(crlf, 0, crlf.Length);
                        }
                    }
                }

                return encoding.GetString(ms.ToArray());
            }
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
        public override CalendarWriter InsertLineBreaks()
        {
            var unfolded = builder.ToString();
            if (!string.IsNullOrEmpty(unfolded) && !string.IsNullOrWhiteSpace(unfolded))
            {
                var folded = Fold(unfolded, CRLF, MAX, Encoding);
                builder = new StringBuilder(folded, folded.Length);
            }
            return this;
        }

        public override CalendarWriter RemoveLineBreaks()
        {
            var folded = builder.ToString();
            if (!string.IsNullOrEmpty(folded) && !string.IsNullOrWhiteSpace(folded))
            {
                var unfolded = folded.Replace(CRLF + " ", string.Empty);
                builder = new StringBuilder(unfolded, unfolded.Length);
            }
            return this;
        }
    }

    public class CalendarStreamWriter : CalendarWriter
    {
        private Stream stream;
        private static readonly string CRLF = Environment.NewLine;
        private const int MAX = 75;
        private const int BUFSIZE = 16 * 1024; //friendly to most CPU L1 caches
        private const char SPACE = '\u0020';

        public Stream Datasource => stream;

        public CalendarStreamWriter() : this(CultureInfo.CurrentCulture)
        {
        }

        public CalendarStreamWriter(IFormatProvider formatProvider) : base(formatProvider)
        {
            stream = new MemoryStream();
        }

        public CalendarStreamWriter(Stream stream, IFormatProvider formatProvider) : base(formatProvider)
        {
            this.stream = new MemoryStream();
            CopyStream(stream, this.stream, BUFSIZE);
        }

        public CalendarStreamWriter(Stream stream) : base(CultureInfo.CurrentCulture)
        {
            this.stream = new MemoryStream();
            CopyStream(stream, this.stream, BUFSIZE);
        }

        public static CalendarStreamWriter Create(Stream stream, IFormatProvider formatProvider)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            return new CalendarStreamWriter(stream, formatProvider);
        }

        private static void CopyStream(Stream input, Stream output, int bufferSize)
        {
            var buffer = new byte[bufferSize];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        private static Stream Fold(Stream stream, int bufferSize, string newline, int max, Encoding encoding)
        {
            var ms = CopyStream(stream, bufferSize);
            var crlf = encoding.GetBytes(newline); //CRLF
            var crlfs = encoding.GetBytes(newline + new string(SPACE, 1)); //CRLF and SPACE
            string line;

            var reader = new StreamReader(stream);
            while ((line = reader.ReadLine()) != null)
            {
                var bytes = encoding.GetBytes(line);
                var size = bytes.Length;
                if (size <= max)
                {
                    ms.Write(bytes, 0, size);
                    ms.Write(crlf, 0, crlf.Length);
                }
                else
                {
                    var blocksize = size / max; //calculate block length
                    var remainder = size % max; //calculate remaining length
                    var b = 0;
                    while (b < blocksize)
                    {
                        ms.Write(bytes, (b++) * max, max);
                        ms.Write(crlfs, 0, crlfs.Length);
                    }
                    if (remainder > 0)
                    {
                        ms.Write(bytes, blocksize * max, remainder);
                        ms.Write(crlf, 0, crlf.Length);
                    }
                }
            }

            return ms;
        }

        private static void ReinitializeStream(ref Stream stream)
        {
            stream.Dispose();
            stream = new MemoryStream();
        }

        private static void ReinitializeStream(ref Stream stream, byte[] bytes)
        {
            stream.Dispose();
            stream = new MemoryStream(bytes, 0, bytes.Length);
        }

        private static Stream CopyStream(Stream stream, int bufferSize)
        {
            var copy = new MemoryStream();
            CopyStream(stream, copy, bufferSize);
            return copy;
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
        public override CalendarWriter InsertLineBreaks()
        {
            if (stream.Length != 0L)
            {
                var folded = Fold(stream, BUFSIZE, CRLF, MAX, Encoding);
                ReinitializeStream(ref stream);
                CopyStream(folded, stream, BUFSIZE);
            }
            return this;
        }

        public override CalendarWriter RemoveLineBreaks()
        {
            if (stream.Length != 0L)
            {
                var reader = new StreamReader(stream);
                var folded = reader.ReadToEnd();
                var unfolded = folded.Replace(CRLF + " ", string.Empty);
                var bytes = Encoding.GetBytes(unfolded);
                ReinitializeStream(ref stream, bytes);
            }
            return this;
        }
    }
}
