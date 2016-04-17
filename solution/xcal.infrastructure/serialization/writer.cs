using System;
using System.IO;
using System.Text;
using System.Linq;

namespace reexjungle.xcal.infrastructure.serialization
{
    /// <summary>
    /// Represents a writer that provides a fast, non-cached, forward-only way to generate streams or files that contain iCalendar data.
    /// </summary>
    public class iCalWriter : TextWriter
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

        public iCalWriter(Encoding encoding = null)
        {
            Encoding = encoding ?? Encoding.UTF8;
        }

        public iCalWriter(IFormatProvider provider, Encoding encoding = null): base(provider)
        {
            Encoding = encoding ?? Encoding.UTF8;
        }

        public static iCalWriter Create(Stream stream)
        {
            return Create(new StreamWriter(stream));
        }

        public static iCalWriter Create(StringBuilder builder)
        {
            return Create(new StringWriter(builder));
        }

        public static iCalWriter Create(TextWriter writer)
        {
            return new iCalWriter(writer.FormatProvider, writer.Encoding);
        }

        public static iCalWriter Create(iCalWriter writer)
        {
            return new iCalWriter(writer.FormatProvider, writer.Encoding);
        }

        public void WriteStartComponent(string name)
        {
            WriteLine("BEGIN:{0}", name);
        }

        public void WriteEndComponent(string name)
        {
            WriteLine("END:{0}", name);
        }

        private string ToQuotedString(string value)
        {
            return DQUOTE + value.Replace(DQUOTE, EscapedDQUOTE) + DQUOTE;
        }


        private string ToSAFE_STRING(string value)
        {
            return value
                .Replace(COLON, EscapedCOLON)
                .Replace(SEMICOLON, EscapedSEMICOLON)
                .Replace(COMMA, EscapedCOMMA);
        }

        public string ToQSAFE_STRING(string value)
        {
            return value
                .Replace(HTAB, EMPTY)
                .Replace(DQUOTE, EscapedDQUOTE);
        }

        public void WriteXName(string name)
        {
            Write("X-" + name);
        }

        public void WriteValue(string value)
        {
            Write(value);
        }

        public void WriteSafeValue(string value)
        {
            Write(ToSAFE_STRING(value));
        }

        public void WriteQuotedValue(string value)
        {
            Write(ToQuotedString(value));
        }

        public void WriteParameter(string parameterName, params string[] parameterValues)
        {
            Write("{0}={1}",parameterName, string.Join(",", parameterValues));
        }

        public void WriteProperty(string propertyName, params string[] parameters)
        {
            Write("{0};{1}", propertyName, string.Join(";", parameters));
        }

        public void WriteProperty(string property)
        {
            WriteLine(property);
        }

        public void WriteProperties(params string[] properties)
        {
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                WriteLine(@property);
            }
        }

        public void WriteComponent(string componentName, params string[] properties)
        {
            WriteStartComponent(componentName);
            WriteProperties(properties);
            WriteEndComponent(componentName);
        }

        public override Encoding Encoding { get; }
    }

}
