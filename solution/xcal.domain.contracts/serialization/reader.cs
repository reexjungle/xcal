using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xcal.domain.contracts.serialization
{

    /// <summary>
    /// Represents a reader that provides fast, noncached, forward-only access to iCalendar data.
    /// </summary>
    public abstract class CalendarReader : TextReader
    {
        //literal Constants
        protected const char HTAB = '\u0009';
        protected const char EMPTY = '\0';
        protected const string DQUOTE = @"""";
        protected const string COMMA = ",";
        protected const string COLON = ":";
        protected const string SEMICOLON = ";";
        protected const string EQUALS = "=";
        protected const string EscapedDQUOTE = @"\""";
        protected const string EscapedCOMMA = @"\,";
        protected const string EscapedCOLON = @"\:";
        protected const string EscapedSEMICOLON = @"\;";
        protected const string EscapedEQUALS = @"\=";

        public string Name { get; private set; }

        public CalendarNodeType NodeType { get; protected set; }

        public virtual string Value { get; protected set; }

        public BeginCalendarNode BeginNode { get; protected set; }

        public EndCalendarNode EndNode{ get; protected set; }
                     
        public List<ValueCalendarNode> ValueNodes { get; protected set; }

        public List<ParameterCalendarNode> ParameterNodes { get; protected set; }

        public PropertyCalendarNode PropertyNode { get; protected set; }

        public abstract CalendarReader ReadFragment();

        public abstract bool CanPeek();

        public abstract bool CanRead();

        public abstract string ReadFromStart();

        private static string Escape(string value) =>
            value
                .Replace(EscapedCOMMA, "\\" + EscapedCOMMA)
                .Replace(EscapedCOLON, "\\" + EscapedCOLON)
                .Replace(EscapedSEMICOLON, "\\" + EscapedSEMICOLON)
                .Replace(EscapedEQUALS, "\\" + EscapedEQUALS);

        private static string Unescape(string value) =>
            value
                .Replace("\\" + EscapedCOMMA, EscapedCOMMA)
                .Replace("\\" + EscapedCOLON, EscapedCOLON)
                .Replace("\\" + EscapedSEMICOLON, EscapedSEMICOLON)
                .Replace("\\" + EscapedEQUALS, EscapedEQUALS);


        protected void ResetNodes()
        {
            BeginNode = BeginCalendarNode.Empty;
            EndNode = EndCalendarNode.Empty;
            PropertyNode = PropertyCalendarNode.Empty;
            ParameterNodes.Clear();
            ValueNodes.Clear();

        }


        private void UpdateReader(CalendarNode node)
        {
            Name = node.Name;
            NodeType = node.Type;
            Value = node.Value;
        }

        protected void Parse(string value)
        {
            var escapedValue = Escape(value);

            BeginNode = ExtractBeginCalendarNode(escapedValue);
            if (BeginNode != BeginCalendarNode.Empty)
            {
                UpdateReader(BeginNode);
                return;
            }

            EndNode = ExtractEndCalendarNode(escapedValue);
            if (EndNode != EndCalendarNode.Empty)
            {
                UpdateReader(EndNode);
                return;
            }

            PropertyNode = ExtractPropertyCalendarNode(escapedValue);
            if (PropertyNode != PropertyCalendarNode.Empty)
            {
                ParameterNodes.AddRange(PropertyNode.ParameterNodes);
                ValueNodes.AddRange(PropertyNode.ValueNodes);
                UpdateReader(PropertyNode);
                return;
            }

            var parameterNode = ExtractParameterCalendarNode(escapedValue);
            if (parameterNode != ParameterCalendarNode.Empty)
            {
                ParameterNodes.Add(parameterNode);
                ValueNodes.AddRange(parameterNode.ValueNodes);
                UpdateReader(parameterNode);
                return;
            }

            var valueNodes = ExtractValueCalendarNodes(EscapedCOMMA, escapedValue).ToList();
            if (valueNodes.Any())
            {
                ValueNodes.AddRange(valueNodes);
                UpdateReader(ValueCalendarNode.Flatten(EscapedCOMMA, valueNodes));
                return;
            }

            valueNodes = ExtractValueCalendarNodes(EscapedSEMICOLON, escapedValue).ToList();
            if (valueNodes.Any())
            {
                ValueNodes.AddRange(valueNodes);
                UpdateReader(ValueCalendarNode.Flatten(EscapedSEMICOLON, valueNodes));
            }

        }

        private static BeginCalendarNode ExtractBeginCalendarNode(string text)
        {
            var tokens = text.Split(new[] {EscapedCOLON}, StringSplitOptions.RemoveEmptyEntries);
            return tokens.Length == 2 && tokens[0].Equals("BEGIN", StringComparison.OrdinalIgnoreCase)
                ? new BeginCalendarNode(Unescape(tokens[1]))
                : BeginCalendarNode.Empty;
        }

        private static EndCalendarNode ExtractEndCalendarNode(string text)
        {
            var tokens = text.Split(new[] {EscapedCOLON}, StringSplitOptions.RemoveEmptyEntries);
            return tokens.Length == 2 && tokens[0].Equals("END", StringComparison.OrdinalIgnoreCase)
                ? new EndCalendarNode(Unescape(tokens[1]))
                : EndCalendarNode.Empty;
        }

        private static IEnumerable<ValueCalendarNode> ExtractValueCalendarNodes(string separator, string text)
        {
            var tokens = text.Split(new[] {separator}, StringSplitOptions.RemoveEmptyEntries);
            return tokens.Length > 0
                ? tokens.Select(x => new ValueCalendarNode(Unescape(x)))
                : Enumerable.Empty<ValueCalendarNode>();
        }

        private static ParameterCalendarNode ExtractParameterCalendarNode(string value)
        {
            var tokens = value.Split(new[] {EscapedEQUALS}, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 2)
            {
                var valueNodes = ExtractValueCalendarNodes(tokens[1], EscapedCOMMA);
                return new ParameterCalendarNode(tokens[0], valueNodes.ToArray());
            }
            return ParameterCalendarNode.Empty;
        }

        private static PropertyCalendarNode ExtractPropertyCalendarNode(string text)
        {
            var name = string.Empty;
            var values = new List<ValueCalendarNode>();
            var parameters = new List<ParameterCalendarNode>();

            //delimited by semicolons and colon
            if (text.Contains(EscapedCOLON))
            {
                var tokens = text.Split(new[] {EscapedCOLON}, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 2)
                {
                    var nodes = ExtractValueCalendarNodes(tokens[1], EscapedCOMMA);
                    if (nodes.Any()) values.AddRange(nodes);
                }

                var parts = tokens[0].Split(new[] {EscapedSEMICOLON}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    name = parts[0];
                    if (parts.Length > 1)
                    {
                        for (var i = 1; i < tokens.Length; i++)
                        {
                            var node = ExtractParameterCalendarNode(tokens[i]);
                            parameters.Add(node);
                        }
                    }
                }

                return new PropertyCalendarNode(name, parameters, values);
            }

            //Delimited by semicolons only
            if (text.Contains(EscapedSEMICOLON) && !text.Contains(EscapedCOLON))
            {
                var tokens = text.Split(new[] {EscapedSEMICOLON}, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                {
                    name = tokens[0];
                    if (tokens.Length > 1)
                    {
                        var valuestrings = tokens.Where((x, index) => index > 0).ToArray();
                        if (values.Any()) return new PropertyCalendarNode(name, valuestrings);
                    }
                }
            }
            return PropertyCalendarNode.Empty;
        }

    }

    public class CalendarTextReader: CalendarReader
    {
        private const char CR = '\r';
        private const char LF = '\n';

        private string source;
        private int position;

        public CalendarTextReader(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            this.source = source;
            position = 0;
        }

        private CalendarTextReader(string source, int position)
        {
            this.source = source;
            this.position = position;
        }

        public override CalendarReader ReadFragment()
        {
            if (source == null) throw new InvalidOperationException("Reader has been closed");

            //var fragment = ReadLineNoParse();
            //var reader = new CalendarTextReader(string.Copy(fragment), position);
            //return reader;

            throw new NotImplementedException();
        }

        public override bool CanPeek() => Peek() != -1;

        public override bool CanRead() => Read() != -1;

        public override int Peek()
        {
            if (source == null) throw new InvalidOperationException("Reader has been closed");
            return position != source.Length ? source[position] : -1;
        }

        public override int Read()
        {
            if (source == null) throw new InvalidOperationException("Reader has been closed");

            if (position == source.Length) return -1;
            var ch = source[position++];
            var fragment = source.Substring(0, position);
            Parse(fragment);
            return ch;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (index < 0)  throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)  throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - index < count) throw new ArgumentException("Invalid Offset Length");
            if (source == null) throw new InvalidOperationException("Reader has been closed");


            var size = source.Length - position;
            if (size > 0)
            {
                if (size > count) size = count;
                source.CopyTo(position, buffer, index, size);
                position += size;
                var fragment = new string(buffer);
                Parse(fragment);
            }
            return size;
        }

        public override string ReadFromStart()
        {
            if (source == null) throw new InvalidOperationException("Reader has been closed");

            var fragment = source.Substring(0, position);
            Parse(fragment);
            return fragment;
        }

        private static int SeekEndOfLine(string s, int pos)
        {
            var i = pos;
            while (i < s.Length)
            {
                var current = s[i];
                if (current == CR || current == LF)
                {
                    pos = i + 1;
                    if (current == CR && pos < s.Length && s[pos] == LF) pos++;
                    return pos;
                }
                i++;
            }
            if (i > pos) return pos;
            return -1;
        }

        //private static int SeekEndOfFragment(string s, int pos, CalendarNode node)
        //{
        //    var i = pos;
        //}

        public override string ReadLine()
        {
            if (source == null)
                throw new InvalidOperationException("Reader has been closed");

            var i = position;
            while (i < source.Length)
            {
                var current = source[i];
                if (current == CR || current == LF)
                {
                    var line = source.Substring(position, i - position);
                    position = i + 1;
                    if (current == CR && position < source.Length && source[position] == LF) position++;
                    Parse(line);
                    return line;
                }
                i++;
            }
            if (i > position)
            {
                var line = source.Substring(position, i - position);
                position = i;
                Parse(line);
                return line;
            }
            return null;
        }

        public override string ReadToEnd()
        {
            if (source == null)
                throw new InvalidOperationException("Reader has been closed");

            var fragment = position == 0 
                ? source 
                : source.Substring(position, source.Length - position);

            position = source.Length;
            Parse(fragment);
            return fragment;
        }

        public override void Close()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            source = null;
            position = 0;
            base.Dispose(disposing);
        }
    }

    public class CalendarStreamReader : CalendarReader
    {
        public override CalendarReader ReadFragment()
        {
            throw new NotImplementedException();
        }

        public override bool CanPeek()
        {
            throw new NotImplementedException();
        }

        public override bool CanRead()
        {
            throw new NotImplementedException();
        }

        public override string ReadFromStart()
        {
            throw new NotImplementedException();
        }

        public override int Peek()
        {
            throw new NotImplementedException();
        }

        public override int Read()
        {
            throw new NotImplementedException();
        }

        public override string ReadLine()
        {
            throw new NotImplementedException();
        }

        public override string ReadToEnd()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }
    }

}