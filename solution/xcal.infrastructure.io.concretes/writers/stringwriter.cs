using reexjungle.xcal.core.domain.contracts.io.writers;
using System;
using System.IO;
using System.Text;

namespace xcal.infrastructure.io.concretes.writers
{
    /// <summary>
    /// Represents a writer that encapsulates a <see cref="StringWriter"/> for writing iCalendar
    /// information to a string.
    /// </summary>
    public class CalendarStringWriter : CalendarWriter
    {
        private static readonly string CRLF = Environment.NewLine;
        private const int MAX = 75;
        private const char SPACE = '\u0020';

        /// <summary>
        /// Creates a new instance of the <see cref="CalendarStringWriter"/> class.
        /// </summary>
        public CalendarStringWriter() : base(new StringWriter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarStringWriter"/> that encapsulates
        /// the given <see cref="StringWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The writer, whose underlying <see cref="StringBuilder"/> is used to store information.
        /// </param>
        public CalendarStringWriter(StringWriter writer) : base(writer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarStringWriter"/> that writes to the
        /// specified <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">
        /// The string builder that initializes the underlying <see cref="StringWriter"/>
        /// encapsulated by this class.
        /// </param>
        public CalendarStringWriter(StringBuilder builder) : base(new StringWriter(builder))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarStringWriter"/> that has the
        /// specified format provider.
        /// </summary>
        /// <param name="provider">The <see cref="IFormatProvider"/> object that controls formatting.</param>
        public CalendarStringWriter(IFormatProvider provider) : base(new StringWriter(provider))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarStringWriter"/> that writes to the
        /// specified <see cref="StringBuilder"/> and has the specified format provider.
        /// </summary>
        /// <param name="builder">The StringBuilder to write to.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> object that controls formatting.</param>
        public CalendarStringWriter(StringBuilder builder, IFormatProvider provider) : base(new StringWriter(builder, provider))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalendarStringWriter"/> class.
        /// </summary>
        /// <returns>A new instance of the <see cref="CalendarStringWriter"/> class.</returns>
        public static CalendarWriter Create() => new CalendarStringWriter();

        /// <summary>
        /// Creates a new instance of the <see cref="CalendarStringWriter"/> class that encapsulates
        /// the specified <see cref="StringWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="StringWriter"/> that is encapsulated by the created <see cref="CalendarStringWriter"/>.
        /// </param>
        /// <returns>A new instance of the <see cref="CalendarStringWriter"/> class.</returns>
        public static CalendarWriter Create(StringWriter writer) => new CalendarStringWriter(writer);

        /// <summary>
        /// Creates a new instance of the <see cref="CalendarStringWriter"/> class that writes to the
        /// specified <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to write to.</param>
        /// <returns>A new instance of the <see cref="CalendarStringWriter"/> class.</returns>
        public static CalendarWriter Create(StringBuilder builder) => new CalendarStringWriter(builder);

        /// <summary>
        /// Creates a new instance of the <see cref="CalendarStringWriter"/> class that writes to the
        /// specified <see cref="StringBuilder"/> and has the specified format provider.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to write to.</param>
        /// <param name="provider">The <see cref="IFormatProvider"/> object that controls formatting.</param>
        /// <returns>A new instance of the <see cref="CalendarStringWriter"/> class.</returns>
        public static CalendarWriter Create(StringBuilder builder, IFormatProvider provider) => new CalendarStringWriter(builder, provider);

        /// <summary>
        /// Creates a new instance of the <see cref="CalendarStringWriter"/> class that writes to the
        /// specified <see cref="StringBuilder"/> and has the specified format provider.
        /// </summary>
        /// <param name="provider">The <see cref="IFormatProvider"/> object that controls formatting.</param>
        /// <returns>A new instance of the <see cref="CalendarStringWriter"/> class.</returns>
        public static CalendarWriter Create(IFormatProvider provider) => new CalendarStringWriter(provider);

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
        public override ICalendarWriter InsertLineBreaks()
        {
            var unfolded = ToString();
            if (!string.IsNullOrEmpty(unfolded) && !string.IsNullOrWhiteSpace(unfolded))
            {
                var folded = Fold(unfolded, CRLF, MAX, Encoding);
                return new CalendarStringWriter(new StringBuilder(folded, folded.Length));
            }
            return this;
        }

        /// <summary>
        /// Removes line breaks after every 75 characters in the string representation.
        /// </summary>
        /// <returns>The actual instance of the <see cref="CalendarWriter"/> class.</returns>
        public override ICalendarWriter RemoveLineBreaks()
        {
            var folded = ToString();
            if (!string.IsNullOrEmpty(folded) && !string.IsNullOrWhiteSpace(folded))
            {
                var unfolded = folded.Replace(CRLF + " ", string.Empty);
                return new CalendarStringWriter(new StringBuilder(unfolded, unfolded.Length));
            }
            return this;
        }
    }
}
