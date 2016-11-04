using System;
using System.IO;
using System.Text;

namespace xcal.infrastructure.io.concretes.writers
{
    public class CalendarStreamWriter : CalendarWriter
    {
        private static readonly string CRLF = Environment.NewLine;
        private const int MAX = 75;
        private const int BUFSIZE = 16 * 1024; //friendly to most CPU L1 caches
        private const char SPACE = '\u0020';

        public CalendarStreamWriter(Stream stream) : base(new StreamWriter(stream, Encoding.UTF8))
        {
        }

        public CalendarStreamWriter(string path) : base(new StreamWriter(path))
        {
        }

        public CalendarStreamWriter(string path, bool append) : base(new StreamWriter(path, append, Encoding.UTF8))
        {
        }

        public CalendarStreamWriter(Stream stream, int bufferSize) : base(new StreamWriter(stream, Encoding.UTF8, bufferSize))
        {
        }

        public CalendarStreamWriter(Stream stream, int bufferSize, bool leaveOpen) : base(new StreamWriter(stream, Encoding.UTF8, bufferSize, leaveOpen))
        {
        }

        public CalendarStreamWriter(string path, bool append, int bufferSize) : base(new StreamWriter(path, append, Encoding.UTF8, bufferSize))
        {
        }

        private static Stream CreateStream(string text)
        {
            var stream = new MemoryStream();
            var swriter = new StreamWriter(stream);
            swriter.Write(text);
            stream.Position = 0;
            return stream;
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
            using (var stream = CreateStream(ToString()))
            {
                if (stream.Length != 0L)
                {
                    var folded = Fold(stream, BUFSIZE, CRLF, MAX, Encoding);
                    var output = new MemoryStream();
                    CopyStream(folded, output, BUFSIZE);
                    return new CalendarStreamWriter(output);
                }
            }
            return this;
        }

        public override CalendarWriter RemoveLineBreaks()
        {
            using (var stream = CreateStream(ToString()))
            {
                if (stream.Length != 0L)
                {
                    var reader = new StreamReader(stream);
                    var foldedstr = reader.ReadToEnd();
                    var unfoldedstr = foldedstr.Replace(CRLF + " ", string.Empty);
                    using (var unfolded = CreateStream(unfoldedstr))
                    {
                        var output = new MemoryStream();
                        CopyStream(unfolded, output, BUFSIZE);
                        return new CalendarStreamWriter(output);
                    }
                }
            }
            return this;
        }
    }
}
